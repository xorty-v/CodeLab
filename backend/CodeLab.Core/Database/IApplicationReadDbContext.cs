using CodeLab.Domain.Exercises;

namespace CodeLab.Core.Database;

public interface IApplicationReadDbContext
{
    IQueryable<Exercise> ExercisesQuery { get; }
}