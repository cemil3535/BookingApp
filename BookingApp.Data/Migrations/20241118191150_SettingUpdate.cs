using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SettingUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaintennenceMode",
                table: "Settings",
                newName: "MaintenenceMode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaintenenceMode",
                table: "Settings",
                newName: "MaintennenceMode");
        }
    }
}
