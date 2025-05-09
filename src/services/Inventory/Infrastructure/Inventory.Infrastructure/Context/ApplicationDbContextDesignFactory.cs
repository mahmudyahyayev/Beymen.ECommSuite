using BuildingBlocks.Persistence.EfCore.Postgres;

namespace Inventory.Infrastructure.Context;
public class ApplicationDbContextDesignFactory : DbContextDesignFactoryBase<ApplicationDbContext>
{
    public ApplicationDbContextDesignFactory()
        : base(connectionStringSection: "PostgresOptions:ConnectionString",
               env: null,
               migrationDefaultSchema: ApplicationDbContext.DefaultSchema)
    {

    }
}

