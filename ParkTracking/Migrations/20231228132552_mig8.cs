using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkTracking.Migrations
{
    /// <inheritdoc />
    public partial class mig8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Park");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Park",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
