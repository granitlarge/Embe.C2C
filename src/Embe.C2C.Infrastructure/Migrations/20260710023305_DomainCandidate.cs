using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Embe.C2C.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DomainCandidate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Judgements_DomainUsers_JudgeUserId",
                table: "Judgements");

            migrationBuilder.DropForeignKey(
                name: "FK_Judgements_DomainUsers_JudgeeUserId",
                table: "Judgements");

            migrationBuilder.DropIndex(
                name: "IX_Judgements_JudgeeUserId",
                table: "Judgements");

            migrationBuilder.DropIndex(
                name: "IX_Judgements_JudgeUserId",
                table: "Judgements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Candidates",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "JudgeUserId",
                table: "Judgements");

            migrationBuilder.RenameColumn(
                name: "JudgeeUserId",
                table: "Judgements",
                newName: "CandidateId");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1SearchProfileId",
                table: "Matchings",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId2SearchProfileId",
                table: "Matchings",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Candidates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CandidateSearchProfileId",
                table: "Candidates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserSearchProfileId",
                table: "Candidates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Candidates",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Candidates",
                table: "Candidates",
                column: "Id");

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
                name: "IX_Matchings_UserId1SearchProfileId",
                table: "Matchings",
                column: "UserId1SearchProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Matchings_UserId2SearchProfileId",
                table: "Matchings",
                column: "UserId2SearchProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Judgements_CandidateId",
                table: "Judgements",
                column: "CandidateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_CandidateSearchProfileId",
                table: "Candidates",
                column: "CandidateSearchProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_UserId_CandidateUserId_UserSearchProfileId_Candi~",
                table: "Candidates",
                columns: new[] { "UserId", "CandidateUserId", "UserSearchProfileId", "CandidateSearchProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_UserSearchProfileId",
                table: "Candidates",
                column: "UserSearchProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchProfileSearchProfile_UserSearchProfilesId",
                table: "SearchProfileSearchProfile",
                column: "UserSearchProfilesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_SearchProfiles_CandidateSearchProfileId",
                table: "Candidates",
                column: "CandidateSearchProfileId",
                principalTable: "SearchProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_SearchProfiles_UserSearchProfileId",
                table: "Candidates",
                column: "UserSearchProfileId",
                principalTable: "SearchProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Judgements_Candidates_CandidateId",
                table: "Judgements",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_SearchProfiles_CandidateSearchProfileId",
                table: "Candidates");

            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_SearchProfiles_UserSearchProfileId",
                table: "Candidates");

            migrationBuilder.DropForeignKey(
                name: "FK_Judgements_Candidates_CandidateId",
                table: "Judgements");

            migrationBuilder.DropForeignKey(
                name: "FK_Matchings_SearchProfiles_UserId1SearchProfileId",
                table: "Matchings");

            migrationBuilder.DropForeignKey(
                name: "FK_Matchings_SearchProfiles_UserId2SearchProfileId",
                table: "Matchings");

            migrationBuilder.DropTable(
                name: "SearchProfileSearchProfile");

            migrationBuilder.DropIndex(
                name: "IX_Matchings_UserId1SearchProfileId",
                table: "Matchings");

            migrationBuilder.DropIndex(
                name: "IX_Matchings_UserId2SearchProfileId",
                table: "Matchings");

            migrationBuilder.DropIndex(
                name: "IX_Judgements_CandidateId",
                table: "Judgements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Candidates",
                table: "Candidates");

            migrationBuilder.DropIndex(
                name: "IX_Candidates_CandidateSearchProfileId",
                table: "Candidates");

            migrationBuilder.DropIndex(
                name: "IX_Candidates_UserId_CandidateUserId_UserSearchProfileId_Candi~",
                table: "Candidates");

            migrationBuilder.DropIndex(
                name: "IX_Candidates_UserSearchProfileId",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "UserId1SearchProfileId",
                table: "Matchings");

            migrationBuilder.DropColumn(
                name: "UserId2SearchProfileId",
                table: "Matchings");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "CandidateSearchProfileId",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "UserSearchProfileId",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Candidates");

            migrationBuilder.RenameColumn(
                name: "CandidateId",
                table: "Judgements",
                newName: "JudgeeUserId");

            migrationBuilder.AddColumn<Guid>(
                name: "JudgeUserId",
                table: "Judgements",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Candidates",
                table: "Candidates",
                columns: new[] { "UserId", "CandidateUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Judgements_JudgeeUserId",
                table: "Judgements",
                column: "JudgeeUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Judgements_JudgeUserId",
                table: "Judgements",
                column: "JudgeUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Judgements_DomainUsers_JudgeUserId",
                table: "Judgements",
                column: "JudgeUserId",
                principalTable: "DomainUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Judgements_DomainUsers_JudgeeUserId",
                table: "Judgements",
                column: "JudgeeUserId",
                principalTable: "DomainUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
