using System.Text.RegularExpressions;
using CodeLab.Domain.Abstractions.Errors;
using CSharpFunctionalExtensions;

namespace CodeLab.Domain.Exercises;

public record Slug
{
    public string Value { get; }

    private Slug(string value)
    {
        Value = value;
    }

    public static Result<Slug, Error> Generate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return GeneralErrors.ValueIsInvalid("slug");
        }

        string normalized = value.ToLowerInvariant().Trim();

        normalized = Regex.Replace(normalized, @"[^a-z0-9\s-]", "");
        normalized = Regex.Replace(normalized, @"[\s-]+", "-").Trim('-');

        return new Slug(normalized);
    }

    public static Slug Parse(string value)
    {
        return new Slug(value.ToLowerInvariant().Trim());
    }
}