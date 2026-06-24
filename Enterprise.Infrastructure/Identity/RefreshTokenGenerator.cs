using Enterprise.Application.Interfaces.Security;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Enterprise.Infrastructure.Identity
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        public string Generate()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(randomBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}
