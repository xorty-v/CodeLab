using CodeLab.Contracts.Submissions.Messaging;
using Wolverine;
using Wolverine.RabbitMQ;

namespace CodeLab.Core.Messaging;

public static class RabbitMqConfiguration
{
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

        opts.PublishMessagesToRabbitMqExchange<SubmissionCreated>(
            SubmissionEventsRouting.EXCHANGE, _ => SubmissionEventsRouting.RoutingKeys.SubmissionCreated());
    }
}