using CodeLab.Core;
using Wolverine;

namespace CodeLab.SubmissionProcessing.Worker.Messaging;

public static class WolverineConfiguration
{
    public static void AddWolverine(this IHostApplicationBuilder builder)
    {
        builder.Services.AddWolverine(ExtensionDiscovery.ManualOnly, opts =>
        {
            string rabbitConnectionString = builder.Configuration.GetConnectionString(ConnectionStringNames.RABBIT_MQ)!;

            opts.ApplicationAssembly = typeof(WolverineConfiguration).Assembly;

            opts.ConfigureRabbitMq(rabbitConnectionString);
        });
    }
}