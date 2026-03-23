using System.Text.Json.Serialization;

namespace CodeLab.Domain.Submissions;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SubmissionStatus
{
    Pending,
    Processing,
    Completed
}