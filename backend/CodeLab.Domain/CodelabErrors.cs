using CodeLab.Domain.Abstractions.Errors;

namespace CodeLab.Domain;

public static class CodelabErrors
{
    public static Error DatabaseError()
    {
        return Error.Failure("codelab.database.error", "Ошибка базы данных при работе с сервисом");
    }

    public static Error ProcessFailed()
    {
        return Error.Failure("process.failed", "Процесс завершился с ошибкой");
    }

    public static Error InvalidDockerOutput(string details)
    {
        return Error.Failure("docker.invalid.output", $"Невалидный вывод docker: {details}");
    }

    public static Error PreparationFailed(string details)
    {
        return Error.Failure("submission.preparation.failed", $"Ошибка при подготовке файлов: {details}");
    }

    public static Error TestFileNotFound(string slug)
    {
        return Error.Failure("exercise.tests.not_found", $"Файл тестов для упражнения '{slug}' не найден");
    }
}