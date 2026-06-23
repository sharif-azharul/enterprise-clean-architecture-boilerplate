using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Application.Features.Users.Commands
{
    public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email
    ) : IRequest<Guid>;
}
