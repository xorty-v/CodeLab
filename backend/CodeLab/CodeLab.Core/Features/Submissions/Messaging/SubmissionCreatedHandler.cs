using CodeLab.CodeRunner;
using Microsoft.Extensions.Logging;

namespace CodeLab.Core.Features.Submissions.Messaging;

public record SubmissionCreated(Guid SubmissionId, string Slug);

public sealed class SubmissionCreatedHandler
{
    private readonly ISubmissionsRepository _submissionsRepository;
    private readonly ICodeRunnerService _codeRunnerService;
    private readonly ILogger<SubmissionCreatedHandler> _logger;

    public SubmissionCreatedHandler(
        ISubmissionsRepository submissionsRepository,
        ICodeRunnerService codeRunnerService,
        ILogger<SubmissionCreatedHandler> logger)
    {
        _submissionsRepository = submissionsRepository;
        _codeRunnerService = codeRunnerService;
        _logger = logger;
    }

    public async Task Handle(SubmissionCreated message, CancellationToken cancellationToken)
    {
        var submissionResult = await _submissionsRepository.GetByAsync(s => s.Id == message.SubmissionId,
            cancellationToken);

        if (submissionResult.IsFailure)
        {
            _logger.LogWarning("Submission {SubmissionId} not found", message.SubmissionId);
            return;
        }

        var submission = submissionResult.Value;

        submission.MarkAsProcessing();
        await _submissionsRepository.UpdateAsync(submission, cancellationToken);

        var result = await _codeRunnerService.RunCodeAsync(message.Slug, submission.SourceCode, cancellationToken);

        submission.MarkCompleted(result.Value.Tests.ToList());
        await _submissionsRepository.UpdateAsync(submission, cancellationToken);

        _logger.LogInformation("Submission {Id} finished with status {Status}", submission.Id, submission.Status);
    }
}