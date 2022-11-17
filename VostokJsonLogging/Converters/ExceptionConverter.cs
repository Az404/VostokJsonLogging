using System.Text.Json;
using System.Text.Json.Serialization;
using VostokJsonLogging.SerializableWrappers;

namespace VostokJsonLogging.Converters;

internal class ExceptionConverter : JsonConverter<Exception>
{
    public override Exception? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<ExceptionData>(ref reader, options)?.ToException();
    }

    public override void Write(Utf8JsonWriter writer, Exception value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, ExceptionData.FromException(value), options);
    }
}