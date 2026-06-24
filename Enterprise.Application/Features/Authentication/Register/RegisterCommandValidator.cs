using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Application.Features.Authentication.Register
{
    public class RegisterCommandValidator
    : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty();

            RuleFor(x => x.LastName)
                .NotEmpty();

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .MinimumLength(8);
        }
    }
}
