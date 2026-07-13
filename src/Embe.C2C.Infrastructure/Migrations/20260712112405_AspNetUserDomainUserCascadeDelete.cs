using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Embe.C2C.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AspNetUserDomainUserCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DomainUsers_AspNetUsers_IdentityUserId",
                table: "DomainUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_DomainUsers_AspNetUsers_IdentityUserId",
                table: "DomainUsers",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DomainUsers_AspNetUsers_IdentityUserId",
                table: "DomainUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_DomainUsers_AspNetUsers_IdentityUserId",
                table: "DomainUsers",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
