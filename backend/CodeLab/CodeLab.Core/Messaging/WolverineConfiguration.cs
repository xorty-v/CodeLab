using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Wolverine;

namespace CodeLab.Core.Messaging;

public static class WolverineConfiguration
{
    public static void AddWolverine(this WebApplicationBuilder builder)
    {
        builder.Host.ConfigureServices((context, services) =>
        {
            string rabbitConnectionString = context.Configuration.GetConnectionString(ConnectionStringNames.RABBIT_MQ)!;

            services.AddWolverine(ExtensionDiscovery.ManualOnly, opts =>
            {
                opts.ApplicationAssembly = typeof(WolverineConfiguration).Assembly;

                opts.ConfigureRabbitMq(rabbitConnectionString);
            });
        });
    }
}