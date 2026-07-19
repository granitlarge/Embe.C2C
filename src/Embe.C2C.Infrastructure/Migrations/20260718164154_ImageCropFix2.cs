using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Embe.C2C.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImageCropFix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageDetails_CropOffset_Y",
                table: "Image",
                newName: "ImageDetails_CropOffsetY");

            migrationBuilder.RenameColumn(
                name: "ImageDetails_CropOffset_X",
                table: "Image",
                newName: "ImageDetails_CropOffsetX");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageDetails_CropOffsetY",
                table: "Image",
                newName: "ImageDetails_CropOffset_Y");

            migrationBuilder.RenameColumn(
                name: "ImageDetails_CropOffsetX",
                table: "Image",
                newName: "ImageDetails_CropOffset_X");
        }
    }
}
