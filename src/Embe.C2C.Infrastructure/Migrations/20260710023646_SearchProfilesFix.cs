using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Embe.C2C.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SearchProfilesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SearchProfileSearchProfile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SearchProfileSearchProfile",
                columns: table => new
                {
                    CandidateSearchProfilesId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserSearchProfilesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchProfileSearchProfile", x => new { x.CandidateSearchProfilesId, x.UserSearchProfilesId });
                    table.ForeignKey(
                        name: "FK_SearchProfileSearchProfile_SearchProfiles_CandidateSearchPr~",
                        column: x => x.CandidateSearchProfilesId,
                        principalTable: "SearchProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SearchProfileSearchProfile_SearchProfiles_UserSearchProfile~",
                        column: x => x.UserSearchProfilesId,
                        principalTable: "SearchProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SearchProfileSearchProfile_UserSearchProfilesId",
                table: "SearchProfileSearchProfile",
                column: "UserSearchProfilesId");
        }
    }
}
