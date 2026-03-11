using CodeLab.Contracts.Submissions;
using CodeLab.Contracts.Submissions.Messaging;
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
using Wolverine;

namespace CodeLab.Core.Features.Submissions;

public class SubmitRequestValidator : AbstractValidator<SubmitRequest>
{
    public SubmitRequestValidator()
    {
        RuleFor(x => x.SourceCode).MustBeValueObject(SourceCode.Create);
    }
}

public sealed class Submit : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("exercises/{slug}/submissions", async Task<EndpointResult<Guid>> (
            [FromRoute] string slug,
            [FromBody] SubmitRequest request,
            [FromServices] SubmitHandler handler,
            CancellationToken cancellationToken) => await handler.Handle(slug, request, cancellationToken));
    }
}

public sealed class SubmitHandler
{
    private readonly ISubmissionsRepository _submissionsRepository;
    private readonly IExercisesRepository _exercisesRepository;
    private readonly IMessageBus _bus;
    private readonly IValidator<SubmitRequest> _validator;

    public SubmitHandler(
        IExercisesRepository exercisesRepository,
        ISubmissionsRepository submissionsRepository,
        IMessageBus bus,
        IValidator<SubmitRequest> validator)
    {
        _submissionsRepository = submissionsRepository;
        _exercisesRepository = exercisesRepository;
        _bus = bus;
        _validator = validator;
    }

    public async Task<Result<Guid, Error>> Handle(
        string slug,
        SubmitRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToError();

        Slug valueSlug = Slug.Parse(slug);
        SourceCode sourceCode = SourceCode.Create(request.SourceCode).Value;

        var exerciseResult = await _exercisesRepository.GetByAsync(e => e.Slug == valueSlug, cancellationToken);
        if (exerciseResult.IsFailure)
            return exerciseResult.Error;

        var exercise = exerciseResult.Value;
        var submission = new Submission(exercise.Id, sourceCode);

        await _submissionsRepository.AddAsync(submission, cancellationToken);
        await _bus.PublishAsync(new SubmissionCreated(submission.Id, exercise.Slug.Value));

        return submission.Id;
    }
}