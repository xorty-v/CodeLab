using CodeLab.Domain.Abstractions.Errors;
using CSharpFunctionalExtensions;

namespace CodeLab.Domain.Submissions;

public sealed record SourceCode
{
    public const int MAX_LENGTH = 10_000;

    private SourceCode(string value) => Value = value;

    public string Value { get; }

    public static Result<SourceCode, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GeneralErrors.ValueIsInvalid(nameof(value));

        var normalized = value.Replace("\r\n", "\n").Replace("\r", "\n");

        if (normalized.Length > MAX_LENGTH)
            return GeneralErrors.ValueIsInvalid(nameof(value));

        return new SourceCode(normalized);
    }
}