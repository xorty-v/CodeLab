using System.Linq.Expressions;
using CodeLab.Domain.Abstractions.Errors;
using CodeLab.Domain.Submissions;
using CSharpFunctionalExtensions;

namespace CodeLab.Core.Features.Submissions;

public interface ISubmissionsRepository
{
    Task<Result<Submission, Error>> GetByAsync(Expression<Func<Submission, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<Result<Guid, Error>> AddAsync(Submission submission, CancellationToken cancellationToken = default);

    Task UpdateAsync(Submission submission, CancellationToken cancellationToken = default);
}