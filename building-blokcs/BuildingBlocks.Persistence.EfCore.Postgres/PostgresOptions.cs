using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Persistence.EfCore.Postgres
{
    public class PostgresOptions
    {
        [RegularExpression("/{{YOUR_PASSWORD}}/", ErrorMessage = $"[{nameof(PostgresOptions)}]" + "Please change {{YOUR_PASSWORD}} to your desired password from appsettings file")]
        public string ConnectionString { get; set; } = default!;
        public bool UseInMemory { get; set; }
        public string? MigrationAssembly { get; set; } = null!;
    }
}
