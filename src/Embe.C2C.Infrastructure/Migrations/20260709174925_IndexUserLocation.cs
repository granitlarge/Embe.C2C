using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Embe.C2C.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IndexUserLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DomainUsers_Location",
                table: "DomainUsers",
                column: "Location")
                .Annotation("Npgsql:IndexMethod", "GIST");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DomainUsers_Location",
                table: "DomainUsers");
        }
    }
}
