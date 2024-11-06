namespace Dal;
using DalApi;
using DO;

internal class TaskImplementation : ITask
{
    const string taskRoot = "tasks"; //XML Serializer

    public int Create(Task task)
    {
        var tasksList = XMLTools.LoadListFromXMLSerializer<Task>(taskRoot);
        int id = Config.NextTaskId;
        tasksList.Add(task with { Id = id });
        XMLTools.SaveListToXMLSerializer(tasksList, taskRoot);
        return id;
    }

    public void Reset()
    {
        var tasksList = XMLTools.LoadListFromXMLSerializer<Task>(taskRoot);
        tasksList.Clear();
        XMLTools.SaveListToXMLSerializer(tasksList, taskRoot);
        Config.ResetConfig();

    }

    public void Delete(int id)
    {
        var tasksList = XMLTools.LoadListFromXMLSerializer<Task>(taskRoot);
        if (tasksList.RemoveAll(t => t.Id == id) == 0)
            throw new DalDoesNotExistException($"Can't delete, task with ID: {id} does not exist!!");
        XMLTools.SaveListToXMLSerializer(tasksList, taskRoot);
    }

    public Task? Read(int id) =>
    
        XMLTools.LoadListFromXMLSerializer<Task>(taskRoot).FirstOrDefault(t => t?.Id == id) ?? null;       

    public Task? Read(Func<Task, bool> filter) =>

        XMLTools.LoadListFromXMLSerializer<Task>(taskRoot).FirstOrDefault(filter) ?? null;
        
    public IEnumerable<Task> ReadAll(Func<Task, bool>? filter = null)
    {
        var tasksList = XMLTools.LoadListFromXMLSerializer<Task>(taskRoot);
        return filter == null? tasksList.Select(t=>t):tasksList.Where(filter);
    }

    public void Update(Task task)
    {
        var tasksList = XMLTools.LoadListFromXMLSerializer<Task>(taskRoot);
        tasksList.RemoveAll(t => t.Id == task.Id);
        tasksList.Add(task);
        XMLTools.SaveListToXMLSerializer(tasksList, taskRoot);
    }
}
