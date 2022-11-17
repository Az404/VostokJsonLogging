using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Abstractions;
using VostokJsonLogging.Helpers;

namespace VostokJsonLogging.Tests;

public class LogEventJsonSerializerTests
{
    [Test]
    public void Should_correctly_serialize_and_deserialize()
    {
        var sourceEvent = new LogEvent(
            LogLevel.Info,
            DateTimeOffset.Parse("2022-10-19T16:37:59.7642418+05:00"),
            "msg",
            new Dictionary<string, object>
            {
                ["key"] = "value"
            },
            new Exception("Some error")
        );

        var json = LogEventJsonSerializer.ToJson(sourceEvent);
        Console.WriteLine(json);

        var deserializedEvent = LogEventJsonSerializer.FromJson(json);

        deserializedEvent.Should()
            .BeEquivalentTo(sourceEvent, options =>
                options.Using<Exception>(ctx => ctx.Subject.Message.Should().Contain(ctx.Expectation.Message))
                    .WhenTypeIs<Exception>());
    }

    [Test]
    public void Should_serialize_complex_object_in_properties_as_toString()
    {
        var sourceEvent = new LogEvent(
            LogLevel.Info,
            DateTimeOffset.Parse("2022-10-19T16:37:59.7642418+05:00"),
            "msg",
            new Dictionary<string, object>
            {
                ["key"] = new MyClass { Value = 123 }
            },
            null
        );
        
        var json = LogEventJsonSerializer.ToJson(sourceEvent);
        Console.WriteLine(json);

        var deserializedEvent = LogEventJsonSerializer.FromJson(json);

        deserializedEvent!.Properties!["key"].Should().Be("123");
    }

    [Test]
    public void Should_render_messageTemplate_when_serialize()
    {
        var sourceEvent = new LogEvent(
            LogLevel.Info,
            DateTimeOffset.Parse("2022-10-19T16:37:59.7642418+05:00"),
            "{key}",
            new Dictionary<string, object>
            {
                ["key"] = 123
            },
            null
        );
        
        var json = LogEventJsonSerializer.ToJson(sourceEvent);
        Console.WriteLine(json);

        var deserializedEvent = LogEventJsonSerializer.FromJson(json);

        deserializedEvent!.MessageTemplate.Should().Be("123");
    }

    [Test]
    public void Should_escape_deserialized_messageTemplate()
    {
        var sourceEvent = new LogEvent(
            LogLevel.Info,
            DateTimeOffset.Parse("2022-10-19T16:37:59.7642418+05:00"),
            "{key1}",
            new Dictionary<string, object>
            {
                ["key1"] = "{key2}",
                ["key2"] = "123"
            },
            null
        );
        
        var json = LogEventJsonSerializer.ToJson(sourceEvent);
        Console.WriteLine(json);

        var deserializedEvent = LogEventJsonSerializer.FromJson(json);

        deserializedEvent!.MessageTemplate.Should().Be("{{key2}}");
    }

    [Test]
    public void Should_render_date_properties_as_ISO8601()
    {
        const string testDate = "2020-10-25T10:11:12.7642418+06:00";
        var sourceEvent = new LogEvent(
            LogLevel.Info,
            DateTimeOffset.Parse("2022-10-19T16:37:59.7642418+05:00"),
            "{key}",
            new Dictionary<string, object>
            {
                ["key"] = DateTimeOffset.Parse(testDate)
            },
            null
        );
        
        var json = LogEventJsonSerializer.ToJson(sourceEvent);
        Console.WriteLine(json);

        var deserializedEvent = LogEventJsonSerializer.FromJson(json);

        deserializedEvent!.Properties!["key"].Should().Be(testDate);
    }

    private class MyClass
    {
        public int Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}