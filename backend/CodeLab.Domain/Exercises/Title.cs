using System.Text.RegularExpressions;
using CodeLab.Domain.Abstractions.Errors;
using CSharpFunctionalExtensions;

namespace CodeLab.Domain.Exercises;

public sealed record Title
{
    public const int MAX_LENGTH = 100;

    private Title(string value) => Value = value;

    public string Value { get; }

    public static Result<Title, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GeneralErrors.ValueIsInvalid("title");

        string normalized = Regex.Replace(value.Trim(), @"\s+", " ");

        if (normalized.Length > MAX_LENGTH)
            return GeneralErrors.ValueIsInvalid("title");

        return new Title(normalized);
    }
}