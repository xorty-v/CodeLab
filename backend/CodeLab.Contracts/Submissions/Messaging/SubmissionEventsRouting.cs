namespace CodeLab.Contracts.Submissions.Messaging;

public static class SubmissionEventsRouting
{
    public const string EXCHANGE = "codelab-submissions";

    public static class RoutingKeys
    {
        public static string SubmissionCreated() => "submission.created";
    }
}