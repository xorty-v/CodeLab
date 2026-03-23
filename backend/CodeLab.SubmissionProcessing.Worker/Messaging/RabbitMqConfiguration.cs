using CodeLab.Contracts.Submissions.Messaging;
using Wolverine;
using Wolverine.RabbitMQ;

namespace CodeLab.SubmissionProcessing.Worker.Messaging;

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
    }
}