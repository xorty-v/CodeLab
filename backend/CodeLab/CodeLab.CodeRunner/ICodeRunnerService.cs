using CodeLab.Domain.Abstractions.Errors;
using CSharpFunctionalExtensions;

namespace CodeLab.CodeRunner;

public interface ICodeRunnerService
{
    Task<Result<CodeExecutionResult, Error>> RunCodeAsync(string slug, string code, CancellationToken cancellationToken);
}