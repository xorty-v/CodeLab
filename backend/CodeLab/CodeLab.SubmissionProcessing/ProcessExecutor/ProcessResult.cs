namespace CodeLab.SubmissionProcessing.ProcessExecutor;

public record ProcessResult(int ExitCode, string StandardOutput, string StandardError);
