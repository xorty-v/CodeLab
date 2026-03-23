using CodeLab.Contracts.Exercises;
using CodeLab.Core.Endpoints;
using CodeLab.Domain.Abstractions.Errors;
using CodeLab.Domain.Exercises;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CodeLab.Core.Features.Exercises;

public sealed class GetBySlug : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("exercises/{slug}", async Task<EndpointResult<GetExerciseResponse>> (
            [FromRoute] string slug,
            [FromServices] GetBySlugHandler handler,
            CancellationToken cancellationToken) => await handler.Handle(slug, cancellationToken));
    }
}

public sealed class GetBySlugHandler
{
    private readonly IExercisesRepository _exercisesRepository;

    public GetBySlugHandler(IExercisesRepository exercisesRepository)
    {
        _exercisesRepository = exercisesRepository;
    }

    public async Task<Result<GetExerciseResponse, Error>> Handle(string slug, CancellationToken cancellationToken)
    {
        var slugResult = Slug.Create(slug);

        if (slugResult.IsFailure)
            return slugResult.Error;

        var exerciseResult = await _exercisesRepository.GetByAsync(e => e.Slug == slugResult.Value, cancellationToken);
        if (exerciseResult.IsFailure)
            return exerciseResult.Error;

        Exercise exercise = exerciseResult.Value;

        return new GetExerciseResponse(exercise.Slug.Value, exercise.Title.Value, exercise.MarkdownContent.Value);
    }
}