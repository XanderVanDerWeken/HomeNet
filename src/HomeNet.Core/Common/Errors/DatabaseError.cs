namespace HomeNet.Core.Common.Errors;

public sealed class DatabaseError : Error
{
    public string TableName { get; }
    public Exception Exception { get; }

    public DatabaseError(string tableName, Exception exception) 
        : base(ErrorCodes.Database, $"A database error occurred while accessing the '{tableName}'.")
    {
        TableName = tableName;
        Exception = exception;
    }
}
