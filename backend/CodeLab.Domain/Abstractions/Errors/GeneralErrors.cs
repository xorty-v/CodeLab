namespace CodeLab.Domain.Abstractions.Errors;

public static class GeneralErrors
{
    public static Error ValueIsInvalid(string? fieldName = null)
    {
        return fieldName == null
            ? Error.Validation("value.is.invalid", "The value is invalid.")
            : Error.Validation("value.is.invalid", $"The value of '{fieldName}' is invalid.");
    }

    public static Error NotFound(Guid? id = null, string? entityName = null)
    {
        string entity = entityName ?? "record";
        return id == null
            ? Error.NotFound("record.not.found", $"The {entity} was not found.")
            : Error.NotFound("record.not.found", $"The {entity} with id '{id}' was not found.");
    }

    public static Error NotFoundBy(string fieldName, string value, string? entityName = null)
    {
        string entity = entityName ?? "record";
        return Error.NotFound("record.not.found", $"The {entity} with {fieldName} '{value}' was not found.");
    }

    public static Error ValueIsRequired(string? fieldName = null)
    {
        return fieldName == null
            ? Error.Validation("value.is.required", "The value is required.")
            : Error.Validation("value.is.required", $"The field '{fieldName}' is required.");
    }

    public static Error LengthIsInvalid(string? fieldName = null, int? min = null, int? max = null)
    {
        string label = fieldName != null ? $"The field '{fieldName}'" : "The value";

        if (min.HasValue && max.HasValue)
            return Error.Validation("length.is.invalid", $"{label} must be between {min} and {max} characters long.");

        if (min.HasValue)
            return Error.Validation("length.is.invalid", $"{label} must be at least {min} characters long.");

        if (max.HasValue)
            return Error.Validation("length.is.invalid", $"{label} must be at most {max} characters long.");

        return Error.Validation("length.is.invalid", $"{label} has an invalid length.");
    }

    public static Error AlreadyExists(string? entityName = null, string? value = null)
    {
        string entity = entityName ?? "Record";

        if (!string.IsNullOrWhiteSpace(value))
            return Error.Conflict("record.already.exists", $"{entity} '{value}' already exists.");

        return Error.Conflict("record.already.exists", $"{entity} already exists.");
    }

    public static Error UniqueConstraintViolation(string? fieldName = null)
    {
        string field = fieldName != null ? $" for field '{fieldName}'" : string.Empty;
        return Error.Conflict("unique.constraint.violation", $"The value{field} must be unique.");
    }

    public static Error ForeignKeyViolation(string? fieldName = null)
    {
        string field = fieldName ?? "The related record";
        return Error.Conflict("foreign.key.violation", $"{field} is used by other data.");
    }

    public static Error OutOfRange(string? fieldName = null, object? min = null, object? max = null)
    {
        string label = fieldName ?? "The value";

        if (min != null && max != null)
            return Error.Validation("value.out.of.range", $"{label} must be between {min} and {max}.");

        if (min != null)
            return Error.Validation("value.out.of.range", $"{label} must be greater than or equal to {min}.");

        if (max != null)
            return Error.Validation("value.out.of.range", $"{label} must be less than or equal to {max}.");

        return Error.Validation("value.out.of.range", $"{label} is out of range.");
    }

    public static Error InvalidFormat(string? fieldName = null)
    {
        string label = fieldName ?? "The value";
        return Error.Validation("invalid.format", $"{label} has an invalid format.");
    }

    public static Error ConcurrencyConflict() =>
        Error.Conflict("concurrency.conflict", "The data was changed by another user.");

    public static Error OperationCancelled() =>
        Error.Failure("operation.cancelled", "The operation was cancelled.");

    public static Error DatabaseError() =>
        Error.Failure("database.error", "A database error occurred.");

    public static Error Failure(string? message = null) =>
        Error.Failure("server.failure", message ?? "A server error occurred.");

    public static Error Unauthorized() =>
        Error.Failure("unauthorized", "Authorization is required.");

    public static Error Forbidden() =>
        Error.Failure("forbidden", "Access is forbidden.");

    public static Error InvalidOperation(string? message = null) =>
        Error.Validation("invalid.operation", message ?? "The operation is not allowed in the current state.");
}