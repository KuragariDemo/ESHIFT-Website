using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShift.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToCustomerOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CustomerOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CustomerOrders");
        }
    }
}
