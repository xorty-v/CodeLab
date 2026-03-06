namespace CodeLab.Domain.Exercises;

public sealed class Exercise
{
    public Exercise(Guid? id, string title, string description, Slug slug, DateOnly assignedDate)
    {
        Id = id ?? Guid.NewGuid();
        Title = title;
        Description = description;
        Slug = slug;
        AssignedDate = assignedDate;
        CreatedAt = DateTime.UtcNow;
    }

    // EF Core
    private Exercise()
    {
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; }

    public string Description { get; private set; }

    public Slug Slug { get; private set; }

    public DateOnly AssignedDate { get; private set; }

    public DateTime CreatedAt { get; private set; }
}