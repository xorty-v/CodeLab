using CodeLab.Core.Features.Exercises;
using CodeLab.Domain.Abstractions.Errors;
using CodeLab.Domain.Exercises;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace CodeLab.Infrastructure.Postgres.Repositories;

internal sealed class ExercisesRepository : IExercisesRepository
{
    private readonly AppDbContext _dbContext;

    public ExercisesRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Exercise>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Exercises
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<Exercise, Error>> GetBySlugAsync(
        Slug slug,
        CancellationToken cancellationToken = default)
    {
        Exercise? exercise = await _dbContext.Exercises.FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);

        return exercise == null
            ? GeneralErrors.NotFound()
            : Result.Success<Exercise, Error>(exercise);
    }
}