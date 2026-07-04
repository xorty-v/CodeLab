using CodeLab.Domain.Abstractions.Errors;
using CSharpFunctionalExtensions;

namespace CodeLab.Core.Abstractions.FileSystem;

public interface IFileSystemProvider
{
    Task<Result<string, Error>> ReadTextAsync(
        string path,
        CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> WriteTextAsync(
        string path,
        string content,
        CancellationToken cancellationToken = default);

    UnitResult<Error> Copy(
        string sourcePath,
        string destinationPath, bool overwrite = false);

    UnitResult<Error> Delete(string path);
}