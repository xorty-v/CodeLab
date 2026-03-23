using CodeLab.Domain.Abstractions.Errors;
using CSharpFunctionalExtensions;

namespace CodeLab.Domain.Submissions;

public sealed record TestResult
{
    private TestResult(TestStatus status, string? message, List<TestItem>? testItems)
    {
        Status = status;
        Message = message;
        TestItems = testItems;
    }

    // EF Core
    private TestResult() { }

    public TestStatus Status { get; }
    public string? Message { get; }
    public List<TestItem>? TestItems { get; }

    public static Result<TestResult, Error> Create(List<TestItem>? testItems, string? message = null)
    {
        if (!string.IsNullOrWhiteSpace(message))
            return new TestResult(TestStatus.Error, message, []);

        if (testItems is not { Count: > 0 })
            return GeneralErrors.ValueIsRequired("testItems");

        var status = testItems.Select(t => t.Status).Max();

        return new TestResult(status, null, testItems);
    }
}