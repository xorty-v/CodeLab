using CodeLab.Core.Features.Exercises;
using CodeLab.Core.Features.Submissions;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeLab.Core;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<GetHandler>();
        services.AddScoped<GetBySlugHandler>();

        services.AddScoped<SubmitHandler>();
        services.AddScoped<GetByIdHandler>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjectionExtensions).Assembly);

        return services;
    }
}