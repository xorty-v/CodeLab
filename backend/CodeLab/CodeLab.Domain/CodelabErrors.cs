using CodeLab.Domain.Abstractions.Errors;

namespace CodeLab.Domain;

public static class CodelabErrors
{
    public static Error DatabaseError() =>
        Error.Failure("codelab.database.error", "Ошибка базы данных при работе с сервисом");

    public static Error OperationCancelled() =>
        Error.Failure("codelab.operation.cancelled", "Операция была отменена");
}