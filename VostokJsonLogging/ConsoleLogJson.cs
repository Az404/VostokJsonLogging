using JetBrains.Annotations;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Abstractions.Wrappers;
using Vostok.Logging.Console;
using Vostok.Logging.Formatting;
using VostokJsonLogging.Helpers;

namespace VostokJsonLogging;

/// <summary>
/// <para>A log which outputs events to console in JSON-serialized form.</para>
/// <para>Based on <see cref="ConsoleLog"/>.</para>
/// </summary>
[PublicAPI]
public class ConsoleLogJson : ILog
{
    private readonly ConsoleLog baseLog;

    public ConsoleLogJson()
    {
        baseLog = new ConsoleLog(new ConsoleLogSettings
        {
            OutputTemplate = OutputTemplate
                .Create()
                .AddMessage()
                .AddNewline()
                .Build()
        });
    }

    public void Log(LogEvent @event)
    {
        baseLog.Log(new LogEvent(
            @event.Level,
            @event.Timestamp,
            "{event}",
            new Dictionary<string, object>
            {
                ["event"] = new LogEventJsonWrapper(@event)
            },
            null));
    }

    public bool IsEnabledFor(LogLevel level)
    {
        return baseLog.IsEnabledFor(level);
    }

    public ILog ForContext(string context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        return new SourceContextWrapper(this, context);
    }

    private class LogEventJsonWrapper
    {
        private readonly LogEvent logEvent;

        public LogEventJsonWrapper(LogEvent logEvent)
        {
            this.logEvent = logEvent;
        }

        public override string ToString() => LogEventJsonSerializer.ToJson(logEvent);
    }
}