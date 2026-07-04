using System.Globalization;
using CodeLab.Infrastructure.FileSystem;
using CodeLab.Infrastructure.Postgres;
using CodeLab.SubmissionProcessing.Worker;
using CodeLab.SubmissionProcessing.Worker.Messaging;
using Serilog;
using Exception = System.Exception;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting worker service");

    var builder = Host.CreateApplicationBuilder(args);

    string environment = builder.Environment.EnvironmentName;

    builder.Configuration.AddJsonFile($"appsettings.{environment}.json", true, true);

    builder.Configuration.AddEnvironmentVariables();

    builder.Services.AddConfiguration(builder.Configuration);

    builder.Services.AddInfrastructurePostgres(builder.Configuration);

    builder.Services.AddInfrastructureFileSystem(builder.Configuration);

    builder.AddWolverine();

    var host = builder.Build();

    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Worker service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}