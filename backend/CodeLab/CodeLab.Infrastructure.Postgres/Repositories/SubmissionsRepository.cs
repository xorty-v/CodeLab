using System.Linq.Expressions;
using CodeLab.Core.Features.Submissions;
using CodeLab.Domain;
using CodeLab.Domain.Abstractions.Errors;
using CodeLab.Domain.Exercises;
using CodeLab.Domain.Submissions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CodeLab.Infrastructure.Postgres.Repositories;

internal sealed class SubmissionsRepository : ISubmissionsRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<SubmissionsRepository> _logger;

    public SubmissionsRepository(AppDbContext dbContext, ILogger<SubmissionsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Submission, Error>> GetByAsync(Expression<Func<Submission, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Submission? submission = await _dbContext.Submissions.FirstOrDefaultAsync(predicate, cancellationToken);

            return submission == null
                ? GeneralErrors.NotFound()
                : Result.Success<Submission, Error>(submission);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled while getting submission");
            return CodelabErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting submission");
            return CodelabErrors.DatabaseError();
        }
    }

    public async Task<Result<Guid, Error>> AddAsync(Submission submission, CancellationToken cancellationToken)
    {
        _dbContext.Submissions.Add(submission);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return submission.Id;
    }

    public async Task UpdateAsync(Submission submission, CancellationToken cancellationToken)
    {
        _dbContext.Submissions.Update(submission);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}