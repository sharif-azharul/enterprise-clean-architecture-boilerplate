using Enterprise.Application.Interfaces;
using Enterprise.Application.Interfaces.Security;
using Enterprise.Infrastructure.Identity;
using Enterprise.Infrastructure.Persistence.Contexts;
using Enterprise.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection
            AddInfrastructure(
                this IServiceCollection services,
                IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            return services;
        }
    }
}
