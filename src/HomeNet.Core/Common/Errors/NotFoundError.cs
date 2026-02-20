namespace HomeNet.Core.Common.Errors;

public sealed class NotFoundError : Error
{
    public string EntityName { get; }
    public object? Key { get; }

    public NotFoundError(string entityName, object? key) 
        : base(ErrorCodes.NotFound, $"The requested {entityName} with key '{key}' was not found.")
    {
        EntityName = entityName;
        Key = key;
    }
}
