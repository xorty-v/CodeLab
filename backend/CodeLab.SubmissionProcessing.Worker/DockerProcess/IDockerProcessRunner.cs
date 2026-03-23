using CodeLab.Domain.Abstractions.Errors;
using CSharpFunctionalExtensions;

namespace CodeLab.SubmissionProcessing.Worker.DockerProcess;

public interface IDockerProcessRunner
{
    Task<Result<ContainerResponse, Error>> RunSubmissionAsync(string submissionPath, CancellationToken ct = default);
}