using CodeLab.Contracts.Exercises;
using CodeLab.Core.Abstractions.Endpoints;
using CodeLab.Core.Abstractions.Validation;
using CodeLab.Core.Database;
using CodeLab.Domain.Abstractions.Errors;
using CodeLab.Domain.Exercises;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace CodeLab.Core.Features.Exercises;

public class GetExercisesCalendarRequestValidator : AbstractValidator<GetExercisesCalendarRequest>
{
    public GetExercisesCalendarRequestValidator()
    {
        RuleFor(x => x.Year)
            .NotNull().WithError(GeneralErrors.ValueIsRequired("year"))
            .GreaterThanOrEqualTo(2026)
            .WithError(GeneralErrors.ValueIsInvalid("year"))
            .LessThanOrEqualTo(2100)
            .WithError(GeneralErrors.ValueIsInvalid("year"));

        RuleFor(x => x.Month)
            .NotNull().WithError(GeneralErrors.ValueIsRequired("month"))
            .InclusiveBetween(1, 12)
            .WithError(GeneralErrors.ValueIsInvalid("month"));
    }
}

public sealed class GetCalendar : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("exercises/calendar", async Task<EndpointResult<ExercisesCalendarResponse>> (
            [AsParameters] GetExercisesCalendarRequest request,
            [FromServices] GetHandler handler,
            CancellationToken cancellationToken) => await handler.Handle(request, cancellationToken));
    }
}

public sealed class GetHandler
{
    private readonly IApplicationReadDbContext _readDbContext;
    private readonly IValidator<GetExercisesCalendarRequest> _validator;

    public GetHandler(IApplicationReadDbContext readDbContext, IValidator<GetExercisesCalendarRequest> validator)
    {
        _readDbContext = readDbContext;
        _validator = validator;
    }

    public async Task<Result<ExercisesCalendarResponse, Error>> Handle(
        GetExercisesCalendarRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToError();

        IQueryable<Exercise> query = _readDbContext.ExercisesQuery.IgnoreQueryFilters();

        var startDate = new DateOnly(request.Year, request.Month, 1);
        var nextMonth = startDate.AddMonths(1);

        List<(DateOnly Date, ExerciseDto Exercise)> exercises = await query
            .Where(x => x.AssignedDate >= startDate && x.AssignedDate < nextMonth)
            .Select(x => new { x.Id, x.Title, x.AssignedDate })
            .Select(x => new ValueTuple<DateOnly, ExerciseDto>(
                x.AssignedDate,
                new ExerciseDto(x.Id, x.Title.Value)))
            .ToListAsync(cancellationToken);

        Dictionary<DateOnly, ExerciseDto> exercisesByDate = exercises.ToDictionary(x => x.Date, x => x.Exercise);

        int daysInMonth = DateTime.DaysInMonth(request.Year, request.Month);

        List<CalendarDayDto> days = Enumerable
            .Range(1, daysInMonth)
            .Select(day =>
            {
                DateOnly date = new(request.Year, request.Month, day);
                exercisesByDate.TryGetValue(date, out ExerciseDto? exercise);

                return new CalendarDayDto(date, exercise);
            })
            .ToList();

        return new ExercisesCalendarResponse(
            request.Year,
            request.Month,
            days);
    }
}