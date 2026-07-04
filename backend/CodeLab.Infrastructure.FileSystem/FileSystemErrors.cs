using CodeLab.Domain.Abstractions.Errors;

namespace CodeLab.Infrastructure.FileSystem;

internal static class FileSystemErrors
{
    public static Error FileNotFound(string? path = null)
    {
        string message = path is null
            ? "Файл не найден"
            : $"Файл '{path}' не найден";

        return Error.NotFound("filesystem.not.found", message, path);
    }

    public static Error InvalidPath(string? path = null)
    {
        string message = path is null
            ? "Путь к файлу недействителен"
            : $"Путь '{path}' недействителен";

        return Error.Validation("filesystem.invalid.path", message, path);
    }

    public static Error IOError(string path, string message)
    {
        return Error.Failure("filesystem.io", $"Ошибка ввода-вывода для '{path}': {message}", path);
    }

    public static Error Unknown(string? message = null)
    {
        return Error.Failure("filesystem.unknown", message ?? "Неизвестная ошибка файловой системы");
    }
}

internal static class FileStorageErrorMapper
{
    public static Error ToError(Exception ex, string? path = null)
    {
        return ex switch
        {
            OperationCanceledException => GeneralErrors.OperationCancelled(),
            ArgumentException => FileSystemErrors.InvalidPath(path),
            PathTooLongException => FileSystemErrors.InvalidPath(path),
            DirectoryNotFoundException => FileSystemErrors.FileNotFound(path),
            FileNotFoundException => FileSystemErrors.FileNotFound(path),
            IOException ioException when path is not null => FileSystemErrors.IOError(path, ioException.Message),
            IOException ioException => FileSystemErrors.IOError("unknown", ioException.Message),
            _ => FileSystemErrors.Unknown(ex.Message)
        };
    }
}