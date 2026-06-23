using Enterprise.Application.Interfaces;
using Enterprise.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Application.Features.Users.Commands
{
    public class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUserRepository _repository;

        public CreateUserCommandHandler(
            IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(
            CreateUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email
            };

            await _repository.AddAsync(user);

            return user.Id;
        }
    }
}
