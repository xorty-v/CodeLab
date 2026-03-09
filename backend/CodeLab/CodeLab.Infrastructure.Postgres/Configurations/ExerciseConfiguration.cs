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

        builder.Property(x => x.Title)
            .HasConversion(
                v => v.Value,
                v => Title.Create(v).Value)
            .HasMaxLength(Title.MAX_LENGTH)
            .IsRequired();

        builder.Property(x => x.MarkdownContent)
            .HasConversion(
                x => x.Value,
                x => MarkdownContent.Create(x).Value)
            .HasMaxLength(MarkdownContent.MAX_LENGTH)
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasConversion(
                x => x.Value,
                x => Slug.Create(x).Value)
            .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.Slug).IsUnique().HasDatabaseName(ExerciseIndexes.SLUG);
        builder.HasIndex(x => x.AssignedDate).IsUnique().HasDatabaseName(ExerciseIndexes.ASSIGNED_DATE);
    }
}