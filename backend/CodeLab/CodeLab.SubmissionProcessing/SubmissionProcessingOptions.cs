namespace CodeLab.SubmissionProcessing;

public sealed record SubmissionProcessingOptions
{
    public const string SECTION_NAME = "SubmissionProcessing";

    public string WorkspacePath { get; init; }

    public string DockerPath { get; init; } = "docker";

    public string RunnerImage { get; init; } = "codelab/csharp-runner:1.0";

    public string MemoryLimit { get; init; } = "256m";

    public double CpuLimit { get; init; } = 0.5;

    public int ExecutionTimeoutSeconds { get; init; } = 30;
}