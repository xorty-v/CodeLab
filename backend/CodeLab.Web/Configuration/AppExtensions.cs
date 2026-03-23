using CodeLab.Core.Endpoints;
using CodeLab.Web.Middlewares;
using Serilog;

namespace CodeLab.Web.Configuration;

public static class AppExtensions
{
    public static IApplicationBuilder Configure(this WebApplication app)
    {
        app.UseExceptionMiddleware();
        app.UseRequestCorrelationId();
        app.UseSerilogRequestLogging();

        app.MapOpenApi();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "CodeLab V1");
        });

        app.MapEndpoints(app.MapGroup("/api"));

        return app;
    }
}