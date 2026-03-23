using CodeLab.Contracts.Submissions.Messaging;
using CodeLab.Core.Features.Submissions;

namespace CodeLab.SubmissionProcessing.Worker;

public sealed class SubmissionCreatedHandler
{
    private readonly ISubmissionsRepository _submissionsRepository;
    private readonly ISubmissionProcessingService _submissionProcessingService;
    private readonly ILogger<SubmissionCreatedHandler> _logger;

    public SubmissionCreatedHandler(
        ISubmissionsRepository submissionsRepository,
        ISubmissionProcessingService submissionProcessingService,
        ILogger<SubmissionCreatedHandler> logger)
    {
        _submissionsRepository = submissionsRepository;
        _submissionProcessingService = submissionProcessingService;
        _logger = logger;
    }

    public async Task Handle(SubmissionCreated message, CancellationToken ct)
    {
        var submissionResult = await _submissionsRepository.GetByAsync(s => s.Id == message.SubmissionId, ct);

        if (submissionResult.IsFailure)
        {
            _logger.LogWarning("Submission {SubmissionId} not found", message.SubmissionId);
            return;
        }

        var submission = submissionResult.Value;

        var markProcessingResult = submission.MarkAsProcessing();
        if (markProcessingResult.IsFailure)
        {
            _logger.LogWarning(
                "Submission {SubmissionId} cannot start processing. Status: {Status}", submission.Id,
                submission.Status);
            return;
        }

        await _submissionsRepository.UpdateAsync(submission, ct);

        var processingResult = await _submissionProcessingService.ProcessSubmissionAsync(
            message.SubmissionId,
            message.Slug,
            ct);

        if (processingResult.IsFailure)
        {
            _logger.LogError(
                "Processing failed for submission {SubmissionId}: {Error}", message.SubmissionId,
                processingResult.Error);
            return;
        }

        _logger.LogInformation("Submission {SubmissionId} processed successfully", message.SubmissionId);
    }
}