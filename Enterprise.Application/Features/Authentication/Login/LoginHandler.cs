using Enterprise.Application.Features.Authentication.DTOs;
using Enterprise.Application.Interfaces;
using Enterprise.Application.Interfaces.Security;
using Enterprise.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Application.Features.Authentication.Login
{
    public class LoginCommandHandler
    : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public LoginCommandHandler(
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

        public async Task<AuthResponse> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository
                .GetByEmailAsync(request.Email);

            if (user is null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            var isValidPassword =
                _passwordHasher.Verify(
                    request.Password,
                    user.PasswordHash);

            if (!isValidPassword)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            var accessToken =
                _jwtTokenGenerator.GenerateToken(user);

            var refreshTokenValue =
                _refreshTokenGenerator.Generate();

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(30),
                IsRevoked = false
            };

            await _refreshTokenRepository
                .AddAsync(refreshToken);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue
            };
        }
    }
}
