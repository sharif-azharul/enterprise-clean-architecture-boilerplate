using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Application.Interfaces.Security
{
    public interface IRefreshTokenGenerator
    {
        string Generate();
    }
}
