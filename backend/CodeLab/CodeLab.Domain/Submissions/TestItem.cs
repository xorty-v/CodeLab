using CodeLab.Domain.Abstractions.Errors;
using CSharpFunctionalExtensions;

namespace CodeLab.Domain.Submissions;

public sealed record TestItem
{
    private TestItem(string name, TestStatus status, string? output)
    {
        Name = name;
        Status = status;
        Output = output;
    }

    public string Name { get; }
    public TestStatus Status { get; }
    public string? Output { get; }

    public static Result<TestItem, Error> Create(string name, TestStatus status, string? output)
    {
        if (string.IsNullOrWhiteSpace(name))
            return GeneralErrors.ValueIsRequired("testItem");

        return new TestItem(name, status, output);
    }
}