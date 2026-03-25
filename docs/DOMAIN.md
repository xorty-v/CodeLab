# Domain

## Сущности

### Exercise

Файл: Exercises/Exercise.cs

Назначение:

- Описание учебной задачи, доступной для выполнения в определенный день.

Поля:

- `Id: Guid`
- `Title: Title` — нормализованный заголовок.
- `MarkdownContent: MarkdownContent` — текст задания в формате Markdown.
- `Slug: Slug` — уникальный URL-friendly идентификатор.
- `AssignedDate: DateOnly` — дата, на которую назначено упражнение (уникальна).
- `CreatedAt: DateTime` — дата создания (UTC).

### Submission

Файл: Submissions/Submission.cs

Назначение:

- Решение упражнения, отправленное пользователем и жизненный цикл его проверки.

Поля:

- `Id: Guid`
- `ExerciseId: Guid` — связь с упражнением (FK).
- `SourceCode: SourceCode` — код решения от пользователя.
- `Status: SubmissionStatus` — текущий этап (Pending, Processing, Completed).
- `TestResult: TestResult?` — результат выполнения тестов (хранится как jsonb).
- `SubmittedAt: DateTime` — время отправки.
- `CompletedAt: DateTime?` — время завершения проверки.

Поведение:

- `MarkAsProcessing()` — перевод в состояние проверки. Допустимо только из Pending.
- `MarkAsCompleted(tests, message)` — фиксация результатов тестов и завершение процесса. Допустимо только из Processing.

## +Bonus

1. Таблица exercises:

- Уникальный индекс `ix_exercises_slug` на поле `Slug`.
- Уникальный индекс `ix_exercises_assigned_date` на поле `AssignedDate`.

2. Таблица submissions:

- Поле `test_result` мапится как JSONB.
- Каскадное удаление при удалении родительского Exercise.
- Хранение Enums в базе в строковом виде `HasConversion<string>`.
