using GymX.Domain.Exceptions;

namespace GymX.Application.Common.Interfaces
{
    public class BusinessRuleException : DomainException
    {
        public BusinessRuleException(string code, string message) : base(code, message)
        {
        }
    }
}
