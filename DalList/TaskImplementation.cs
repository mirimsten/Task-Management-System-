namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;

internal class TaskImplementation : ITask
{
    public int Create(Task _task)
    {
        int newId = DataSource.Config.NextTaskId;
        Task task = _task with { Id = newId };
        DataSource.Tasks.Add(task);
        return newId;
    }

    public void Reset()
    {
        DataSource.Tasks.Clear();
    }

    public void Delete(int id)
    {
        if (DataSource.Tasks.FirstOrDefault(t => t!.Id == id) == null)
        {
            throw new DalDoesNotExistException($"Can't delete, task with ID: {id} does not exist!!");
        }
        DataSource.Tasks.RemoveAll(t => t!.Id == id);

    }

    public Task? Read(int id)
    {
        return DataSource.Tasks.FirstOrDefault(t => t?.Id == id);
    }

    public Task? Read(Func<Task, bool> filter)
    {
        return DataSource.Tasks.FirstOrDefault(filter!);
    }

    public IEnumerable<Task> ReadAll(Func<Task, bool>? filter = null)
    { 
        return filter == null ? DataSource.Tasks.Select(t => t) : DataSource.Tasks.Where(filter);
    }

    public void Update(Task _task)
    {
        if (DataSource.Tasks.FirstOrDefault(t => t!.Id == _task.Id) == null)
        {
            throw new DalDoesNotExistException($"Can't update, task with ID: {_task?.Id} does not exist!!");
        }
        DataSource.Tasks.RemoveAll(t => t!.Id == _task.Id);
        DataSource.Tasks.Add(_task);
    }
}
