using BlApi;
using BO;

namespace BlImplementation;
internal class Bl : IBl
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void InitializeDB() => DalTest.Initialization.Do();

    public void ResetDB() => DalTest.Initialization.Reset();

    public IEngineer Engineer => new EngineerImplementation();

    public ITask Task => new TaskImplementation();

    public IMilestone Milestone => new MilestoneImplementation();

    #region Project Dates
    public DateTime? Start() => _dal.StartDate; 
    
    public DateTime? End() => _dal.EndDate;

    public BO.ProjectStatus? Status() => _dal.GetProjectStatus();

    private static DateOnly _currentDate = DateOnly.FromDateTime(DateTime.Now);

    public DateOnly CurrentDate { get => _currentDate; private set => _currentDate = value; }

    public void AddDay() => CurrentDate = CurrentDate.AddDays(1);

    public DateOnly ResetDate() => CurrentDate = DateOnly.FromDateTime(DateTime.Now);

    #endregion

}
