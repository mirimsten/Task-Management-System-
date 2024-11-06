namespace DO;

/// <summary>
/// Dependency entity keeps for each task the task it relies on.
/// </summary>
/// <param name="id">Identification number for the dependency </param>
/// <param name="dependentTask"> The task number </param>
/// <param name="dependsOnTask"> The number of the task it depends on </param>
/// 

public record Dependency
(
    int Id,
    int DependentTask,
    int DependsOnTask
)
{
    public Dependency() : this(0, 0, 0) { }  //empty ctor
    public override string ToString()//print the item
    {
        return $"\nId: {Id}\nDependent Task:{DependentTask}\nDepends On Task: {DependsOnTask}";
    }
}
