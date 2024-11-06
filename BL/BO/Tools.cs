
using DalApi;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace BO;
public static class Tools
{
    public static ProjectStatus? GetProjectStatus(this IDal d) => (ProjectStatus?)d.ProjectStatus;
    public static void SetProjectStatus(this IDal d, ProjectStatus s) => d.ProjectStatus = (DO.ProjectStatus)s;

    /// <summary>
    /// A function that is used in reflection to convert DAL entities to BO entities by property names.
    /// </summary>
    /// <typeparam name="TS">source type</typeparam>
    /// <typeparam name="TD">destination type</typeparam>
    /// <param name="source">source object</param>
    /// <returns>TS type new object</returns>   
    public static TD Convert<TS, TD>(this TS source) where TD : new()
    {
        TD destination = new();
        var srcPropsWithValues = typeof(TS)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(x => x.Name, y => y.GetValue(source));

        var dstProps = typeof(TD)
       .GetProperties(BindingFlags.Public | BindingFlags.Instance)
       .ToDictionary(key => key, value => value.GetCustomAttribute<DefaultValueAttribute>()?.Value
                                       ?? (value.PropertyType.IsValueType
                                       ? Activator.CreateInstance(value.PropertyType, null)
                                       : null));
        foreach (var prop in dstProps)
        {
            var destProperty = prop.Key;

            if (srcPropsWithValues.ContainsKey(destProperty.Name))
            {
                var defaultValue = prop.Value;
                var sourceValue = srcPropsWithValues[destProperty.Name];

                destProperty.SetValue(destination, sourceValue ?? defaultValue);
            }
        }
        return destination;
    }

    /// <summary>
    /// A function that use the CONVERT function to convert types of entire lists.
    /// </summary>
    /// <typeparam name="TS">source type</typeparam>
    /// <typeparam name="TD">destination type</typeparam>
    /// <param name="list">list of source-type objects</param>
    /// <returns>list of TD objects</returns>
    public static List<TD> ConvertList<TS,TD>(this IEnumerable<TS> list) where TD : new()
    {
        return list.Select(d => d.Convert<TS, TD>()).ToList();
    }

    /// <summary>
    /// A function that uses reflection to print all types of objects with a generic override to the TOSTRING function
    /// </summary>
    /// <typeparam name="T">object type</typeparam>
    /// <param name="obj">object to print</param>
    /// <returns>string of the object values</returns>
    public static string ToStringProperties<T>(this T obj)
    {
        var result = new StringBuilder();
        foreach (var prop in typeof(T).GetProperties())
        { 
            var value = prop.GetValue(obj);
            if(prop.PropertyType.GetInterfaces().Contains(typeof(IList)))
            {
                var sb = new StringBuilder();
                foreach (var item in (IEnumerable)prop.GetValue(obj, null)!)
                {
                    sb.Append($"\t\n{item}");
                }
                value = sb.ToString();
            }            
            result.Append($"{prop.Name}:{value}\n");
        }
        return result.ToString();
    }

    /// <summary>
    /// Help method for calculating the status of a task according to the dates of the DAL entity
    /// </summary>
    /// <param name="task">a DO task object</param>
    /// <returns>the tasks status</returns>
    public static Status GetTaskStatus(DO.Task task) => task.SchedualDate is null ? Status.Unscheduled : task.StartDate is null ? Status.Scheduled : task.CompleteDate is null ? Status.OnTrack : Status.Done;

    /// <summary>
    /// A function that is loaded on GetStatus, to get the status of a task from a list of tasks.
    /// </summary>
    /// <param name="tasks">Collection of tasks</param>
    /// <param name="id">ID number of the task for which we will return a status</param>
    /// <returns>the task status</returns>
    public static Status GetTaskStatus(IEnumerable<DO.Task> tasks, int id) => GetTaskStatus(tasks.FirstOrDefault(t => t.Id == id)!);

    /// <summary>
    /// A function that retrieves a list of previous tasks for the Task and Milestone objects.
    /// </summary>
    /// <param name="dal">IDAL object</param>
    /// <param name="task">the cuurent task/milestone</param>
    /// <returns>list of previous tasks</returns>
    public static List<TaskInList> GetListOfTasks (this IDal dal, DO.Task task)
    {
        //ID numbers of all tasks that the current task depends on
        var dependencies = dal.Dependency.ReadAll(d => d.DependentTask == task.Id).Select(d => d.DependsOnTask).ToList();
        //Retrieving the full objects of the tasks
        var dependenciesTasks = dal.Task.ReadAll().
            Where(t => dependencies.Any(d => d == t.Id)).OrderBy(t => t.SchedualDate).ToList();
        //Converting to a list of tasks, and calculating for each task in the list its status.
        var tasksList = dependenciesTasks.ConvertList<DO.Task, TaskInList>();
        tasksList.ForEach(d => d.Status = GetTaskStatus(dependenciesTasks, d.Id));
        return tasksList;
    }
}