using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Embe.C2C.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BlockingsAndMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blocking_DomainUsers_BlockedUserId",
                table: "Blocking");

            migrationBuilder.DropForeignKey(
                name: "FK_Blocking_DomainUsers_BlockerUserId",
                table: "Blocking");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Blocking",
                table: "Blocking");

            migrationBuilder.RenameTable(
                name: "Blocking",
                newName: "Blockings");

            migrationBuilder.RenameIndex(
                name: "IX_Blocking_BlockerUserId",
                table: "Blockings",
                newName: "IX_Blockings_BlockerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Blocking_BlockedUserId",
                table: "Blockings",
                newName: "IX_Blockings_BlockedUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blockings",
                table: "Blockings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Blockings_DomainUsers_BlockedUserId",
                table: "Blockings",
                column: "BlockedUserId",
                principalTable: "DomainUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Blockings_DomainUsers_BlockerUserId",
                table: "Blockings",
                column: "BlockerUserId",
                principalTable: "DomainUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blockings_DomainUsers_BlockedUserId",
                table: "Blockings");

            migrationBuilder.DropForeignKey(
                name: "FK_Blockings_DomainUsers_BlockerUserId",
                table: "Blockings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Blockings",
                table: "Blockings");

            migrationBuilder.RenameTable(
                name: "Blockings",
                newName: "Blocking");

            migrationBuilder.RenameIndex(
                name: "IX_Blockings_BlockerUserId",
                table: "Blocking",
                newName: "IX_Blocking_BlockerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Blockings_BlockedUserId",
                table: "Blocking",
                newName: "IX_Blocking_BlockedUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blocking",
                table: "Blocking",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Blocking_DomainUsers_BlockedUserId",
                table: "Blocking",
                column: "BlockedUserId",
                principalTable: "DomainUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Blocking_DomainUsers_BlockerUserId",
                table: "Blocking",
                column: "BlockerUserId",
                principalTable: "DomainUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
