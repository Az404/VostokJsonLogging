namespace VostokJsonLogging.SerializableWrappers;

/// <summary>
/// Helper DTO for (de)serialization of <see cref="Exception"/> class.
/// </summary>
internal class ExceptionData
{
    public string? Type { get; init; }
    public string? Message { get; init; }
    public string? StackTrace { get; init; }
    public Exception? InnerException { get; init; }

    public static ExceptionData FromException(Exception error)
    {
        if (error is ExceptionDataWrapper exceptionDataWrapper)
            return exceptionDataWrapper.ExceptionData;
        
        return new ExceptionData
        {
            Type = error.GetType().ToString(),
            Message = error.Message,
            StackTrace = error.StackTrace,
            InnerException = error.InnerException
        };
    }

    public ExceptionDataWrapper ToException() => new(this);
}