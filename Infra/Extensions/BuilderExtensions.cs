using Infra.Domain.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetDevPack.Security.Jwt.Store.EntityFrameworkCore;
using NetDevPack.Security.JwtExtensions;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;
using System.Reflection;
using System.Text;

namespace Infra.Extensions
{
    public static class BuilderExtensions
    {
        public static void ConfigSerilog(this WebApplicationBuilder @this)
        {
            var messageTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}";
            var appName = Assembly.GetExecutingAssembly().GetName().Name;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithCorrelationId()
                .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager"))
                .Filter.ByExcluding(Matching.FromSource("Microsoft.Hosting.Lifetime"))
                .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
                .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker"))
                .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Routing.EndpointMiddleware"))
                .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Hosting.Diagnostics"))
                .WriteTo.Async(wt => wt.Console(outputTemplate: messageTemplate))
                .WriteTo.Async(wt => wt.File($"logs/log-{appName}-.txt", rollingInterval: RollingInterval.Day, outputTemplate: messageTemplate))
                .CreateLogger();

            @this.Host.UseSerilog(Log.Logger);
        }

        public static void ConfigAuth(this WebApplicationBuilder @this)
        {
            var jwtOptions = @this.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
            @this.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opts =>
                {
                    byte[] privateKey = Encoding.UTF8.GetBytes(jwtOptions?.PrivateKey ?? "");

                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(privateKey)
                    };
                });
            @this.Services.AddAuthorization();
        }

        public static void ConfigAuthJwks(this WebApplicationBuilder @this)
        {
            var jwtOptions = @this.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
            @this.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opts =>
                {
                    opts.RequireHttpsMetadata = true;
                    opts.SaveToken = true; // keep the public key at Cache for 10 min.
                    opts.IncludeErrorDetails = true; // <- great for debugging
                    // Extension to define how validate jwt, override TokenValidationParameters
                    opts.SetJwksOptions(new JwkOptions(jwtOptions.IssuerUrl));
                });
            @this.Services.AddAuthorization();
        }

        public static void ConfigContextForPostgreSQL<T>(this WebApplicationBuilder builder) where T : DbContext
        {
            var database = builder.Configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>();
            builder.Services.AddDbContext<T>(options =>
                options.UseNpgsql(database.ConnectionString, cfg => cfg.EnableRetryOnFailure()));
        }

        public static void ConfigJwks<T>(this WebApplicationBuilder builder) where T : DbContext, ISecurityKeyContext
        {
            builder.Services.AddMemoryCache();
            builder.Services
                .AddJwksManager() // <- Add JWKS service
                .UseJwtValidation() // <- This will instruct ASP.NET to validate the JWT token using JwksManager component
                .PersistKeysToDatabaseStore<T>(); // <- Save JWKS keys on DB using EF Core
        }

        public static void ConfigSwaggerBearer(this WebApplicationBuilder builder)
        {
            var swaggerOptions = builder.Configuration.GetSection(SwaggerOptions.SectionName).Get<SwaggerOptions>();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(swaggerOptions.Name, new OpenApiInfo { Title = swaggerOptions.Title, Version = swaggerOptions.Version });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = swaggerOptions.DescriptionAuth,
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                });

                //var xmlFilenameProj = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //if (!string.IsNullOrEmpty(xmlFilenameProj))
                //    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilenameProj));

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }
    }
}
