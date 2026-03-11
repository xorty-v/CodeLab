using CodeLab.Domain;
using CodeLab.Domain.Abstractions.Errors;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeLab.SubmissionProcessing;

public interface IFileSystemService
{
    Result<string, Error> PrepareWorkspace(Guid submissionId, string slug, string sourceCode);

    void CleanupWorkspace(string workspacePath);
}

internal sealed class FileSystemService : IFileSystemService
{
    private const string SUBMISSIONS_ROOT_FOLDER = "Submissions";
    private const string EXERCISES_ROOT_FOLDER = "Exercises";
    private const string SOLUTION_FILE_NAME = "Solution.cs";
    private const string TEST_FILE_NAME = "SolutionTests.cs";

    private readonly SubmissionProcessingOptions _options;
    private readonly ILogger<FileSystemService> _logger;

    public FileSystemService(IOptions<SubmissionProcessingOptions> options, ILogger<FileSystemService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Result<string, Error> PrepareWorkspace(Guid submissionId, string slug, string sourceCode)
    {
        try
        {
            var workDir = Path.Combine(_options.WorkspacePath, SUBMISSIONS_ROOT_FOLDER,
                submissionId.ToString());
            var testFilePath =
                Path.Combine(_options.WorkspacePath, EXERCISES_ROOT_FOLDER, slug, TEST_FILE_NAME);

            if (!File.Exists(testFilePath))
                return CodelabErrors.TestFileNotFound(slug);

            if (!Directory.Exists(workDir))
                Directory.CreateDirectory(workDir);

            File.WriteAllText(Path.Combine(workDir, SOLUTION_FILE_NAME), sourceCode);
            File.Copy(testFilePath, Path.Combine(workDir, TEST_FILE_NAME), true);

            return workDir;
        }
        catch (IOException ex)
        {
            return CodelabErrors.PreparationFailed(ex.Message);
        }
    }

    public void CleanupWorkspace(string workDirectory)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(workDirectory) && Directory.Exists(workDirectory))
            {
                Directory.Delete(workDirectory, true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cleanup workspace {WorkDirectory}", workDirectory);
        }
    }
}