namespace CodeLab.Domain.Submissions;

public record TestItem(string Name, TestStatus Status, string? Message);

public enum TestStatus
{
    Pass,
    Fail,
    Error
}