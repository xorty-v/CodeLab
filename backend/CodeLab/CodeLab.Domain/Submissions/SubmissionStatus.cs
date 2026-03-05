using System.Text.Json.Serialization;

namespace CodeLab.Domain.Submissions;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SubmissionStatus
{
    Pending,
    Processing,
    Succeeded,
    Failed,
    Error
}