using System.Text.Json;
using System.Text.Json.Serialization;
using CodeLab.Domain;
using CodeLab.Domain.Abstractions.Errors;
using CodeLab.Domain.Submissions;
using CSharpFunctionalExtensions;

namespace CodeLab.SubmissionProcessing.DockerProcess;

internal static class DockerOutputParser
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }, PropertyNameCaseInsensitive = true
    };

    public static Result<ContainerResponse, Error> Parse(string stdout)
    {
        if (string.IsNullOrWhiteSpace(stdout))
            return CodelabErrors.InvalidDockerOutput("Empty output");

        ContainerResponse? response;
        try
        {
            response = JsonSerializer.Deserialize<ContainerResponse>(stdout, _jsonOptions);
        }
        catch (JsonException ex)
        {
            return CodelabErrors.InvalidDockerOutput($"JSON parse error: {ex.Message}");
        }

        if (response is null)
            return CodelabErrors.InvalidDockerOutput("Null response");

        return response;
    }
}

public sealed class ContainerResponse
{
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("tests")]
    public required List<TestItem> Tests { get; init; }
}