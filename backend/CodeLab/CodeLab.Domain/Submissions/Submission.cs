using CodeLab.Domain.Abstractions.Errors;
using CSharpFunctionalExtensions;

namespace CodeLab.Domain.Submissions;

public sealed class Submission
{
    public Submission(Guid? id, Guid exerciseId, string sourceCode)
    {
        Id = id ?? Guid.NewGuid();
        ExerciseId = exerciseId;
        SourceCode = sourceCode;
        Status = SubmissionStatus.Pending;
        SubmittedAt = DateTime.UtcNow;
    }

    // EF Core
    private Submission()
    {
    }

    public Guid Id { get; private set; }

    public Guid ExerciseId { get; private set; }

    public string SourceCode { get; private set; }

    public SubmissionStatus Status { get; private set; }

    public List<TestItem>? TestResults { get; private set; } = new();

    public DateTime SubmittedAt { get; private set; }

    public void MarkAsProcessing() => Status = SubmissionStatus.Processing;

    public UnitResult<Error> MarkCompleted(List<TestItem> testItems)
    {
        if (Status != SubmissionStatus.Processing)
            return GeneralErrors.ValueIsInvalid("Submission is not in processing state");

        if (!testItems.Any())
            return GeneralErrors.ValueIsInvalid("Submission must contain test items");

        TestResults.Clear();
        TestResults.AddRange(testItems);

        if (testItems.Any(r => r.Status == TestStatus.Error))
        {
            Status = SubmissionStatus.Error;
        }
        else
        {
            Status = testItems.All(r => r.Status == TestStatus.Pass)
                ? SubmissionStatus.Succeeded
                : SubmissionStatus.Failed;
        }

        return UnitResult.Success<Error>();
    }
}