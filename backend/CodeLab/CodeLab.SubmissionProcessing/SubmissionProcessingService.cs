using CodeLab.Core.Features.Submissions;
using CodeLab.Domain;
using CodeLab.Domain.Abstractions.Errors;
using CodeLab.SubmissionProcessing.DockerProcess;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace CodeLab.SubmissionProcessing;

internal sealed class SubmissionProcessingService : ISubmissionProcessingService
{
    private readonly IDockerProcessRunner _dockerProcessRunner;
    private readonly ISubmissionsRepository _submissionsRepository;
    private readonly IFileSystemService _fileSystemService;
    private readonly ILogger<SubmissionProcessingService> _logger;

    public SubmissionProcessingService(
        IDockerProcessRunner dockerProcessRunner,
        ISubmissionsRepository submissionsRepository,
        IFileSystemService fileSystemService,
        ILogger<SubmissionProcessingService> logger)
    {
        _dockerProcessRunner = dockerProcessRunner;
        _submissionsRepository = submissionsRepository;
        _fileSystemService = fileSystemService;
        _logger = logger;
    }

    public async Task<UnitResult<Error>> ProcessSubmissionAsync(
        Guid submissionId,
        string slug,
        CancellationToken cancellationToken = default)
    {
        var submissionResult = await _submissionsRepository.GetByAsync(s => s.Id == submissionId, cancellationToken);
        if (submissionResult.IsFailure)
            return submissionResult.Error;

        var submission = submissionResult.Value;
        string? workspacePath = null;

        try
        {
            _logger.LogInformation("Starting submission processing for SubmissionId: {SubmissionId}", submissionId);

            var workspaceResult = _fileSystemService.PrepareWorkspace(submissionId, slug, submission.SourceCode.Value);
            if (workspaceResult.IsFailure)
                return workspaceResult.Error;

            workspacePath = workspaceResult.Value;

            var dockerResult = await _dockerProcessRunner.RunSubmissionAsync(workspacePath, cancellationToken);
            if (dockerResult.IsFailure)
                return dockerResult.Error;

            submission.MarkAsCompleted(dockerResult.Value.Tests, dockerResult.Value.Message);
            await _submissionsRepository.UpdateAsync(submission, cancellationToken);

            _logger.LogInformation("Completed submission processing for SubmissionId: {SubmissionId}", submissionId);

            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error during submission {SubmissionId} processing", submissionId);
            return CodelabErrors.ProcessFailed();
        }
        finally
        {
            if (workspacePath != null)
            {
                _fileSystemService.CleanupWorkspace(workspacePath);
            }
        }
    }
}