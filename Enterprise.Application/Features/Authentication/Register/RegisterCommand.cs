using Enterprise.Application.Features.Authentication.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Application.Features.Authentication.Register
{
    public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password)
    : IRequest<AuthResponse>;
}
