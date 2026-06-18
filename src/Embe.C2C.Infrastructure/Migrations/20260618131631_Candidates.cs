using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Embe.C2C.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Candidates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidates",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidates", x => new { x.UserId, x.CandidateUserId });
                    table.ForeignKey(
                        name: "FK_Candidates_DomainUsers_CandidateUserId",
                        column: x => x.CandidateUserId,
                        principalTable: "DomainUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Candidates_DomainUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "DomainUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_CandidateUserId",
                table: "Candidates",
                column: "CandidateUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidates");
        }
    }
}
