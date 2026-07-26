
namespace GymX.Domain.Exceptions
{
    public class BusinessRuleException : DomainException
    {
        public BusinessRuleException(string code, string message) : base(code, message)
        {
        }
    }
}
