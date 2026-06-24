using Enterprise.Application.Features.Authentication.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Application.Features.Authentication.Login
{
    public record LoginCommand(
    string Email,
    string Password)
    : IRequest<AuthResponse>;
}
