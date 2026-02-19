using CodeLab.Domain.Abstractions.Errors;
using CodeLab.Domain.Submissions;
using CSharpFunctionalExtensions;

namespace CodeLab.Core.Features.Submissions;

public interface ISubmissionsRepository
{
    Task<Result<Submission, Error>> GetById(Guid submissionId, CancellationToken cancellationToken);

    Task AddAsync(Submission submission, CancellationToken cancellationToken);

    Task UpdateAsync(Submission submission, CancellationToken cancellationToken);
}