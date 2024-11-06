using DalApi;
using DO;

namespace Dal;

//stage 3
sealed internal class DalXml : IDal 
{
    public static IDal Instance { get; } = new DalXml();
    private DalXml() { }

    public IDependency Dependency => new DependencyImplementation();
    public IEngineer Engineer => new EngineerImplementation();
    public ITask Task => new TaskImplementation();
    public DateTime? StartDate { get => Config.StartDate; set => Config.StartDate = value; }
    public DateTime? EndDate { get => Config.EndDate; set => Config.EndDate = value; }
    public ProjectStatus? ProjectStatus { get=>Config.ProjectStatus; set=>Config.ProjectStatus = value; }

}

