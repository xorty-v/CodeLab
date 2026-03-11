using CodeLab.Domain.Abstractions.Errors;
using CSharpFunctionalExtensions;

namespace CodeLab.SubmissionProcessing.DockerProcess;

public interface IDockerProcessRunner
{
    Task<Result<ContainerResponse, Error>> RunSubmissionAsync(string submissionPath, CancellationToken ct = default);
}