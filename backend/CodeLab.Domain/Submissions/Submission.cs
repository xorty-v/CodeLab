using CodeLab.Domain.Abstractions.Errors;
using CSharpFunctionalExtensions;

namespace CodeLab.Domain.Submissions;

public sealed class Submission
{
    public Submission(Guid exerciseId, SourceCode sourceCode)
    {
        Id = Guid.NewGuid();
        ExerciseId = exerciseId;
        SourceCode = sourceCode;
        Status = SubmissionStatus.Pending;
        SubmittedAt = DateTime.UtcNow;
    }

    // EF Core
    private Submission() { }

    public Guid Id { get; private set; }

    public Guid ExerciseId { get; private set; }

    public SourceCode SourceCode { get; private set; }

    public SubmissionStatus Status { get; private set; }

    public TestResult? TestResult { get; private set; }

    public DateTime SubmittedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public UnitResult<Error> MarkAsProcessing()
    {
        if (Status != SubmissionStatus.Pending)
            return GeneralErrors.Failure("Submission already started");

        Status = SubmissionStatus.Processing;

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> MarkAsCompleted(List<TestItem>? tests, string? message = null)
    {
        if (Status != SubmissionStatus.Processing)
            return GeneralErrors.Failure("Submission is not processing");

        var result = TestResult.Create(tests, message);
        if (result.IsFailure)
            return result.Error;

        TestResult = result.Value;
        Status = SubmissionStatus.Completed;
        CompletedAt = DateTime.UtcNow;

        return UnitResult.Success<Error>();
    }
}