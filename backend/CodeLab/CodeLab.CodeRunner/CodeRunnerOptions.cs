namespace CodeLab.CodeRunner;

public sealed record CodeRunnerOptions
{
    public const string SECTION_NAME = "CodeRunnerOptions";

    public string ExercisePath { get; init; } = "exercises";
}