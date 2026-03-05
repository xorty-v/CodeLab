using CodeLab.Domain.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeLab.Infrastructure.Postgres.Configurations;

internal sealed class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("exercises");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired();

        builder.Property(x => x.Description).IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property(x => x.Slug)
            .HasConversion(
                x => x.Value,
                x => Slug.Generate(x).Value)
            .HasColumnName("slug")
            .IsRequired();
    }
}