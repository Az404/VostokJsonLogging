using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using NUnit.Framework;
using VostokJsonLogging.Converters;

namespace VostokJsonLogging.Tests;

public class ExceptionConverterTests
{
    private static readonly JsonSerializerOptions Options = new() { Converters = { new ExceptionConverter() } };

    [Test]
    public void Should_correctly_serialize_without_innerException()
    {
        var exception = GetAsThrowed(new FormatException("A"));

        var json = JsonSerializer.Serialize(exception, Options);
        Console.WriteLine(json);

        var jsonNode = JsonNode.Parse(json)!;
        jsonNode["Type"]!.GetValue<string>().Should().Be("System.FormatException");
        jsonNode["Message"]!.GetValue<string>().Should().Be("A");
        jsonNode["StackTrace"]!.GetValue<string>().Should().NotBeNull();
        jsonNode["InnerException"].Should().BeNull();
    }

    [Test]
    public void Should_correctly_serialize_with_innerException()
    {
        var exception = GetAsThrowed(
            new AggregateException("A", 
                GetAsThrowed(new FormatException("B"))));

        var json = JsonSerializer.Serialize(exception, Options);
        Console.WriteLine(json);

        var jsonNode = JsonNode.Parse(json)!;
        jsonNode["InnerException"]!["Message"]!.GetValue<string>().Should().Be("B");
    }

    [Test]
    public void Should_correctly_serialize_twice()
    {
        var exception = GetAsThrowed(new FormatException("A"));

        var json1 = JsonSerializer.Serialize(exception, Options);
        var deserialized = JsonSerializer.Deserialize<Exception>(json1, Options);
        var json2 = JsonSerializer.Serialize(deserialized, Options);

        json1.Should().Be(json2);
    }

    /// <summary>
    /// To get filled StackTrace
    /// </summary>
    private static Exception GetAsThrowed(Exception initial)
    {
        try
        {
            throw initial;
        }
        catch (Exception e)
        {
            return e;
        }
    }
}