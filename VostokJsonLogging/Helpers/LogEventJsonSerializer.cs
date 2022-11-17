using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Vostok.Logging.Abstractions;
using VostokJsonLogging.Converters;

namespace VostokJsonLogging.Helpers;

/// <summary>
/// <see cref="LogEvent"/> serialization helper with preconfigured options.
/// </summary>
public static class LogEventJsonSerializer
{
    private static readonly JsonSerializerOptions? SerializerOptions = new()
    {
        Converters = { new LogEventConverter(), new ExceptionConverter(), new JsonStringEnumConverter() },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping //NOTE: changes unicode chars to escape chars
    };

    /// <summary>
    /// Serialize <see cref="LogEvent"/> to json string.
    /// </summary>
    public static string ToJson(LogEvent logEvent) => JsonSerializer.Serialize(logEvent, SerializerOptions);

    /// <summary>
    /// Deserialize <see cref="LogEvent"/> from json string.
    /// </summary>
    public static LogEvent? FromJson(string serialized) => JsonSerializer.Deserialize<LogEvent>(serialized, SerializerOptions);
}