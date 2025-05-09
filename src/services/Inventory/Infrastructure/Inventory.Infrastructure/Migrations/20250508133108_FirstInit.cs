using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FirstInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "product");

            migrationBuilder.EnsureSchema(
                name: "rezervation");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "product_statuses",
                schema: "product",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    name = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reservation_statuses",
                schema: "rezervation",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    name = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservation_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                schema: "product",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    stock = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    created = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    original_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_products_product_statusues_status",
                        column: x => x.status,
                        principalSchema: "product",
                        principalTable: "product_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservations",
                schema: "rezervation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    created = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    original_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservations", x => x.id);
                    table.ForeignKey(
                        name: "fk_reservations_reservation_statuses_status",
                        column: x => x.status,
                        principalSchema: "rezervation",
                        principalTable: "reservation_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservation_items",
                schema: "rezervation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    reservation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservation_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_reservation_items_reservations_reservation_id",
                        column: x => x.reservation_id,
                        principalSchema: "rezervation",
                        principalTable: "reservations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "product",
                table: "product_statuses",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "available" },
                    { 2, "unavailable" },
                    { 3, "discontinued" }
                });

            migrationBuilder.InsertData(
                schema: "rezervation",
                table: "reservation_statuses",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "pending" },
                    { 2, "confirmed" },
                    { 3, "cancelled" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_product_statuses_id",
                schema: "product",
                table: "product_statuses",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_products_id",
                schema: "product",
                table: "products",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_products_status",
                schema: "product",
                table: "products",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_items_id",
                schema: "rezervation",
                table: "reservation_items",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reservation_items_reservation_id",
                schema: "rezervation",
                table: "reservation_items",
                column: "reservation_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_statuses_id",
                schema: "rezervation",
                table: "reservation_statuses",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reservations_id",
                schema: "rezervation",
                table: "reservations",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reservations_status",
                schema: "rezervation",
                table: "reservations",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "products",
                schema: "product");

            migrationBuilder.DropTable(
                name: "reservation_items",
                schema: "rezervation");

            migrationBuilder.DropTable(
                name: "product_statuses",
                schema: "product");

            migrationBuilder.DropTable(
                name: "reservations",
                schema: "rezervation");

            migrationBuilder.DropTable(
                name: "reservation_statuses",
                schema: "rezervation");
        }
    }
}
