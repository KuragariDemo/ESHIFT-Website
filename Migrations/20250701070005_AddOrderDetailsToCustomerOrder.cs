using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShift.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderDetailsToCustomerOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssistantsName",
                table: "CustomerOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarModel",
                table: "CustomerOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarNumber",
                table: "CustomerOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverName",
                table: "CustomerOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledDateTime",
                table: "CustomerOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CustomerOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssistantsName",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "CarModel",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "CarNumber",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "DriverName",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "ScheduledDateTime",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CustomerOrders");
        }
    }
}
