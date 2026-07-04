using CodeLab.Core.Abstractions.FileSystem;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeLab.Infrastructure.FileSystem;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructureFileSystem(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<FileSystemOptions>(configuration.GetSection(FileSystemOptions.SECTION_NAME));

        services.AddScoped<IFileSystemProvider, FileSystemProvider>();

        return services;
    }
}