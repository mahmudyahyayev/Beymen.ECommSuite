using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingBlocks.Messaging.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMessageStoreCorrelationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "correlation_id",
                schema: "messaging",
                table: "store_messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "correlation_id",
                schema: "messaging",
                table: "store_messages",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
