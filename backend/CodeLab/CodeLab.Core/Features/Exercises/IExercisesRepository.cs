using CodeLab.Domain.Abstractions.Errors;
using CodeLab.Domain.Exercises;
using CSharpFunctionalExtensions;

namespace CodeLab.Core.Features.Exercises;

public interface IExercisesRepository
{
    Task<IReadOnlyList<Exercise>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Result<Exercise, Error>> GetBySlugAsync(Slug slug, CancellationToken cancellationToken = default);
}