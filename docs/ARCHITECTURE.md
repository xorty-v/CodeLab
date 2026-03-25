# Архитектура

Интерактивная платформа для ежедневного решения задач на C#. Система обеспечивает асинхронную проверку кода через очередь
сообщений и [специализированный раннер с использованием xUnit-тестов](RUNNER.md).

## Структура каждого сервиса (Clean Architecture + DDD)

```text
{Service}.Domain/                    # Entities, Value Objects, Domain Errors
{Service}.Contracts/                 # DTOs, Messaging
{Service}.Core/                      # Use cases, repository interfaces, validators
  Features/{Entity}/UseCases/        # Vertical slices
{Service}.Infrastructure.Postgres/   # EF Core DbContext, migrations, repositories
{Service}.Web/                       # ASP.NET Core host, DI, endpoint mapping
tests/{Service}.IntegrationTests/    # xUnit
```

## Railway-Oriented Programming

```csharp
// Никогда не throw для бизнес-ошибок — только Result
public async Task<Result<Guid, Error>> Handle(ActionRequest request, CancellationToken ct)
{
    var validResult = await _validator.ValidateAsync(request, ct);
    if (!validResult.IsValid) 
        return validResult.ToError();

    var result = await _repositoty.GetByAsync(request.Slug, ct);
    if (result == null) 
        return GeneralErrors.NotFound(request.Slug);

    return result.Id;
}
```

## Feature Slice (одна фича = один файл)

```csharp
// {Service}.Core/Features/{Entity}/{Action}.cs
public record ActionRequest(string Data);

public class ActionRequestValidator : AbstractValidator<ActionRequest>
{
    public ActionRequestValidator()
    {
        RuleFor(x => x.Data).MustBeValueObject(ValueObject.Create);
    }
}

public sealed class ActionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("resource/{id}/action", async (
            [FromRoute] string id,
            [FromBody] ActionRequest request,
            [FromServices] ActionHandler handler,
            CancellationToken ct) => await handler.Handle(id, request, ct));
    }
}

public sealed class ActionHandler
{
    public async Task<Result<Guid, Error>> Handle(ActionRequest request, CancellationToken ct)
    {
        // 1. Validate → 2. Create Value Objects → 3. Business rules → 4. Save → 5. Return Result
    }
}
```

## Доступ к данным для чтения

Для чтения данных используется единый интерфейс `IApplicationReadDbContext`, предоставляющий `IQueryable` сущностей. Обработчики строят запросы напрямую через этот интерфейс, без отдельных методов репозиториев для каждого Get. При необходимости в будущем реализацию можно легко заменить на Dapper.

```csharp
IQueryable<...> query = _readDbContext.ExercisesQuery.IgnoreQueryFilters();
var result = await query
        .Where(x => /* условие по request */)
        .Select(x => /* проекция в DTO */)
        .ToListAsync(ct);
```

## EF Core

```csharp
// Применяем соглашения snake_case (EFCore.NamingConventions)
builder.ToTable("entities");
builder.Property(x => x.Field)
    .HasConversion(v => v.Value, v => Field.Create(v).Value)
    .HasMaxLength(...);
```

## Обработка ошибок & API оборачивает все ответы в Envelope<T>:

```csharp
public record Envelope<T>
{
    T? Result { get; }
    Error? Error { get; }
    bool IsError { get; }
    DateTime TimeGenerated { get; }
}
```

Error содержит Messages (список ErrorMessage) и Type (VALIDATION, NOT_FOUND, CONFLICT, FAILURE и т.д.). EndpointResult<T> автоматически конвертирует Result<T, Error> в корректный HTTP-статус + Envelope.


## Архитектурные заметки
1. Все реализации IEndpoint в сборке Core сканируются через reflection и подключаются в слое Web вызовом app.MapEndpoints().
2. Все Repository автоматически регистрируются в DI по интерфейсам через сканирование сборки с Scoped временем жизни.