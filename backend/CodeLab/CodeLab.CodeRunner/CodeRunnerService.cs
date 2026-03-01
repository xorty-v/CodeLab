using System.Text.Json;
using System.Text.Json.Serialization;
using CodeLab.Domain.Abstractions.Errors;
using CodeLab.Domain.Submissions;
using CSharpFunctionalExtensions;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;

namespace CodeLab.CodeRunner;

internal sealed class CodeRunnerService : ICodeRunnerService
{
    private readonly string _submissionsRoot =
        Path.Combine("C:\\Users\\x1m\\RiderProjects\\CodeLab\\backend", "submissions");

    private readonly string _exercisesRoot =
        Path.Combine("C:\\Users\\x1m\\RiderProjects\\CodeLab\\backend", "exercises");

    private readonly ILogger<CodeRunnerService> _logger;

    public CodeRunnerService(ILogger<CodeRunnerService> logger)
    {
        _logger = logger;
    }

    public async Task<Result<CodeExecutionResult, Error>> RunCodeAsync(string slug, string code, CancellationToken ct)
    {
        var tempId = Guid.NewGuid();
        var tempDirectory = CreateSubmissionDirectory(tempId);
        await WriteSubmissionFiles(tempDirectory, slug, code, ct);

        var result = await RunDockerContainer(tempDirectory, ct);

        return result;
    }

    private string CreateSubmissionDirectory(Guid tempId)
    {
        var path = Path.Combine(_submissionsRoot, tempId.ToString());
        Directory.CreateDirectory(path);
        return path;
    }

    private async Task WriteSubmissionFiles(string submissionPath, string slug, string sourceCode, CancellationToken ct)
    {
        var solutionPath = Path.Combine(submissionPath, "Solution.cs");
        await File.WriteAllTextAsync(solutionPath, sourceCode, ct);

        var testSourcePath = Path.Combine(_exercisesRoot, slug, "SolutionTests.cs");
        var testDestPath = Path.Combine(submissionPath, "SolutionTests.cs");

        File.Copy(testSourcePath, testDestPath, overwrite: true);
    }

    private async Task<Result<CodeExecutionResult, Error>> RunDockerContainer(string submissionPath,
        CancellationToken ct)
    {
        var client = new DockerClientConfiguration().CreateClient();

        var container = await client.Containers.CreateContainerAsync(
            new CreateContainerParameters
            {
                Image = "codelab/csharp-runner",
                Cmd = new List<string> { "/solution", "/solution" },
                HostConfig = new HostConfig
                {
                    NetworkMode = "none",
                    Mounts = new List<Mount>
                    {
                        new() { Type = "bind", Source = submissionPath, Target = "/solution" }
                    },
                    Tmpfs = new Dictionary<string, string> { ["/tmp"] = "rw" },
                    AutoRemove = true
                }
            }, ct);

        await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters(), ct);
        await client.Containers.WaitContainerAsync(container.ID, ct);

        var resultJsonPath = Path.Combine(submissionPath, "results.json");

        var json = await File.ReadAllTextAsync(resultJsonPath, ct);
        var options = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } };
        var testResult = JsonSerializer.Deserialize<CodeExecutionResult>(json, options);

        return testResult;
    }
}

public record CodeExecutionResult(TestStatus Status, IEnumerable<TestItem> Tests);