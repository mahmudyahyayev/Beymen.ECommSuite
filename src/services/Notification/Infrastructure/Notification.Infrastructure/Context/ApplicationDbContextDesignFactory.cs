using BuildingBlocks.Persistence.EfCore.Postgres;

namespace Notification.Infrastructure.Context;
public class ApplicationDbContextDesignFactory : DbContextDesignFactoryBase<ApplicationDbContext>
{
    public ApplicationDbContextDesignFactory()
        : base(connectionStringSection: "PostgresOptions:ConnectionString",
               env: null,
               migrationDefaultSchema: ApplicationDbContext.DefaultSchema)
    {

    }
}