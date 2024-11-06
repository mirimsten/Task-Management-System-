namespace DO;

/// <summary>
/// The task entity contains all the information needed to perform the task.
/// </summary>
/// <param name="id">A unique ID number for the task</param>
/// <param name="description">Description of the task</param>
/// <param name="alias">Another alias for the task</param>
/// <param name="mileStone">Milestones in the mission</param>
/// <param name="CreatedAtDate">Task creation date</param>
/// <param name="start">Date of starting work on the task</param>
/// <param name="schedual">Planned date for the start of work</param>
/// <param name="duration ">The amount of time required to perform the task</param>
/// <param name="deadline">Last possible end date</param>
/// <param name="complete">Actual end date</param>
/// <param name="deliverables">product</param>
/// <param name="remarks">Notes</param>
/// <param name="engineerid">The ID of the engineer assigned to the task</param>
/// <param name="ComplexityLevel">The difficulty level of the task</param>

public record Task
(
    int Id,
    string Description,
    string Alias,
    bool IsMilestone,
    DateTime? CreatedAtDate,
    DateTime? StartDate,
    DateTime? SchedualDate,
    TimeSpan? Duration,
    DateTime? DeadlineDate,
    DateTime? CompleteDate,
    string? Deliverables,
    string? Remarks,
    int? EngineerId,
    EngineerExperience ComplexityLevel
)
{
    public Task() : this(0, "", "", false, null, null, null, null, null, null, "", "", 0, 0) { }  //empty ctor

    #region Ignore If Null
    //Boolean functions for all nullable properties, so that null values wont be written in the xml file.
    public bool ShouldSerializeDescription() => !string.IsNullOrEmpty(Description);
    public bool ShouldSerializeAlias() => !string.IsNullOrEmpty(Alias);
    public bool ShouldSerializeCreatedAtDate() => CreatedAtDate.HasValue;
    public bool ShouldSerializeStartDate() => StartDate.HasValue;
    public bool ShouldSerializeSchedualDate() => SchedualDate.HasValue;
    public bool ShouldSerializeCompleteDate() => CompleteDate.HasValue;
    public bool ShouldSerializeDuration() => Duration.HasValue;
    public bool ShouldSerializeDeadlineDate() => DeadlineDate.HasValue;
    public bool ShouldSerializeDeliverables() => !string.IsNullOrEmpty(Deliverables);
    public bool ShouldSerializeRemarks() => !string.IsNullOrEmpty(Remarks);   
    
    #endregion
    public override string ToString()//print the task
    {
        return
@$"
Id: {Id},    
Description: {Description ?? "-----"},
Alias: {Alias ?? "------"},
Deliverables: {Deliverables ?? "------"}
Remarks: {Remarks ?? "------"},
Mile Stone: {IsMilestone},                           
Createion time: {CreatedAtDate},
Schedual date: {SchedualDate}, 
Start date: {StartDate}, 
Duration: {Duration},
Dead line date: {DeadlineDate},
Complete at: {CompleteDate}
Engineer in charge: {EngineerId},
Complexity level: {ComplexityLevel} ";
    }
}
