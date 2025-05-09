using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Order.Infrastructure.Migrations
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
                name: "order");

            migrationBuilder.EnsureSchema(
                name: "product");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "addresses",
                schema: "address",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_address = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_addresses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "order_statuses",
                schema: "order",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    name = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                schema: "product",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "order",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    shipping_address_id = table.Column<Guid>(type: "uuid", nullable: true),
                    shipping_address = table.Column<string>(type: "text", nullable: true),
                    billing_address_id = table.Column<Guid>(type: "uuid", nullable: true),
                    billing_address = table.Column<string>(type: "text", nullable: true),
                    total_price = table.Column<decimal>(type: "numeric", maxLength: 2000, nullable: false),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    created = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    original_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_orders_order_statuses_status",
                        column: x => x.status,
                        principalSchema: "order",
                        principalTable: "order_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_items",
                schema: "order",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    product_id = table.Column<Guid>(type: "uuid", nullable: true),
                    product_name = table.Column<string>(type: "text", nullable: true),
                    unit_price = table.Column<decimal>(type: "numeric", nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    total_price = table.Column<decimal>(type: "numeric", nullable: false),
                    created = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_order_items_orders_order_id",
                        column: x => x.order_id,
                        principalSchema: "order",
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "order",
                table: "order_statuses",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "created" },
                    { 2, "paid" },
                    { 3, "cancelled" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_addresses_id",
                schema: "address",
                table: "addresses",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_order_items_id",
                schema: "order",
                table: "order_items",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_order_items_order_id",
                schema: "order",
                table: "order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_statuses_id",
                schema: "order",
                table: "order_statuses",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_orders_id",
                schema: "order",
                table: "orders",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_orders_status",
                schema: "order",
                table: "orders",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_products_id",
                schema: "product",
                table: "products",
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
                name: "order_items",
                schema: "order");

            migrationBuilder.DropTable(
                name: "products",
                schema: "product");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "order");

            migrationBuilder.DropTable(
                name: "order_statuses",
                schema: "order");
        }
    }
}
