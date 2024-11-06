namespace BO;

public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base($"{message} dosn't exsit.") { }
    public BlDoesNotExistException(Exception innerException) : base(innerException.Message, innerException) { }
}

public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(Exception innerException) : base(innerException.Message, innerException) { }
}

public class BlXMLFileLoadCreateException : Exception
{
    public BlXMLFileLoadCreateException(string? message, Exception innerException) : base(message, innerException) { }
}

public class BlInvalidException : Exception
{
    public string InvalidPropertyName { get; private set; }
    public BlInvalidException(string invalidPropertyName) : base($"Invalid property: {invalidPropertyName}") { this.InvalidPropertyName = invalidPropertyName; }
}

public class BlNullException : Exception
{
    public string NullPropertyName { get; private set; }
    public BlNullException(string nullPropertyName) : base($"Null property: {nullPropertyName}") { this.NullPropertyName = nullPropertyName; }
}

public class BlIllegalException : Exception
{
    public string PropertyName { get; private set; }
    public string FuncName { get; private set; }
    public string? Details {  get; private set; }
    public BlIllegalException(string name, string func, string? details = null) : base($"Invalid {func} {name}.")
    {
        PropertyName = name; FuncName = func; Details = details;
    }
}
public class BlDeletionImpossible : Exception
{
    public BlDeletionImpossible(string? message) : base(message) { }
    public BlDeletionImpossible(string massage, Exception innerException) : base(massage, innerException) { }
}