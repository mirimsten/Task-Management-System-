using BO;

namespace BlApi;

public interface IEngineer
{
    public IEnumerable<EngineerInList> ReadAllEngineers(Func<DO.Engineer, bool>? filter = null );
    public Engineer GetEngineer(int id);
    public int CreateEngineer(Engineer engineer);
    public void DeleteEngineer(int id);
    public void UpdateEngineer(Engineer engineer);
}
