using System.Text.Json.Serialization;

namespace CodeLab.Domain.Submissions;

public record TestItem(string Name, TestStatus Status, string? Message);

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TestStatus
{
    Pass,
    Fail,
    Error
}