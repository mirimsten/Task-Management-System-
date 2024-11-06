namespace BlApi;
public interface IMilestone
{
    public bool ScheduleExists { get;}
    public void CreateSchedule(DateTime start, DateTime end);
    public BO.Milestone GetMilestone(int id);
    public BO.Milestone UpdateMilestone(int id, string alias, string description, string comments);
    public IEnumerable<BO.MilestoneInList> GetAllMilestones(Func<BO.Milestone, bool>? filter = null);
}
