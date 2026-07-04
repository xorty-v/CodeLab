namespace CodeLab.Infrastructure.FileSystem;

public sealed record FileSystemOptions
{
    public const string SECTION_NAME = "FileSystem";

    public string ExercisesRootPath { get; init; } = string.Empty;
    public string WorkspacesRootPath { get; init; } = string.Empty;
}