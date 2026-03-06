using System.Linq.Expressions;
using CodeLab.Core.Features.Exercises;
using CodeLab.Domain;
using CodeLab.Domain.Abstractions.Errors;
using CodeLab.Domain.Exercises;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CodeLab.Infrastructure.Postgres.Repositories;

internal sealed class ExercisesRepository : IExercisesRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ExercisesRepository> _logger;

    public ExercisesRepository(ApplicationDbContext dbContext, ILogger<ExercisesRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Exercise, Error>> GetByAsync(Expression<Func<Exercise, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Exercise? exercise = await _dbContext.Exercises.FirstOrDefaultAsync(predicate, cancellationToken);

            return exercise == null
                ? GeneralErrors.NotFound()
                : Result.Success<Exercise, Error>(exercise);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled while getting exercise");
            return CodelabErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting exercise");
            return CodelabErrors.DatabaseError();
        }
    }
}