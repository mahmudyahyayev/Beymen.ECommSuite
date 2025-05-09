using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Notification.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FirstInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "customer");

            migrationBuilder.EnsureSchema(
                name: "notification");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "customers",
                schema: "customer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification_statuses",
                schema: "notification",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    name = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification_types",
                schema: "notification",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    name = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                schema: "notification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    recipient = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    created = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    original_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_notifications_notification_statuses_status",
                        column: x => x.status,
                        principalSchema: "notification",
                        principalTable: "notification_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_notifications_notification_types_type_id",
                        column: x => x.type_id,
                        principalSchema: "notification",
                        principalTable: "notification_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "notification",
                table: "notification_statuses",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "pending" },
                    { 2, "sent" },
                    { 3, "failed" }
                });

            migrationBuilder.InsertData(
                schema: "notification",
                table: "notification_types",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "sms" },
                    { 2, "email" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_customers_id",
                schema: "customer",
                table: "customers",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notification_statuses_id",
                schema: "notification",
                table: "notification_statuses",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notification_types_id",
                schema: "notification",
                table: "notification_types",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notifications_id",
                schema: "notification",
                table: "notifications",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notifications_status",
                schema: "notification",
                table: "notifications",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_type_id",
                schema: "notification",
                table: "notifications",
                column: "type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customers",
                schema: "customer");

            migrationBuilder.DropTable(
                name: "notifications",
                schema: "notification");

            migrationBuilder.DropTable(
                name: "notification_statuses",
                schema: "notification");

            migrationBuilder.DropTable(
                name: "notification_types",
                schema: "notification");
        }
    }
}
