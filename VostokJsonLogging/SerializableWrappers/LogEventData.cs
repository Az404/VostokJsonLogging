using Vostok.Commons.Formatting;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Abstractions.Values;
using Vostok.Logging.Formatting;

namespace VostokJsonLogging.SerializableWrappers;

/// <summary>
/// Helper DTO for (de)serialization of <see cref="LogEvent"/> class.
/// </summary>
internal class LogEventData
{
    public LogLevel Level { get; init; }

    public DateTimeOffset Timestamp { get; init; }

    public string? Message { get; init; }

    public IReadOnlyDictionary<string, string>? Properties { get; init; }

    public Exception? Exception { get; init; }

    public static LogEventData FromLogEvent(LogEvent logEvent)
    {
        return new LogEventData
        {
            Level = logEvent.Level,
            Timestamp = logEvent.Timestamp,
            Message = LogMessageFormatter.Format(logEvent),
            Properties = RenderProperties(logEvent),
            Exception = logEvent.Exception
        };
    }

    public LogEvent ToLogEvent()
    {
        return new LogEvent(
            Level,
            Timestamp,
            Message != null
                ? MessageTemplateEscaper.Escape(Message)
                : null,
            Properties
                ?.ToDictionary(
                    kv => kv.Key,
                    kv => (object)kv.Value),
            Exception);
    }

    private static IReadOnlyDictionary<string, string>? RenderProperties(LogEvent logEvent)
    {
        return logEvent.Properties
            ?.ToDictionary(
                kv => kv.Key,
                kv => RenderPropertyValue(logEvent, kv.Key, kv.Value));
    }

    private static string RenderPropertyValue(LogEvent logEvent, string key, object value)
    {
        // reference code: https://github.com/vostok/logging.hercules/blob/43a2363a1ccf92ea52dc555eb28792b1fe8e0c87/Vostok.Logging.Hercules/HerculesTagsBuilderExtensions.cs#L24

        if (key == WellKnownProperties.OperationContext && value is OperationContextValue operationContextValue)
            value = operationContextValue.Select(t => OperationContextValueFormatter.Format(logEvent, t)).ToArray();

        var format = value is DateTime or DateTimeOffset ? "O" : null;

        return ObjectValueFormatter.Format(value, format);
    }
}