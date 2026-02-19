using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeLab.CodeRunner;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCodeRunner(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICodeRunnerService, CodeRunnerService>();

        return services;
    }
}