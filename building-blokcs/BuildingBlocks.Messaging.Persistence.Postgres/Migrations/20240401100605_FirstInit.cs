using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingBlocks.Messaging.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class FirstInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "messaging");

            migrationBuilder.CreateTable(
                name: "store_messages",
                schema: "messaging",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_type = table.Column<string>(type: "text", nullable: false),
                    data = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    retry_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    message_status = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    delivery_type = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    last_error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    instance_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_store_messages", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "store_messages",
                schema: "messaging");
        }
    }
}
