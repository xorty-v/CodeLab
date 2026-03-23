using CodeLab.Core.Features.Submissions;
using CodeLab.SubmissionProcessing.Worker.DockerProcess;
using CodeLab.SubmissionProcessing.Worker.ProcessExecutor;

namespace CodeLab.SubmissionProcessing.Worker;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services,
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