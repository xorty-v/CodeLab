using System.Text.Json;
using System.Text.Json.Serialization;
using CodeLab.Domain.Abstractions.Errors;
using CodeLab.Domain.Submissions;
using CSharpFunctionalExtensions;

namespace CodeLab.SubmissionProcessing.Worker.DockerProcess;

internal static class DockerOutputParser
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }, PropertyNameCaseInsensitive = true
    };

    public static Result<ContainerResponse, Error> Parse(string stdout)
    {
        if (string.IsNullOrWhiteSpace(stdout))
            return SubmissionProcessingErrors.InvalidRunnerOutput("empty output");

        ContainerResponse? response;
        try
        {
            response = JsonSerializer.Deserialize<ContainerResponse>(stdout, _jsonOptions);
        }
        catch (JsonException ex)
        {
            return SubmissionProcessingErrors.InvalidRunnerOutput($"JSON parse error: {ex.Message}");
        }

        if (response is null)
            return SubmissionProcessingErrors.InvalidRunnerOutput("null response");

        return response;
    }
}

public sealed class ContainerResponse
{
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("tests")]
    public required List<TestItemRaw> Tests { get; init; }
}

public sealed class TestItemRaw
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("status")]
    public TestStatus Status { get; init; }

    [JsonPropertyName("output")]
    public string? Output { get; init; }

    public TestItem ToDomain() => TestItem.Create(Name, Status, Output).Value;
}