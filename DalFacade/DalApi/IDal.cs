using DO;
namespace DalApi;
public interface IDal
{
    IDependency Dependency { get; }
    IEngineer Engineer { get; }
    ITask Task { get; }
    DateTime? StartDate { get; set; }
    DateTime? EndDate { get; set; }
    ProjectStatus? ProjectStatus { get; set; }
}

