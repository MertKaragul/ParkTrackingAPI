using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkTracking.Migrations
{
    /// <inheritdoc />
    public partial class mig7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Park");

            migrationBuilder.AddColumn<string>(
                name: "UserIdenty",
                table: "Park",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserIdenty",
                table: "Park");

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Park",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
