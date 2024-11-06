namespace DO;


/// <summary>
/// The engineer entity contains the engineers' personal details and their employment details.
/// </summary>
/// <param name="id">Unique ID number of the engineer in the company.</param>
/// <param name="name">The engineer name</param>
/// <param name="email"> The engineer email</param>
/// <param name="Level">The experience and rank of the engineer</param>
/// <param name="cost">hourly wage</param>
/// 

public record Engineer
(
    int Id,
    string? Name,
    string? Email,
    EngineerExperience Level,
    double? Cost
)
{
    public Engineer() : this(0, "", "", 0, 0.0) { }  //empty ctor

    #region Ignore If Null
    //Boolean functions for all nullable properties, so that null values wont be written in the xml file.
    public bool ShouldSerializeName() => !string.IsNullOrEmpty(Name);
    public bool ShouldSerializeEmail() => !string.IsNullOrEmpty(Email);
    public bool ShouldSerializeCost() => Cost.HasValue;


    #endregion
    public override string ToString()
    {
        return $"\nId: {Id},\nName: {Name},\nEmail: {Email},\nLevel: {Level},\nCost: {Cost}\n";       
    }

}
