using System.Text.Json;
using System.Text.Json.Serialization;
using Vostok.Logging.Abstractions;
using VostokJsonLogging.SerializableWrappers;

namespace VostokJsonLogging.Converters;

internal class LogEventConverter : JsonConverter<LogEvent>
{
    public override LogEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<LogEventData>(ref reader, options)?.ToLogEvent();
    }

    public override void Write(Utf8JsonWriter writer, LogEvent value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, LogEventData.FromLogEvent(value), options);
    }
}