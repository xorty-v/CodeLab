using CodeLab.Core.Features.Submissions;
using CodeLab.Domain.Abstractions.Errors;
using CodeLab.Domain.Exercises;
using CodeLab.Domain.Submissions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace CodeLab.Infrastructure.Postgres.Repositories;

internal sealed class SubmissionsRepository : ISubmissionsRepository
{
    private readonly AppDbContext _dbContext;

    public SubmissionsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Submission, Error>> GetById(Guid submissionId, CancellationToken cancellationToken)
    {
        Submission? submission = await _dbContext.Submissions
            .FirstOrDefaultAsync(x => x.Id == submissionId, cancellationToken);

        return submission == null
            ? GeneralErrors.NotFound()
            : Result.Success<Submission, Error>(submission);
    }

    public async Task AddAsync(Submission submission, CancellationToken cancellationToken)
    {
        _dbContext.Submissions.Add(submission);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Submission submission, CancellationToken cancellationToken)
    {
        _dbContext.Submissions.Update(submission);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}