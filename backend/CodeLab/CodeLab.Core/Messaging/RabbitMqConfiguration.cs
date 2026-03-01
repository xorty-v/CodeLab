using CodeLab.Core.Features.Submissions.Messaging;
using Wolverine;
using Wolverine.RabbitMQ;

namespace CodeLab.Core.Messaging;

public static class SubmissionEventsRouting
{
    public const string EXCHANGE = "codelab-submissions";

    public static class RoutingKeys
    {
        public const string ALL_CREATED = "*.created";

        public static string SubmissionCreated() => "submission.created";
    }
}

public static class RabbitMqConfiguration
{
    private const string SUBMISSIONS_QUEUE = "codelab.submissions.runner";

    public static void ConfigureRabbitMq(this WolverineOptions opts, string connectionString)
    {
        opts.UseRabbitMq(new Uri(connectionString))
            .AutoProvision()
            .EnableWolverineControlQueues()
            .UseQuorumQueues()
            .DeclareExchange(SubmissionEventsRouting.EXCHANGE, exchange =>
            {
                exchange.ExchangeType = ExchangeType.Topic;
                exchange.IsDurable = true;
            });

        opts.ListenToRabbitQueue(SUBMISSIONS_QUEUE, queue =>
        {
            queue.BindExchange(
                SubmissionEventsRouting.EXCHANGE,
                SubmissionEventsRouting.RoutingKeys.SubmissionCreated());
        });

        opts.PublishMessagesToRabbitMqExchange<SubmissionCreated>(
            SubmissionEventsRouting.EXCHANGE, _ => SubmissionEventsRouting.RoutingKeys.SubmissionCreated());
    }
}