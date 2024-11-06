using BlApi;
using BO;

namespace BlImplementation;

internal class TaskImplementation : ITask
{
    private static readonly DalApi.IDal _dal = DalApi.Factory.Get;

    #region Private helper functions
    /// <summary>
    /// A helper function that checks for a task object whether its values are valid and it is allowed to update/create it in the data layer.
    /// </summary>
    /// <param name="task">a task object</param>
    /// <exception cref="BlNullException">error if task is null</exception>
    /// <exception cref="BlInvalidException">error if one of the values is not valid</exception>
    private static void TestTask(BO.Task task)
    {
        if (task == null)
            throw new BlNullException("Task");
        if (task.Id < 1)
            throw new BlInvalidException("Id");
        if (string.IsNullOrEmpty(task.Alias))
            throw new BlInvalidException("Alias");
    }

    /// <summary>
    /// A function that receives a data layer task entity, and calculates the object values of the logical task.
    /// </summary>
    /// <param name="task">DO task object</param>
    /// <returns>A BO task object is filled with values.</returns>
    private static BO.Task SetValues(DO.Task task)
    {
        BO.Task result = task.Convert<DO.Task, BO.Task>();

        result.Status = Tools.GetTaskStatus(task);

        result.Complexity = (EngineerExperience)task.ComplexityLevel;

        //If there are start dates, calculate the estimated end date.
        if (task.StartDate != null && task.SchedualDate != null)
        {
            result.ForecastDate = ((DateTime)(task.StartDate < task.SchedualDate ? task.SchedualDate : task.StartDate)).Add((TimeSpan)task.Duration!);
        }

        //If the project is in planning, we will import the list of dependencies
        if (_dal.GetProjectStatus() == ProjectStatus.Planning) result.Dependencies = _dal.GetListOfTasks(task);
        else
        {
            var dep = _dal.Dependency.Read(d => d.DependentTask == task.Id);
            var milstone = _dal.Task.Read(t => t.Id == dep!.DependsOnTask)!;
            result.Milestone = new() { Id = milstone.Id, Alias = milstone.Alias };
        }

        //If an engineer is assigned to the task, we will import him
        if (task.EngineerId != null && task.EngineerId != 0)
        {
            result.Engineer = _dal.Engineer.Read(task.EngineerId!.Value)!.Convert<DO.Engineer, EngineerInTask>();
        }
        return result;
    }
    #endregion

    public void CreateTask(BO.Task task)
    {
        //Make sure the project is not yet running and it is allowed to add tasks
        if (_dal.GetProjectStatus() == ProjectStatus.Execution) throw new BlIllegalException("task", "creation");
        TestTask(task);
        try
        {
            _dal.Task.Create(task.Convert<BO.Task, DO.Task>());
        }
        catch (DO.DalAlreadyExistsException e)
        {
            throw new BlAlreadyExistsException(e);
        }
    }

    public void DeleteTask(int id)
    {
        //Make sure that the deletion of the task is legal (the project is not running, and there are no other tasks that depend on it).
        if (_dal.GetProjectStatus() == ProjectStatus.Execution) throw new BlIllegalException("task", "deletion");
        if (_dal.Dependency.Read(d => d.DependsOnTask == id) != null) throw new BlIllegalException("task", "deletion");

        try { _dal.Task.Delete(id); }
        catch (DO.DalDoesNotExistException e) { throw new BlDoesNotExistException(e); }

        //Deleting all dependencies related to the task.
        foreach (var d in _dal.Dependency.ReadAll(d => d.DependentTask == id))
            _dal.Dependency.Delete(d.Id);



    }

    public BO.Task GetTask(int id)
    {
        DO.Task task = _dal.Task.Read(id) ?? throw new BlDoesNotExistException($"Task {id}");
        return SetValues(task);
    }

    public IEnumerable<TaskInList> ReadAllTasks(Func<DO.Task, bool>? filter = null)
    {
        var tasks = _dal.Task.ReadAll(t => !t.IsMilestone);
        if (filter is not null)
        {
            tasks = _dal.Task.ReadAll(filter);
        }
        var BOtasks = tasks.ConvertList<DO.Task, TaskInList>();
        BOtasks.ForEach(d => d.Status = Tools.GetTaskStatus(tasks, d.Id));
        return BOtasks;
    }

    public IEnumerable<TaskInList> ReadAllTasks(Func<BO.Task, bool>? filter = null)
    {
        var tasks = _dal.Task.ReadAll(t => !t.IsMilestone).ToList();
        if (filter is not null)
        {
            var filterTasks = tasks.Select(t=> GetTask(t.Id)).Where(filter);
            return filterTasks.ConvertList<BO.Task, TaskInList>();
        }
        var BOtasks = tasks.ConvertList<DO.Task, TaskInList>();
        BOtasks.ForEach(d => d.Status = Tools.GetTaskStatus(tasks, d.Id));
        return BOtasks;
    }

    public void UpdateTask(BO.Task task)
    {
        TestTask(task);
        DO.Task beforeUpdates = _dal.Task.Read(task.Id) ?? throw new BlDoesNotExistException("Task");

        //Makes sure that the date values have not been changed while the project is running
        if (_dal.GetProjectStatus() == ProjectStatus.Execution)
            if (beforeUpdates.SchedualDate != task.SchedualDate ||
                beforeUpdates.DeadlineDate != task.DeadlineDate ||
                beforeUpdates.Duration != task.Duration)
                throw new BlIllegalException("task", "update");

        if (task.Dependencies != null)
        {
            foreach (var dep in task.Dependencies)
            {
                if (_dal.Dependency.Read(d => d.DependsOnTask == dep.Id && d.DependentTask == task.Id) == null)
                    _dal.Dependency.Create(new(0, task.Id, dep.Id));
            }
        }

        task.Status = Tools.GetTaskStatus(beforeUpdates with { CompleteDate = task.CompleteDate, StartDate = task.StartDate });
        _dal.Task.Update(task.Convert<BO.Task, DO.Task>() with { ComplexityLevel = (DO.EngineerExperience)task.Complexity, EngineerId = task.Engineer?.Id });
    }
}
