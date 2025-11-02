using Application.Contracts;
using FluentValidation;
using MediatR;

namespace Application.Validations;

public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
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
        {
            return await next();
        }

        var validationContext = new ValidationContext<TRequest>(request);

        var errors = _validators
            .Select(v => v.Validate(validationContext))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .Select(x => new ValidationError(x.PropertyName, x.ErrorMessage))
            .ToList();

        if (errors.Any())
        {
            return (TResponse)typeof(TResponse).GetMethod("ValidationFailure")!.Invoke(null, new object[] { errors })!;
        }

        return await next();
    }
}