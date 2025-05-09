using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingFullAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "apartment_no",
                schema: "address",
                table: "addresses",
                type: "character varying(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "building_no",
                schema: "address",
                table: "addresses",
                type: "character varying(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "address",
                table: "addresses",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "floor",
                schema: "address",
                table: "addresses",
                type: "character varying(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "apartment_no",
                schema: "address",
                table: "addresses");

            migrationBuilder.DropColumn(
                name: "building_no",
                schema: "address",
                table: "addresses");

            migrationBuilder.DropColumn(
                name: "description",
                schema: "address",
                table: "addresses");

            migrationBuilder.DropColumn(
                name: "floor",
                schema: "address",
                table: "addresses");
        }
    }
}
