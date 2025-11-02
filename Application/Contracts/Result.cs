using Application.Validations;
using MediatR;

namespace Application.Contracts;

public class Result
{
    public bool IsSuccess { get; set; }
    public bool IsFailure => !IsSuccess;
    public List<ValidationError>? Errors { get; set; }


    public static Result Success()
    {
        return new Result { IsSuccess = true };
    }

    public static Result ValidationFailure(List<ValidationError> errors)
    {
        return new Result { Errors = errors, IsSuccess = false };
    }
}

public class Result<T, TFailures> : Result, IRequest where TFailures : Enum
{
    public T? Data { get; private init; }
    public TFailures? Error { get; private init; }

    public static Result<T, TFailures> Failure(TFailures failure) => new Result<T, TFailures> { Error = failure };

    public static Result<T, TFailures> Success(T data) => new Result<T, TFailures> { IsSuccess = true, Data = data };

    public static new Result<T, TFailures> ValidationFailure(List<ValidationError> errors) => new() { Errors = errors };

    public static implicit operator Result<T, TFailures>(T data) => Success(data);

    public static Result<T, TFailures> Create(T? data) => new Result<T, TFailures> { Data = data };
}