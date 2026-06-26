using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Application.Interfaces.Services
{
    public interface ICurrentUserService
    {
        string? UserId { get; }

        string? Email { get; }
    }
}
