using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Web.Extensions
{
    public static class AuthenticationExtensions
    {
        public static WebApplicationBuilder AddJwtAuthentication(this WebApplicationBuilder builder, JwtOptions jwtOptions)
        {
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });
            builder.Services.AddAuthorizationBuilder();

            return builder;
        }

        public static WebApplicationBuilder AddJwtAuthenticationV2(this WebApplicationBuilder builder, JwtOptions jwtOptions)
        {
            IdentityModelEventSource.ShowPII = true;
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = jwtOptions.Issuer;
                    options.Audience = jwtOptions.Audience;
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwtOptions!.Issuer,
                        ValidAudience = jwtOptions?.Audience,
                        ValidateIssuer = true,
                        ValidateAudience = true
                    };

                    options.Events = new()
                    {
                        OnTokenValidated = async context =>
                        {
                            if (context.Principal?.Identity is ClaimsIdentity claimsIdentity)
                            {
                                Claim? scopeClaim = claimsIdentity.FindFirst("scope");
                                if (scopeClaim is not null)
                                {
                                    claimsIdentity.RemoveClaim(scopeClaim);
                                    claimsIdentity.AddClaims(scopeClaim.Value.Split(" ").Select(s => new Claim("scope", s)).ToList());
                                }
                            }

                            await Task.CompletedTask;
                        },

                        OnAuthenticationFailed = context =>
                        {

                            return Task.CompletedTask;
                        },
                    };
                });
        
            return builder;
        }
    }
}
