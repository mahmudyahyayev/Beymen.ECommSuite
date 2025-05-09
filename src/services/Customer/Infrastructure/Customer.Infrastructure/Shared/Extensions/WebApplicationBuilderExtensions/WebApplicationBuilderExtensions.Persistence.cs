using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Persistence.EfCore.Postgres;
using Customer.Domain;
using Customer.Infrastructure.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Infrastructure.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddStorage(this WebApplicationBuilder builder)
    {
        AddPostgresWriteStorage(builder.Services, builder.Configuration);
        AddMongoReadStorage(builder.Services, builder.Configuration);
        AddElasticSearchStorage(builder.Services, builder.Configuration);
        return builder;
    }

    private static void AddPostgresWriteStorage(IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("PostgresOptions:UseInMemory"))
        {
            services
                .AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("Customer"));

            services
                .AddScoped<IDbFacadeResolver>(provider => provider.GetService<ApplicationDbContext>()!);
        }
        else
        {
            services
                .AddPostgresDbContext<ApplicationDbContext, long , string>();
        }

        services
            .AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
    }

    private static void AddElasticSearchStorage(IServiceCollection services, IConfiguration configuration)
    {
        //For Cqrs
    }
    private static void AddMongoReadStorage(IServiceCollection services, IConfiguration configuration)
    {
        //For Cqrs
    }
}
