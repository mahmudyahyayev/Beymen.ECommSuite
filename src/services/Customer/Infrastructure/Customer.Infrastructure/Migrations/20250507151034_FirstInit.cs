using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Customer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FirstInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "address");

            migrationBuilder.EnsureSchema(
                name: "customer");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "address_types",
                schema: "address",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    name = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_address_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                schema: "customer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    created = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    original_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "addresses",
                schema: "address",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    type_id = table.Column<int>(type: "integer", nullable: false),
                    country = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    city = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    district = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    street = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    zip_code = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    created = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    original_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_addresses", x => x.id);
                    table.ForeignKey(
                        name: "fk_addresses_address_types_type_id",
                        column: x => x.type_id,
                        principalSchema: "address",
                        principalTable: "address_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "address",
                table: "address_types",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "shipping" },
                    { 2, "billing" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_address_types_id",
                schema: "address",
                table: "address_types",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_addresses_id",
                schema: "address",
                table: "addresses",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_addresses_type_id",
                schema: "address",
                table: "addresses",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "ix_customers_id",
                schema: "customer",
                table: "customers",
                column: "id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "addresses",
                schema: "address");

            migrationBuilder.DropTable(
                name: "customers",
                schema: "customer");

            migrationBuilder.DropTable(
                name: "address_types",
                schema: "address");
        }
    }
}
