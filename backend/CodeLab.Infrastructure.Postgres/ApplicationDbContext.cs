using CodeLab.Core.Database;
using CodeLab.Domain.Exercises;
using CodeLab.Domain.Submissions;
using Microsoft.EntityFrameworkCore;

namespace CodeLab.Infrastructure.Postgres;

public sealed class ApplicationDbContext : DbContext, IApplicationReadDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Exercise> Exercises => Set<Exercise>();

    public DbSet<Submission> Submissions => Set<Submission>();

    public IQueryable<Exercise> ExercisesQuery => Exercises.AsNoTracking().AsQueryable();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}