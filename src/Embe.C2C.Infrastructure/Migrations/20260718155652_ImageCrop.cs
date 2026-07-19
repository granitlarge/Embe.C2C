using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Embe.C2C.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImageCrop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageDetails_CropOffset_X",
                table: "Image",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ImageDetails_CropOffset_Y",
                table: "Image",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageDetails_CropOffset_X",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "ImageDetails_CropOffset_Y",
                table: "Image");
        }
    }
}
