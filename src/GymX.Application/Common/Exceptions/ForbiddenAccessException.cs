
namespace GymX.Application.Common.Exceptions
{
    public class ForbiddenAccessException : ApplicationException
    {
        public ForbiddenAccessException() : base("You do not have permission to access this resource.") { }
    }
}
