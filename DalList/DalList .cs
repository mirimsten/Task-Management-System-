namespace Dal;
using DalApi;
using DO;

sealed internal class DalList : IDal
{
    public static IDal Instance { get; } = new DalList();
    private DalList() { }
    public IDependency Dependency =>  new DependencyImplementation();
    public IEngineer Engineer =>  new EngineerImplementation();
    public ITask Task =>  new TaskImplementation();
    public DateTime? StartDate { get => DataSource.Config.StartDate; set => DataSource.Config.StartDate = value; }
    public DateTime? EndDate { get => DataSource.Config.EndDate; set => DataSource.Config.EndDate = value; }
    public ProjectStatus? ProjectStatus { get => DataSource.Config.ProjectStatus; set => DataSource.Config.ProjectStatus = value; }
}
