using CodeLab.Domain.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeLab.Infrastructure.Postgres.Configurations;

public static class ExerciseIndexes
{
    public const string ASSIGNED_DATE = "ix_exercises_assigned_date";
    public const string SLUG = "ix_exercises_slug";
}

internal sealed class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("exercises");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired();

        builder.Property(x => x.Description).IsRequired();

        builder.Property(x => x.Slug)
            .HasConversion(
                x => x.Value,
                x => Slug.Generate(x).Value)
            .HasColumnName("slug")
            .IsRequired();

        builder.HasIndex(x => x.Slug)
            .HasDatabaseName(ExerciseIndexes.SLUG)
            .IsUnique();

        builder.HasIndex(x => x.AssignedDate)
            .HasDatabaseName(ExerciseIndexes.ASSIGNED_DATE)
            .IsUnique();

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}