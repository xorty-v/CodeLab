using CodeLab.Domain.Submissions;

namespace CodeLab.Contracts.Submissions;

public sealed record SubmissionResponse(Guid Id, SubmissionStatus Status, IReadOnlyCollection<TestItem>? Tests);