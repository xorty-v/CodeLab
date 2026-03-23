using CodeLab.Domain.Abstractions.Errors;
using CSharpFunctionalExtensions;

namespace CodeLab.Core.Features.Submissions;

public interface ISubmissionProcessingService
{
    Task<UnitResult<Error>> ProcessSubmissionAsync(
        Guid submissionId,
        string slug,
        CancellationToken cancellationToken = default);
}