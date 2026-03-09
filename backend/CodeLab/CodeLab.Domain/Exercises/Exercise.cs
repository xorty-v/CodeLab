namespace CodeLab.Domain.Exercises;

public sealed class Exercise
{
    public Exercise(Title title, MarkdownContent markdownContent, Slug slug, DateOnly assignedDate)
    {
        Id = Guid.NewGuid();
        Title = title;
        MarkdownContent = markdownContent;
        Slug = slug;
        AssignedDate = assignedDate;
        CreatedAt = DateTime.UtcNow;
    }

    // EF Core
    private Exercise() { }

    public Guid Id { get; private set; }

    public Title Title { get; private set; }

    public MarkdownContent MarkdownContent { get; private set; }

    public Slug Slug { get; private set; }

    public DateOnly AssignedDate { get; private set; }

    public DateTime CreatedAt { get; private set; }
}