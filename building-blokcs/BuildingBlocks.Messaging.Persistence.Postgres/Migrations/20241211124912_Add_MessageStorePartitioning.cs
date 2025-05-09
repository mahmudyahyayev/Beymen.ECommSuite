using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingBlocks.Messaging.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Add_MessageStorePartitioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "key",
                schema: "messaging",
                table: "store_messages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "modified",
                schema: "messaging",
                table: "store_messages",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "partition",
                schema: "messaging",
                table: "store_messages",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "priority",
                schema: "messaging",
                table: "store_messages",
                type: "integer",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "pod_partition",
                schema: "messaging",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pod_id = table.Column<Guid>(type: "uuid", nullable: false),
                    partition = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pod_partition", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "running_pods",
                schema: "messaging",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pod_name = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true),
                    last_hearth_beat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_running_pods", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pod_partition_partition",
                schema: "messaging",
                table: "pod_partition",
                column: "partition");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pod_partition",
                schema: "messaging");

            migrationBuilder.DropTable(
                name: "running_pods",
                schema: "messaging");

            migrationBuilder.DropColumn(
                name: "key",
                schema: "messaging",
                table: "store_messages");

            migrationBuilder.DropColumn(
                name: "modified",
                schema: "messaging",
                table: "store_messages");

            migrationBuilder.DropColumn(
                name: "partition",
                schema: "messaging",
                table: "store_messages");

            migrationBuilder.DropColumn(
                name: "priority",
                schema: "messaging",
                table: "store_messages");
        }
    }
}
