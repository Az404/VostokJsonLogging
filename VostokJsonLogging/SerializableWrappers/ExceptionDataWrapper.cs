namespace VostokJsonLogging.SerializableWrappers;

/// <summary>
/// <para>A class that partially imitates original serialized <see cref="Exception"/> after deserialization.</para>
/// <list type="bullet">
///     <item>Instance of this class can be stored in the field of <see cref="Exception"/> type.</item>
///     <item>Serialized representation of an instance of this class is equal to serialized representation of original <see cref="Exception"/>.</item>
/// </list>
/// </summary>
public class ExceptionDataWrapper : Exception
{
    internal ExceptionDataWrapper(ExceptionData exceptionData)
    {
        ExceptionData = exceptionData;
    }

    internal ExceptionData ExceptionData { get; }

    public override string Message => $"[{ExceptionData.Type}] {ExceptionData.Message}";
    public override string? StackTrace => ExceptionData.StackTrace;
}