using Backend.Domain.Common;
using FluentValidation;
using MediatR;

namespace Backend.Application.Behaviors;


public sealed class ValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationErrors = _validators
            .Select(v => v.Validate(context))
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .Select(e => e.ErrorMessage)
            .Distinct()
            .ToArray();

        if (validationErrors.Length == 0)
            return await next();

        // Return a Failure result with all validation errors joined
        var errorMessage = string.Join("; ", validationErrors);
        var error = Error.Validation("Validation.Failed", errorMessage);

        // Try to create a Result<T> failure dynamically
        var resultType = typeof(TResponse);
        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var innerType = resultType.GetGenericArguments()[0];
            var failureMethod = typeof(Result<>)
                .MakeGenericType(innerType)
                .GetMethod(nameof(Result<object>.Failure))!;

            return (TResponse)failureMethod.Invoke(null, [error])!;
        }

        throw new ValidationException(string.Join(", ", validationErrors));
    }
}
