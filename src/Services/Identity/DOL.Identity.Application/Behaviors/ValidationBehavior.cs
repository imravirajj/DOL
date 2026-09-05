using FluentValidation;
using MediatR;
using DOL.SharedKernel;

namespace DOL.Identity.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .Select(f => f.ErrorMessage)
            .ToList();

        if (failures.Count != 0)
        {
            // If TResponse is Result or Result<T>
            var responseType = typeof(TResponse);
            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var failureMethod = responseType.GetMethod("Failure", new[] { typeof(List<string>) });
                if (failureMethod != null)
                {
                    return (TResponse)failureMethod.Invoke(null, new object[] { failures })!;
                }
            }
            else if (responseType == typeof(Result))
            {
                return (TResponse)(object)Result.Failure(failures);
            }

            throw new ValidationException(validationResults.SelectMany(r => r.Errors));
        }

        return await next();
    }
}
