using CodeLab.Domain.Abstractions.Errors;
using CodeLab.Domain.Submissions;
using CSharpFunctionalExtensions;

namespace CodeLab.CodeRunner;

public interface ICodeRunnerService
{
    Task<Result<SubmissionStatus, Error>> RunCodeAsync(Guid submissionId, string slug, string sourceCode,
        CancellationToken cancellationToken = default);
}