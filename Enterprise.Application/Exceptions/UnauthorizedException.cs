using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Application.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message)
            : base(message)
        {
        }
    }
}
