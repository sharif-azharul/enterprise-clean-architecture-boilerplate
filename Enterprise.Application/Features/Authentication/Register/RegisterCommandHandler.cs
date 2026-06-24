using Enterprise.Application.Features.Authentication.DTOs;
using Enterprise.Application.Interfaces;
using Enterprise.Application.Interfaces.Security;
using Enterprise.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Application.Features.Authentication.Register
{
    public class RegisterCommandHandler
     : IRequestHandler<RegisterCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IRefreshTokenGenerator refreshTokenGenerator,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);

            if (existingUser is not null)
            {
                throw new InvalidOperationException(
                    "A user with this email already exists.");
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = _passwordHasher.Hash(request.Password)
            };

            await _userRepository.AddAsync(user);

            var accessToken = _jwtTokenGenerator.GenerateToken(user);

            var refreshTokenValue = _refreshTokenGenerator.Generate();

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(30),
                IsRevoked = false
            };

            await _refreshTokenRepository.AddAsync(refreshToken);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue
            };
        }
    }
}
