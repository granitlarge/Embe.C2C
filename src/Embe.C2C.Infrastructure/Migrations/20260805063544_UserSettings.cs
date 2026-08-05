using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Embe.C2C.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Settings_DeviceNotifications",
                table: "DomainUsers",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Settings_EmailNotifications",
                table: "DomainUsers",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Settings_NotifyOnLike",
                table: "DomainUsers",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Settings_NotifyOnMatch",
                table: "DomainUsers",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Settings_NotifyOnMessage",
                table: "DomainUsers",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Settings_DeviceNotifications",
                table: "DomainUsers");

            migrationBuilder.DropColumn(
                name: "Settings_EmailNotifications",
                table: "DomainUsers");

            migrationBuilder.DropColumn(
                name: "Settings_NotifyOnLike",
                table: "DomainUsers");

            migrationBuilder.DropColumn(
                name: "Settings_NotifyOnMatch",
                table: "DomainUsers");

            migrationBuilder.DropColumn(
                name: "Settings_NotifyOnMessage",
                table: "DomainUsers");
        }
    }
}
