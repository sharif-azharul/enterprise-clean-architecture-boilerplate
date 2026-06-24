using Enterprise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Application.Interfaces.Security
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
