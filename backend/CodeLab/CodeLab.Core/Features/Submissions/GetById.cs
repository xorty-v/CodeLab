using CodeLab.Core.Endpoints;
using CodeLab.Domain.Abstractions.Errors;
using CodeLab.Domain.Submissions;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CodeLab.Core.Features.Submissions;

public sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("submissions/{submissionId:guid}", async Task<EndpointResult<SubmissionStatus>> (
                [FromRoute] Guid submissionId,
                [FromServices] GetByIdHandler handler,
                CancellationToken cancellationToken) =>
            await handler.Handle(submissionId, cancellationToken));
    }
}

public sealed class GetByIdHandler
{
    private readonly ISubmissionsRepository _submissionsRepository;

    public GetByIdHandler(ISubmissionsRepository submissionsRepository)
    {
        _submissionsRepository = submissionsRepository;
    }

    public async Task<Result<SubmissionStatus, Error>> Handle(Guid submissionId, CancellationToken cancellationToken)
    {
        Result<Submission, Error> submissionResult = await _submissionsRepository.GetById(submissionId, cancellationToken);
        if (submissionResult.IsFailure)
            return submissionResult.Error;

        return submissionResult.Value.Status;
    }
}