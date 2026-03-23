using CodeLab.Domain.Submissions;

namespace CodeLab.Contracts.Submissions;

public record SubmissionResponse(
    Guid Id,
    SubmissionStatus Status,
    TestResultDto? Result);

public record TestResultDto(
    TestStatus TestStatus,
    string? Message,
    List<TestItem> Tests
);