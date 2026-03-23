using Microsoft.AspNetCore.Routing;

namespace CodeLab.Core.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}