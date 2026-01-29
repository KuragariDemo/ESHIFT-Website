using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShift.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedJobAndLicenseToCar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarNo",
                table: "Cars");

            migrationBuilder.RenameColumn(
                name: "Job",
                table: "Cars",
                newName: "CarLicenseNo");

            migrationBuilder.AddColumn<string>(
                name: "AssignedJobId",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedJobId",
                table: "Cars");

            migrationBuilder.RenameColumn(
                name: "CarLicenseNo",
                table: "Cars",
                newName: "Job");

            migrationBuilder.AddColumn<string>(
                name: "CarNo",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
