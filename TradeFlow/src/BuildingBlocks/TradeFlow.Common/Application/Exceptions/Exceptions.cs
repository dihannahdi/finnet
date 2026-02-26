namespace TradeFlow.Common.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key) : base($"Entity \"{name}\" ({key}) was not found.") { }
}

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base("You do not have permission to perform this action.") { }
}

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}

public class BadRequestException : Exception
{
    public List<string> Errors { get; }

    public BadRequestException(string message) : base(message) { Errors = new List<string> { message }; }
    public BadRequestException(List<string> errors) : base("Multiple validation failures.") { Errors = errors; }
}
