using CodeLab.Core.Features.Submissions;
using CodeLab.SubmissionProcessing.DockerProcess;
using CodeLab.SubmissionProcessing.ProcessExecutor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeLab.SubmissionProcessing;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddSubmissionProcessing(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SubmissionProcessingOptions>(
            configuration.GetSection(SubmissionProcessingOptions.SECTION_NAME));

        services.AddScoped<IProcessRunner, ProcessRunner>();
        services.AddScoped<IDockerProcessRunner, DockerProcessRunner>();
        services.AddScoped<IFileSystemService, FileSystemService>();

        services.AddScoped<ISubmissionProcessingService, SubmissionProcessingService>();

        return services;
    }
}