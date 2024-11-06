using BlApi;
using BO;
using System.Net.Mail;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace BlImplementation;
internal class EngineerImplementation : IEngineer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// A helper function that checks for an engineer object whether its values are valid and it is allowed to update/create it in the data layer.
    /// </summary>
    /// <param name="e">an engineer object</param>
    /// <exception cref="BlNullException">error if enineer is null</exception>
    /// <exception cref="BlInvalidException">error if one of the values is not valid</exception>
    private static void TestEngineer(Engineer e)
    {
        if (e == null)
            throw new BlNullException("Engineer");
        if (e.Id < 0)
            throw new BlInvalidException("Id");
        if (string.IsNullOrEmpty(e.Name))
            throw new BlInvalidException("Name");
        if (e.Cost < 0)
            throw new BlInvalidException("Cost");
        if (!MailAddress.TryCreate(e.Email, out _))
            throw new BlInvalidException("Email");
    }
    public int CreateEngineer(Engineer engineer)
    {
        TestEngineer(engineer);
        try
        {
            return _dal.Engineer.Create(engineer.Convert<Engineer, DO.Engineer>());
        }
        catch (DO.DalAlreadyExistsException e)
        {
            throw new BlAlreadyExistsException(e);
        }
    }
    public void DeleteEngineer(int id)
    {
        //Prevent the possibility of deleting an engineer who is assigned a task while the project is running.
        if (_dal.GetProjectStatus() == ProjectStatus.Execution)
        {
            if (_dal.Task.Read(t => t?.EngineerId == id) != null)
                throw new BlIllegalException("engineer", "deletion");
        }
        try
        {
            _dal.Engineer.Delete(id);
        }
        catch (DO.DalDoesNotExistException e)
        {
            throw new BlDoesNotExistException(e);
        }
    }
    public Engineer GetEngineer(int id)
    {
        DO.Engineer de = _dal.Engineer.Read(id) ?? throw new BlDoesNotExistException($"Engineer {id}");
        TaskInEngineer? t = _dal.Task.Read(t => t.EngineerId == de.Id)?.Convert<DO.Task, TaskInEngineer>() ?? null;
        Engineer be = de.Convert<DO.Engineer, Engineer>();
        be.Task = t;
        return be;
    } 
    public IEnumerable<EngineerInList> ReadAllEngineers(Func<DO.Engineer, bool>? filter = null)
    {
        return (filter is not null) ?
        _dal.Engineer.ReadAll(filter).ConvertList<DO.Engineer, EngineerInList>() :
        _dal.Engineer.ReadAll().ConvertList<DO.Engineer, EngineerInList>();
    }
    public void UpdateEngineer(Engineer engineer)
    {
        TestEngineer(engineer);
        Engineer beforeUpdates = GetEngineer(engineer.Id);

        //Makes sure the engineer level is not lowered.
        if (beforeUpdates.Level > engineer.Level) throw new BlIllegalException("level", "update");

        //Checking if an update is made to the task assigned to the engineer, and updating accordingly in the data layer.
        if (beforeUpdates.Task != engineer.Task)
        {
            //If he was already assigned a task - delete the old assignment
            if (beforeUpdates.Task != null)
            {
                DO.Task lastTask = _dal.Task.Read(beforeUpdates.Task.Id)!;
                DO.Task updateTask = lastTask with { EngineerId = null };
                _dal.Task.Update(updateTask);
            }

            //Checking if a new task is assigned (and not just deleting an assignment)
            if (engineer.Task != null)
            {
                DO.Task newTask = _dal.Task.Read(engineer.Task.Id) ?? throw new BlIllegalException("task", "allotment");
                DO.Task updateTask = newTask with { EngineerId = engineer.Id, StartDate= DateTime.Now };
                _dal.Task.Update(updateTask);
            }
        }
        _dal.Engineer.Update(engineer.Convert<Engineer, DO.Engineer>());
    }
}
