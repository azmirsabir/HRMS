namespace Infrastructure.Exceptions;

public class InfrastructureException : Exception
{
    public int StatusCode { get; }

    public InfrastructureException(int statusCode)
    {
        this.StatusCode = statusCode;
    }

    public InfrastructureException(string? message, int statusCode) : base(message)
    {
        this.StatusCode = statusCode;
    }

    public InfrastructureException(string? message, Exception? innerException, int statusCode) : base(message, innerException)
    {
        this.StatusCode = statusCode;
    }
}