using CodeLab.CodeRunner;
using CodeLab.Core.Endpoints;
using CodeLab.Core.Features.Exercises;
using CodeLab.Core.Validation;
using CodeLab.Domain.Abstractions.Errors;
using CodeLab.Domain.Exercises;
using CodeLab.Domain.Submissions;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace CodeLab.Core.Features.Submissions;

public record SubmitRequest(string SourceCode);

public class SubmitRequestValidator : AbstractValidator<SubmitRequest>
{
    public SubmitRequestValidator()
    {
        RuleFor(x => x.SourceCode).NotEmpty();
    }
}

public sealed class Submit : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("submissions/submit/{slug}", async Task<EndpointResult<string>> (
            [FromRoute] string slug,
            [FromBody] SubmitRequest request,
            [FromServices] SubmitHandler handler,
            CancellationToken cancellationToken) => await handler.Handle(slug, request, cancellationToken));
    }
}

public sealed class SubmitHandler
{
    private readonly ILogger<SubmitHandler> _logger;
    private readonly ISubmissionsRepository _submissionsRepository;
    private readonly IExercisesRepository _exercisesRepository;
    private readonly IValidator<SubmitRequest> _validator;
    private readonly ICodeRunnerService _codeRunnerService;

    public SubmitHandler(
        ILogger<SubmitHandler> logger,
        IExercisesRepository exercisesRepository,
        ISubmissionsRepository submissionsRepository,
        IValidator<SubmitRequest> validator, ICodeRunnerService codeRunnerService)
    {
        _logger = logger;
        _submissionsRepository = submissionsRepository;
        _exercisesRepository = exercisesRepository;
        _validator = validator;
        _codeRunnerService = codeRunnerService;
    }

    public async Task<Result<string, Error>> Handle(
        string slug,
        SubmitRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToError();

        var exerciseResult = await _exercisesRepository.GetBySlugAsync(Slug.FromString(slug), cancellationToken);
        if (exerciseResult.IsFailure)
            return exerciseResult.Error;

        var submission = new Submission(Guid.NewGuid(), exerciseResult.Value.Id, request.SourceCode);

        await _submissionsRepository.AddAsync(submission, cancellationToken);

        _logger.LogInformation("Created submission {Id}", submission.Id);

        var result = await _codeRunnerService.RunCodeAsync(submission.Id, slug, request.SourceCode, cancellationToken);

        return result.Value.ToString();
    }
}