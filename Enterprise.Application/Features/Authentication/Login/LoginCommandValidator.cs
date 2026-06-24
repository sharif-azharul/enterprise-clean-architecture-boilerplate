using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Application.Features.Authentication.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
