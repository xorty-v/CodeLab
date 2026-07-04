using CodeLab.Core.Abstractions.FileSystem;
using CodeLab.Domain.Abstractions.Errors;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace CodeLab.Infrastructure.FileSystem;

internal sealed class FileSystemProvider : IFileSystemProvider
{
    private readonly ILogger<FileSystemProvider> _logger;

    public FileSystemProvider(ILogger<FileSystemProvider> logger)
    {
        _logger = logger;
    }

    public async Task<Result<string, Error>> ReadTextAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            return FileSystemErrors.InvalidPath(path);

        try
        {
            if (!File.Exists(path))
                return FileSystemErrors.FileNotFound(path);

            string content = await File.ReadAllTextAsync(path, cancellationToken);
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read file {Path}", path);
            return FileStorageErrorMapper.ToError(ex, path);
        }
    }

    public async Task<UnitResult<Error>> WriteTextAsync(
        string path,
        string content,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            return FileSystemErrors.InvalidPath(path);

        try
        {
            EnsureParentDirectoryExists(path);

            await File.WriteAllTextAsync(path, content, cancellationToken);

            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write file {Path}", path);
            return FileStorageErrorMapper.ToError(ex, path);
        }
    }

    public UnitResult<Error> Copy(
        string sourcePath,
        string destinationPath,
        bool overwrite = false)
    {
        if (string.IsNullOrWhiteSpace(sourcePath))
            return FileSystemErrors.InvalidPath(sourcePath);

        if (string.IsNullOrWhiteSpace(destinationPath))
            return FileSystemErrors.InvalidPath(destinationPath);

        try
        {
            if (!File.Exists(sourcePath))
                return FileSystemErrors.FileNotFound(sourcePath);

            EnsureParentDirectoryExists(destinationPath);

            File.Copy(sourcePath, destinationPath, overwrite);

            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to copy file from {SourcePath} to {DestinationPath}",
                sourcePath,
                destinationPath);

            return FileStorageErrorMapper.ToError(ex, destinationPath);
        }
    }

    public UnitResult<Error> Delete(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return FileSystemErrors.InvalidPath(path);

        try
        {
            if (!File.Exists(path))
                return UnitResult.Success<Error>();

            File.Delete(path);

            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {Path}", path);
            return FileStorageErrorMapper.ToError(ex, path);
        }
    }

    private static void EnsureParentDirectoryExists(string filePath)
    {
        string? directory = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);
    }
}