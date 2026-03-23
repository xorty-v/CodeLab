using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CodeLab.Domain.Abstractions.Errors;
using CSharpFunctionalExtensions;

namespace CodeLab.Domain.Exercises;

public sealed record Slug
{
    public const int MIN_LENGTH = 2;
    public const int MAX_LENGTH = 80;

    private static readonly Regex _whitespace = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex _invalidChars = new("[^a-z0-9-#+.]+", RegexOptions.Compiled);
    private static readonly Regex _multiDash = new("-+", RegexOptions.Compiled);

    private static readonly Dictionary<char, string> _cyrillicMap = new()
    {
        ['а'] = "a", ['б'] = "b", ['в'] = "v", ['г'] = "g", ['д'] = "d", ['е'] = "e", ['ё'] = "e",
        ['ж'] = "zh", ['з'] = "z", ['и'] = "i", ['й'] = "y", ['к'] = "k", ['л'] = "l", ['м'] = "m",
        ['н'] = "n", ['о'] = "o", ['п'] = "p", ['р'] = "r", ['с'] = "s", ['т'] = "t", ['у'] = "u",
        ['ф'] = "f", ['х'] = "h", ['ц'] = "ts", ['ч'] = "ch", ['ш'] = "sh", ['щ'] = "sch", ['ъ'] = string.Empty,
        ['ы'] = "y", ['ь'] = string.Empty, ['э'] = "e", ['ю'] = "yu", ['я'] = "ya"
    };

    private Slug(string value) => Value = value;

    public string Value { get; }

    public static Result<Slug, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GeneralErrors.ValueIsRequired("slug");

        string normalized = Normalize(value);

        if (normalized.Length is > MAX_LENGTH or < MIN_LENGTH)
            return GeneralErrors.ValueIsInvalid("slug");

        return new Slug(normalized);
    }

    private static string Normalize(string input)
    {
        string s = input.Trim().ToLowerInvariant();

        s = TransliterateCyrillicToLatin(s);

        s = RemoveDiacritics(s);

        s = s.Replace('_', '-');
        s = _whitespace.Replace(s, "-");

        s = _invalidChars.Replace(s, "-");

        s = _multiDash.Replace(s, "-").Trim('-');

        if (s.Length > MAX_LENGTH)
            s = s[..MAX_LENGTH].Trim('-');

        return s;
    }

    private static string TransliterateCyrillicToLatin(string s)
    {
        var sb = new StringBuilder(s.Length);

        foreach (char ch in s)
        {
            if (_cyrillicMap.TryGetValue(ch, out string? repl))
                sb.Append(repl);
            else
                sb.Append(ch);
        }

        return sb.ToString();
    }

    private static string RemoveDiacritics(string text)
    {
        string normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalized.Length);

        foreach (char c in normalized)
        {
            UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory(c);

            if (cat != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}