using System.Linq.Expressions;
using CodeLab.Domain.Abstractions.Errors;
using CodeLab.Domain.Exercises;
using CSharpFunctionalExtensions;

namespace CodeLab.Core.Features.Exercises;

public interface IExercisesRepository
{
    Task<Result<Exercise, Error>> GetByAsync(Expression<Func<Exercise, bool>> predicate,
        CancellationToken cancellationToken = default);
}