using BlApi;
using BO;
using System.Collections.Generic;

namespace BlImplementation;
internal class MilestoneImplementation : IMilestone
{
    private static readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Private inner class that implements IEqualityComparer to enable the distinct operation on <IEnumerable<T>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private class CollectionComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        public bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
        {
            return x!.SequenceEqual(y!);
        }

        public int GetHashCode(IEnumerable<T> obj)
        {
            return obj.Aggregate(17, (hash, item) => hash * 31 + item!.GetHashCode());
        }
    }

    #region private help functions

    /// <summary>
    /// In case an attempt is made to create a timetable that fails, 
    /// the function will delete the incorrect data saved in the database.
    /// </summary>
    private static void CleaerIllegalSchedual()
    {
        _dal.Task.ReadAll()
            .ToList()
            .ForEach(t => _dal.Task.Update(t with { DeadlineDate = null, SchedualDate = null }));
    }

    /// <summary>
    /// The function takes the list of dependencies and creates in the database a new list of dependencies between tasks and milestones and vice versa.
    /// </summary>
    private static void CreateMilestones()
    {
        //Milestone runner ID number
        int nextId = 1;
        int getNextId() => nextId++;

        //List of all existing dependencies
        var dependenciesList = _dal.Dependency.ReadAll().ToList();

        //Deleting the dependency pool
        _dal.Dependency.Reset();

        //ID numbers of all tasks
        var tasks = (from t in _dal.Task.ReadAll()
                     select t.Id).ToList();


        //A collection that contains for each dependent task:
        //Key: ID number of pending task
        //Tasks: collection of ID numbers of the previous tasks.

        var dependentGroups = from DO.Dependency d in dependenciesList
                              group d by d.DependentTask into gs
                              select new
                              {
                                  key = gs.Key,
                                  tasks = gs.Select(g => g.DependsOnTask).OrderBy(task => task).ToList()
                              };



        //A collection of lists of previous tasks without duplicates
        var distinctGroups = dependentGroups.Select(d => d.tasks).Distinct(new CollectionComparer<int>());

        //Creating a first milestone
        int firstMilestonId = _dal.Task.Create(new(0, "", "Start", true, DateTime.Now, null, null, null, null, null, "", "", 0, 0));

        //Create a dependency between each task that does not depend on any task, to the first milestone.
        tasks
            .Where(t => !dependenciesList.Any(d => d.DependentTask == t))
            .ToList()
            .ForEach(t => _dal.Dependency.Create(new(0, t, firstMilestonId)));

        //Create a new milestone for each list of previous tasks
        var milestones = distinctGroups.Select(previousTasks => new
        {
            MilestoneTaskId = _dal.Task.Create(new(0, "", $"M{getNextId()}", true, DateTime.Now, null, null, null, null, null, "", "", 0, 0)),
            PreviousTasks = previousTasks.ToList()
        }).ToList();

        //Create a final milestone and add it to the list of milestones
        //Its previous task list is all tasks that no task is dependent on
        var lastMileston = new
        {
            MilestoneTaskId = _dal.Task.Create(new(0, "", "End", true, DateTime.Now, null, null, null, null, null, "", "", 0, 0)),
            PreviousTasks = tasks.Where(t => !dependenciesList.Any(d => d.DependsOnTask == t)).ToList()
        };
        milestones.Add(lastMileston);

        //Create a new dependency between a milestone and each of its previous tasks
        var milestonesDependencies = (from m in milestones
                                      from t in m.PreviousTasks
                                      select (_dal.Dependency.Create(new(0, m.MilestoneTaskId, t)))).ToList();

        //Create a dependency between each task and a milestone corresponding to its list of previous tasks.
        var tasksDependencies = (from milestone in milestones
                                 from dependency in dependentGroups
                                 where milestone.PreviousTasks.SequenceEqual(dependency.tasks)
                                 select (_dal.Dependency.Create(new(0, dependency.key, milestone.MilestoneTaskId)))).ToList();

    }

    /// <summary>
    /// The function receives a milestone, and updates the deadline date for the milestone and for all tasks preceding it.
    /// </summary>
    /// <param name="milestone">Milestone DAL object</param>
    /// <param name="end">The deadline date for updating</param>
    /// <returns>A list of the next milestones in line, and for each a calculated deadline.</returns>
    private static IEnumerable<(DO.Task? prevMileston, DateTime nextEndDate)> SetEndDate(DO.Task milestone, DateTime end)
    {
        //In case the last date has already been updated in the milestone and it is earlier than the end,
        //we will update the previous tasks with the existing date in the milestone.
        if (milestone.DeadlineDate != null && milestone.DeadlineDate < end)
            end = milestone.DeadlineDate.Value;
        else
            _dal.Task.Update(milestone with { DeadlineDate = end });

        //All dependencies in which the milestone is the dependent task
        var prevDependencies = _dal.Dependency
            .ReadAll(d => d.DependentTask == milestone.Id)
            .Select(d => d.DependsOnTask)
            .ToList();

        if (!prevDependencies.Any()) return new List<(DO.Task? prevMileston, DateTime nextEndDate)> { (prevMileston: null, nextEndDate: end) };

        var prevTasks = prevDependencies
            .Select(d => _dal.Task.Read(d))
            .ToList();

        var prevMilestonsDependencies = _dal.Dependency
            .ReadAll()
            .Where(d => prevDependencies
            .Contains(d.DependentTask))
            .GroupBy(d => d.DependsOnTask)
            .ToList();

        List<(DO.Task? prevMileston, DateTime nextEndDate)> prevMilstones = new();

        foreach (var group in prevMilestonsDependencies)
        {
            var prevMileston = _dal.Task.Read(group.Key)!;

            var Tasks = group.Select(dep => prevTasks.First(t => t!.Id == dep.DependentTask));

            var nextEndDates = Tasks.Select(task =>
             {
                 //Ensure that the deadline date to be updated for the task is no later than the existing date, if any.
                 var endDate = task!.DeadlineDate == null || task.DeadlineDate > end ? end : task!.DeadlineDate;

                 //The possible deadline date for the milestone
                 var nextEndDate = endDate?.Subtract(task.Duration!.Value);

                 //Update the deadline date for the task
                 _dal.Task.Update(task! with { DeadlineDate = endDate });

                 return nextEndDate!.Value;
             });

            var nextEndDate = nextEndDates.Min();

            prevMilstones.Add((prevMileston, nextEndDate));
        }
        return prevMilstones;
    }

    /// <summary>
    /// A recursive function that updates last possible finish dates for tasks and milestones, from the last milestone and back.
    /// </summary>
    /// <param name="lastMileston">The last milestone</param>
    /// <param name="end">Project completion date</param>
    /// <returns>the earliest end date  of all end dates.</returns>
    private static DateTime ScheduleEnd(DO.Task lastMilestone, DateTime end)
    {
        var prevMilestones = SetEndDate(lastMilestone, end);
        DateTime finalEndDate = DateTime.MinValue; // Initialize with a default value

        foreach (var (prevMilestone, nextEndDate) in prevMilestones)
        {
            if (prevMilestone != null)
            {
                finalEndDate = ScheduleEnd(prevMilestone, nextEndDate);
            }
        }

        if (finalEndDate == DateTime.MinValue)
        {
            return prevMilestones.Last().nextEndDate; // Return the end date of the last element if no valid result is found
        }

        return finalEndDate; // Return the last obtained result from the recursion
    }

    /// <summary>
    /// The function does the same as the SetEndDate functionת
    /// only that the date to be updated is a planned date for starting work,
    /// and each time we will update the milestone, and the tasks that depend on it.
    /// </summary>
    /// <param name="milestone">mileston Dal object</param>
    /// <param name="start">Project start date</param>
    /// <returns> a list of next milestones and start dates accordingly.</returns>
    private static IEnumerable<(DO.Task? nextMileston, DateTime nextStartDate)> SetStartDate(DO.Task milestone, DateTime start)
    {
        if (milestone.SchedualDate != null && milestone.SchedualDate > start)
            start = milestone.SchedualDate.Value;
        else
            _dal.Task.Update(milestone with { SchedualDate = start });

        var nextDependencies = _dal.Dependency
            .ReadAll(d => d.DependsOnTask == milestone.Id)
            .Select(d => d.DependentTask)
            .ToList();

        if (!nextDependencies.Any()) return new List<(DO.Task? nextMileston, DateTime nextStartDate)> { (nextMileston: null, nextStartDate: start) };

        var nextTasks = nextDependencies
            .Select(d => _dal.Task.Read(d))
            .ToList();

        var nextMilestonsDependencies = _dal.Dependency
            .ReadAll()
            .Where(d => nextDependencies
            .Contains(d.DependsOnTask))
            .GroupBy(d => d.DependentTask)
            .ToList();

        List<(DO.Task? nextMileston, DateTime nextStartDate)> nextMilestons = new();

        foreach (var group in nextMilestonsDependencies)
        {
            var nextMileston = _dal.Task.Read(group.Key);

            var Tasks = group.Select(dep => nextTasks.First(t => t!.Id == dep.DependsOnTask)).ToList();

            var startDates = Tasks.Select(task =>
            {
                var startDate = start;
                if (task!.SchedualDate == null || task.SchedualDate < start) _dal.Task.Update(task! with { SchedualDate = startDate });
                else startDate = task.SchedualDate.Value;
                var nextStartDate = startDate.Add(task.Duration!.Value);

                return nextStartDate;
            });

            nextMilestons.Add((nextMileston, nextStartDate: startDates.Max()));
        }
        return nextMilestons;
    }

    /// <summary>
    /// A recursive function to calculate and update a planned start date for milestones and tasks.
    /// </summary>
    /// <param name="firstMileston"></param>
    /// <param name="start"></param>
    /// <returns></returns>
    private static DateTime ScheduleStart(DO.Task firstMilestone, DateTime start)
    {
        var nextMilestones = SetStartDate(firstMilestone, start);
        DateTime finalStartDate = DateTime.MinValue; // Initialize with a default value

        foreach (var (nextMilestone, nextStartDate) in nextMilestones)
        {
            if (nextMilestone != null)
            {
                finalStartDate = ScheduleStart(nextMilestone, nextStartDate);
            }
        }

        if (finalStartDate == DateTime.MinValue)
        {
            return nextMilestones.Last().nextStartDate; // Return the start date of the last element if no valid result is found
        }

        return finalStartDate; // Return the last obtained result from the recursion
    }

    private static Status GetStatus(double percentage) => percentage == 0.0 ? Status.Scheduled : percentage == 100.0 ? Status.Done : Status.OnTrack;

    private static Milestone SetValues(DO.Task task)
    {

        Milestone milestone = task.Convert<DO.Task, Milestone>();
        milestone.Dependencies = _dal.GetListOfTasks(task);
        int doneTasks = milestone.Dependencies.Where(t => t.Status == Status.Done).Count();
        int oneTrackTasks = milestone.Dependencies.Where(t => t.Status == Status.OnTrack).Count();
        milestone.CompletionPercentage = (doneTasks + (oneTrackTasks * 0 / 5)) != 0 ? (doneTasks + (oneTrackTasks * 0 / 5)) / milestone.Dependencies.Count * 100.0 : milestone.Dependencies.Count == 0 ? 100.0 : 0.0;
        milestone.Status = GetStatus((double)milestone.CompletionPercentage);
        return milestone;
    }
    #endregion

    public bool ScheduleExists { get => _dal.GetProjectStatus() == ProjectStatus.Execution; }

    /// <summary>
    /// A function to create a project schedule automatically
    /// </summary>
    /// <param name="start">Requested date for the start of the project</param>
    /// <param name="end">Requested date for the end of the project</param>
    /// <exception cref="BlIllegalException">Failure to create schedule</exception>
    public void CreateSchedule(DateTime start, DateTime end)
    {
        //Checking that the project is not running
        if (ScheduleExists)
            throw new BlIllegalException("Schedule", "creation", "A project not in the planning stage.");
        //If we have already tried to create a schedule, we will clear the previous attempt.
        if (_dal.GetProjectStatus() == ProjectStatus.Scheduling)
            CleaerIllegalSchedual();
        //Otherwise we will create milestones and change project status
        else
        {
            CreateMilestones();
            _dal.SetProjectStatus(ProjectStatus.Scheduling);
        }
        DO.Task? _lastMilestone = _dal.Task.Read(t => t.Alias == "End");
        //We will check the correctness of the dates and put values in the tasks, the milestones, the project dates, and the project status.
        if (ScheduleEnd(_lastMilestone!, end) < start)
            throw new BlIllegalException("Schedule", "creation", "End date too early.");
        if (ScheduleStart(_dal.Task.Read(t => t.Alias == "Start")!, start) > end)
            throw new BlIllegalException("Schedule", "creation", "Start date too late.");
        _dal.StartDate = start;
        _dal.EndDate = end;
        _dal.SetProjectStatus(ProjectStatus.Execution);
    }

    public Milestone GetMilestone(int id)
    {
        DO.Task? task = _dal.Task.Read(id);
        if (task == null || !task.IsMilestone) throw new BlDoesNotExistException("Milestone");
        return SetValues(task);
    }

    public Milestone UpdateMilestone(int id, string alias, string description, string? comments)
    {
        DO.Task? toUpdate = _dal.Task.Read(id);
        if (toUpdate == null || !toUpdate.IsMilestone)
            throw new BlDoesNotExistException("Milestone");
        description = string.IsNullOrEmpty(description) ? toUpdate.Description : description;
        alias = string.IsNullOrEmpty(alias) ? toUpdate.Alias : alias;
        comments = string.IsNullOrEmpty(comments) ? toUpdate.Remarks : comments;
        toUpdate = toUpdate with { Alias = alias, Description = description, Remarks = comments };
        _dal.Task.Update(toUpdate);
        return SetValues(toUpdate);
    }

    public IEnumerable<MilestoneInList> GetAllMilestones(Func<Milestone, bool>? filter = null)
    {
        var milstones = _dal.Task.ReadAll(t => t.IsMilestone == true).Select(t => SetValues(t));
        if (filter != null)
            milstones = milstones.Where(filter);
        return milstones.ConvertList<Milestone, MilestoneInList>();
    }
}
