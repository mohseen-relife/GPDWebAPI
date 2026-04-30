using BI;
using Contracts.Managers;
using Contracts.Repositories;
using DAL;
using DAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace GpdWebApi.Extensions
{
    /// <summary>
    /// Extension methods to keep Program.cs clean.
    /// Registers: DbContext, Repositories, Managers, JWT Auth, Swagger.
    /// </summary>
    public static class ServiceExtensions
    {
        // ── Database ──────────────────────────────────────────────────────────
        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration config)
        {
            var connStr = config.GetConnectionString("TandeefDB")
                ?? throw new InvalidOperationException(
                    "Connection string 'TandeefDB' not found in appsettings.json.");

            services.AddDbContext<GpdDbContext>(options =>
                options.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));

            return services;
        }

        // ── Dependency Injection — Repositories & Managers ───────────────────
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            // Repositories (DATA layer)
            services.AddScoped<IBinCollectionRepository, BinCollectionRepository>();
            services.AddScoped<IUserRepository,          UserRepository>();

            // Managers (BI layer)
            services.AddScoped<IBinCollectionManager, BinCollectionManager>();
            services.AddScoped<IAuthManager,          AuthManager>();

            return services;
        }

        // ── JWT Bearer Authentication ─────────────────────────────────────────
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration config)
        {
            var jwt       = config.GetSection("JwtSettings");
            var secretKey = jwt["SecretKey"]
                ?? throw new InvalidOperationException("JwtSettings:SecretKey missing.");

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer           = true,
                        ValidateAudience         = true,
                        ValidateLifetime         = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer              = jwt["Issuer"],
                        ValidAudience            = jwt["Audience"],
                        IssuerSigningKey         = new SymmetricSecurityKey(
                                                       Encoding.UTF8.GetBytes(secretKey)),
                        ClockSkew                = TimeSpan.Zero   // no tolerance
                    };
                });

            return services;
        }

        // ── Swagger with Bearer token input ──────────────────────────────────
        public static IServiceCollection AddSwaggerWithJwt(
            this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title   = "GPD Web API",
                    Version = "v1",
                    Description = "Tandeef Bin Collection API with JWT Authentication"
                });

                // Add Bearer input box to Swagger UI
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name         = "Authorization",
                    Type         = SecuritySchemeType.Http,
                    Scheme       = "Bearer",
                    BearerFormat = "JWT",
                    In           = ParameterLocation.Header,
                    Description  = "Enter your JWT token. Example: eyJhbGci..."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id   = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }
    }
}
