using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Embe.C2C.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RevertGarbage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageDetails_CropOffsetX",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "ImageDetails_CropOffsetY",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "ImageDetails_Status",
                table: "Image");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ImageDetails_CropOffsetX",
                table: "Image",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ImageDetails_CropOffsetY",
                table: "Image",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ImageDetails_Status",
                table: "Image",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
