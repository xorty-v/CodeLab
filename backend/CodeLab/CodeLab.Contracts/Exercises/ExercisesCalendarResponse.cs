namespace CodeLab.Contracts.Exercises;

public record ExercisesCalendarResponse(int Year, int Month, IReadOnlyList<CalendarDayDto> Days);

public record CalendarDayDto(DateOnly Date, ExerciseDto? Exercise);