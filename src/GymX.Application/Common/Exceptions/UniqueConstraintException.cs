namespace GymX.Application.Common.Exceptions;

public class UniqueConstraintException : ApplicationException
{
    public UniqueConstraintException(string message) : base(message) { }

    public UniqueConstraintException(string message, Exception innerException) : base(message, innerException) { }
}
