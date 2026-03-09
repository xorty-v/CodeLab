using CodeLab.Domain.Exercises;
using CodeLab.Domain.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeLab.Infrastructure.Postgres.Configurations;

internal sealed class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("submissions");

        builder.HasKey(s => s.Id);

        builder.Property(x => x.SourceCode)
            .HasConversion(
                x => x.Value,
                x => SourceCode.Create(x).Value)
            .HasMaxLength(SourceCode.MAX_LENGTH)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .IsRequired();


        builder.OwnsOne(s => s.TestResult, result =>
        {
            result.ToJson("test_result");

            result.Property(r => r.Status)
                .HasConversion<string>();

            result.Property(r => r.Message);

            result.OwnsMany(r => r.TestItems, items =>
            {
                items.Property(i => i.Name).IsRequired();

                items.Property(i => i.Status)
                    .HasConversion<string>();

                items.Property(i => i.Output);
            });
        });


        builder.Property(s => s.SubmittedAt).IsRequired();

        builder.HasOne<Exercise>()
            .WithMany()
            .HasForeignKey(s => s.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}