using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymX.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public string Code { get; }
        public DomainException(string code, string message) : base(message)
        {
            Code = code;
        }
    }
}
