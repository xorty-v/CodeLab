using Microsoft.AspNetCore.Routing;

namespace CodeLab.Core.Abstractions.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}