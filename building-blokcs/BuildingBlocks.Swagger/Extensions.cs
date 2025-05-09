using System.Reflection;
using BuildingBlocks.Core.Web.Extensions.ServiceCollection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace BuildingBlocks.Swagger
{
    public static class Extensions
    {
        public static WebApplicationBuilder AddCustomSwagger(
            this WebApplicationBuilder builder,
            params Assembly[] assemblies)
        {
            //if (!builder.Environment.IsProduction())
            builder.Services.AddCustomSwagger(assemblies);

            return builder;
        }

        public static IServiceCollection AddCustomSwagger(
            this IServiceCollection services,
            params Assembly[] assemblies)
        {
            var scanAssemblies = assemblies.Any() ? assemblies : new[] { Assembly.GetExecutingAssembly() };
            services.AddEndpointsApiExplorer();

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddValidatedOptions<SwaggerOptions>();

            services.AddSwaggerGen(options =>
            {
                options.AddEnumsWithValuesFixFilters();

                options.OperationFilter<SwaggerDefaultValues>();

                options.OperationFilter<ApiVersionOperationFilter>();

                foreach (var assembly in scanAssemblies)
                {
                    var xmlFile = XmlCommentsFilePath(assembly);
                    if (File.Exists(xmlFile))
                        options.IncludeXmlComments(xmlFile);
                }
                
                options.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.RelativePath}");
                
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                            new string[] {}
                    }
                });

                //var bearerScheme = new OpenApiSecurityScheme()
                //{
                //    Type = SecuritySchemeType.Http,
                //    Name = JwtBearerDefaults.AuthenticationScheme,
                //    Scheme = JwtBearerDefaults.AuthenticationScheme,
                //    Reference = new() { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme }
                //};

                //var apiKeyScheme = new OpenApiSecurityScheme
                //{
                //    Description = "Api key needed to access the endpoints. X-Api-Key: My_API_Key",
                //    In = ParameterLocation.Header,
                //    Name = Constants.ApiKeyConstants.HeaderName,
                //    Scheme = Constants.ApiKeyConstants.DefaultScheme,
                //    Type = SecuritySchemeType.ApiKey,
                //    Reference = new() { Type = ReferenceType.SecurityScheme, Id = Constants.ApiKeyConstants.HeaderName }
                //};

                //options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, bearerScheme);
                //options.AddSecurityDefinition(Constants.ApiKeyConstants.HeaderName, apiKeyScheme);

                //options.AddSecurityRequirement(
                //    new OpenApiSecurityRequirement
                //    {
                //    { bearerScheme, Array.Empty<string>() },
                //    { apiKeyScheme, Array.Empty<string>() }
                //    }
                //);

                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                options.EnableAnnotations();
            });

            static string XmlCommentsFilePath(Assembly assembly)
            {
                var basePath = Path.GetDirectoryName(assembly.Location);
                var fileName = assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }

            return services;
        }

        public static IApplicationBuilder UseCustomSwagger(this WebApplication app)
        {
            //if (!app.Environment.IsProduction())
            //{
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    var descriptions = app.DescribeApiVersions();
                    foreach (var description in descriptions)
                    {
                        var url = $"/swagger/{description.GroupName}/swagger.json";
                        var name = description.GroupName.ToUpperInvariant();
                        options.SwaggerEndpoint(url, name);
                    }
                });
           // }

            return app;
        }
    }
}
