using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShift.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdAndScheduledDateTimeToAssignedJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AssignedJobs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledDateTime",
                table: "AssignedJobs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 1, 1)); // Change default if needed

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedJobs_CustomerOrders_OrderId",
                table: "AssignedJobs",
                column: "OrderId",
                principalTable: "CustomerOrders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedJobs_CustomerOrders_OrderId",
                table: "AssignedJobs");

            migrationBuilder.DropColumn(
                name: "ScheduledDateTime",
                table: "AssignedJobs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AssignedJobs");
        }
    }
}
