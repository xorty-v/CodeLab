using CodeLab.Domain.Abstractions.Errors;
using CSharpFunctionalExtensions;

namespace CodeLab.Domain.Exercises;

public sealed record MarkdownContent
{
    public const int MAX_LENGTH = 3000;

    private MarkdownContent(string value) => Value = value;

    public string Value { get; }

    public static Result<MarkdownContent, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GeneralErrors.ValueIsInvalid(nameof(value));

        string trimmed = value.Trim();

        if (trimmed.Length > MAX_LENGTH)
            return GeneralErrors.ValueIsInvalid(nameof(value));

        return new MarkdownContent(trimmed);
    }
}