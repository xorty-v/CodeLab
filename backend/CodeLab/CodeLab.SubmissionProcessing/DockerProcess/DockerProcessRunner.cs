using CodeLab.Domain.Abstractions.Errors;
using CodeLab.SubmissionProcessing.ProcessExecutor;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;

namespace CodeLab.SubmissionProcessing.DockerProcess;

internal sealed class DockerProcessRunner : IDockerProcessRunner
{
    private readonly IProcessRunner _processRunner;
    private readonly SubmissionProcessingOptions _options;

    public DockerProcessRunner(IProcessRunner processRunner, IOptions<SubmissionProcessingOptions> options)
    {
        _processRunner = processRunner;
        _options = options.Value;
    }

    public async Task<Result<ContainerResponse, Error>> RunSubmissionAsync(
        string submissionPath,
        CancellationToken ct = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(_options.ExecutionTimeoutSeconds));

        string arguments = BuildDockerRunArguments(submissionPath);
        var command = new ProcessCommand(_options.DockerPath, arguments);

        try
        {
            var processResult = await _processRunner.RunAsync(command, cancellationToken: cts.Token);
            if (processResult.IsFailure)
                return processResult.Error;

            return DockerOutputParser.Parse(processResult.Value.StandardOutput);
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested && !ct.IsCancellationRequested)
        {
            return GeneralErrors.OperationCancelled();
        }
    }

    private string BuildDockerRunArguments(string submissionPath)
    {
        var args = new List<string>
        {
            "run",
            "--rm",
            "--network none",
            $"--mount type=bind,source=\"{submissionPath}\",target=/solution",
            "--tmpfs /tmp:rw",
            _options.RunnerImage,
            "/solution"
        };

        return string.Join(" ", args);
    }
}