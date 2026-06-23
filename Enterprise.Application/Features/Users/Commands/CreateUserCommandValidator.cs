using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Application.Features.Users.Commands
{
    public class CreateUserCommandValidator
    : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.FirstName)
                .NotEmpty();

            RuleFor(x => x.LastName)
                .NotEmpty();
        }
    }
}
