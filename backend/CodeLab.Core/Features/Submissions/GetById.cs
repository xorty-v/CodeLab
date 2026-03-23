using CodeLab.Contracts.Submissions;
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
        app.MapGet("submissions/{submissionId:guid}", async Task<EndpointResult<SubmissionResponse>> (
            [FromRoute] Guid submissionId,
            [FromServices] GetByIdHandler handler,
            CancellationToken cancellationToken) => await handler.Handle(submissionId, cancellationToken));
    }
}

public sealed class GetByIdHandler
{
    private readonly ISubmissionsRepository _submissionsRepository;

    public GetByIdHandler(ISubmissionsRepository submissionsRepository)
    {
        _submissionsRepository = submissionsRepository;
    }

    public async Task<Result<SubmissionResponse, Error>> Handle(Guid submissionId, CancellationToken cancellationToken)
    {
        var submissionResult = await _submissionsRepository.GetByAsync(s => s.Id == submissionId, cancellationToken);
        if (submissionResult.IsFailure)
            return submissionResult.Error;

        var submission = submissionResult.Value;

        return SubmissionMapper.Map(submission);
    }
}

internal static class SubmissionMapper
{
    public static SubmissionResponse Map(Submission submission) =>
        new(
            submission.Id,
            submission.Status,
            submission.TestResult is not null ? MapTestResult(submission.TestResult) : null
        );

    private static TestResultDto MapTestResult(TestResult testResult) =>
        new(
            testResult.Status,
            testResult.Message,
            testResult.TestItems ?? []
        );
}