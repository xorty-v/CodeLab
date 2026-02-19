using CodeLab.Core.Endpoints;
using CodeLab.Domain.Abstractions.Errors;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CodeLab.Core.Features.Exercises;

public record GetExercisesDto(string Slug, string Title);

public sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("exercises", async Task<EndpointResult<IReadOnlyList<GetExercisesDto>>> (
                [FromServices] GetHandler handler,
                CancellationToken cancellationToken) =>
            await handler.Handle(cancellationToken));
    }
}

public sealed class GetHandler
{
    private readonly IExercisesRepository _exercisesRepository;

    public GetHandler(IExercisesRepository exercisesRepository)
    {
        _exercisesRepository = exercisesRepository;
    }

    public async Task<Result<IReadOnlyList<GetExercisesDto>, Error>> Handle(CancellationToken cancellationToken)
    {
        var exercises = await _exercisesRepository.GetAllAsync(cancellationToken);

        var dtos = exercises
            .Select(e => new GetExercisesDto(e.Slug.Value, e.Title))
            .ToList();

        return Result.Success<IReadOnlyList<GetExercisesDto>, Error>(dtos);
    }
}