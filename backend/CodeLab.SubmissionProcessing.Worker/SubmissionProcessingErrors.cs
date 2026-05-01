using CodeLab.Domain.Abstractions.Errors;

namespace CodeLab.SubmissionProcessing.Worker;

public static class SubmissionProcessingErrors
{
    public static Error RunnerProcessFailed(Guid? submissionId = null) =>
        submissionId is null
            ? Error.Failure(
                "submission.processing.runner.failed", "The runner process failed.")
            : Error.Failure(
                "submission.processing.runner.failed", $"The runner process failed for submission '{submissionId}'.");

    public static Error InvalidRunnerOutput(string details) =>
        Error.Failure("submission.processing.invalid.output",
            $"The runner returned invalid output: {details}.");

    public static Error WorkspacePreparationFailed(string details) =>
        Error.Failure(
            "submission.processing.workspace.preparation.failed",
            $"The workspace could not be prepared: {details}.");

    public static Error TestFileNotFound(string slug) =>
        Error.NotFound(
            "submission.processing.exercise.tests.not.found",
            $"The test file for exercise '{slug}' was not found.");
}