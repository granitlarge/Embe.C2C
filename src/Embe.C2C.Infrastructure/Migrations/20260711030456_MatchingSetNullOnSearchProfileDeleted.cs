using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Embe.C2C.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MatchingSetNullOnSearchProfileDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matchings_SearchProfiles_UserId1SearchProfileId",
                table: "Matchings");

            migrationBuilder.DropForeignKey(
                name: "FK_Matchings_SearchProfiles_UserId2SearchProfileId",
                table: "Matchings");

            migrationBuilder.AddForeignKey(
                name: "FK_Matchings_SearchProfiles_UserId1SearchProfileId",
                table: "Matchings",
                column: "UserId1SearchProfileId",
                principalTable: "SearchProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Matchings_SearchProfiles_UserId2SearchProfileId",
                table: "Matchings",
                column: "UserId2SearchProfileId",
                principalTable: "SearchProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matchings_SearchProfiles_UserId1SearchProfileId",
                table: "Matchings");

            migrationBuilder.DropForeignKey(
                name: "FK_Matchings_SearchProfiles_UserId2SearchProfileId",
                table: "Matchings");

            migrationBuilder.AddForeignKey(
                name: "FK_Matchings_SearchProfiles_UserId1SearchProfileId",
                table: "Matchings",
                column: "UserId1SearchProfileId",
                principalTable: "SearchProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matchings_SearchProfiles_UserId2SearchProfileId",
                table: "Matchings",
                column: "UserId2SearchProfileId",
                principalTable: "SearchProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
