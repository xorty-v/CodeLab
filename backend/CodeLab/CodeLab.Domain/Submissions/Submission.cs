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

    public DateTime SubmittedAt { get; private set; }
}