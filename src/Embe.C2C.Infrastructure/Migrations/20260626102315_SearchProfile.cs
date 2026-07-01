using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Embe.C2C.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SearchProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatingPreferences_AgeRangeMax",
                table: "DomainUsers");

            migrationBuilder.DropColumn(
                name: "DatingPreferences_AgeRangeMin",
                table: "DomainUsers");

            migrationBuilder.DropColumn(
                name: "DatingPreferences_InterestedInGenders",
                table: "DomainUsers");

            migrationBuilder.DropColumn(
                name: "DatingPreferences_MaximumDistance",
                table: "DomainUsers");

            migrationBuilder.CreateTable(
                name: "SearchProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgeRangeMin = table.Column<int>(type: "int", nullable: true),
                    AgeRangeMax = table.Column<int>(type: "int", nullable: true),
                    MaximumDistance = table.Column<double>(type: "float", nullable: true),
                    Engagement_Boundedness = table.Column<int>(type: "int", nullable: false),
                    Engagement_EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Engagement_Frequency = table.Column<int>(type: "int", nullable: false),
                    Engagement_Medium = table.Column<int>(type: "int", nullable: false),
                    Engagement_StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchProfileGender",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SearchProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchProfileGender", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchProfileGender_SearchProfiles_SearchProfileId",
                        column: x => x.SearchProfileId,
                        principalTable: "SearchProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SearchProfileGender_SearchProfileId",
                table: "SearchProfileGender",
                column: "SearchProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SearchProfileGender");

            migrationBuilder.DropTable(
                name: "SearchProfiles");

            migrationBuilder.AddColumn<int>(
                name: "DatingPreferences_AgeRangeMax",
                table: "DomainUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DatingPreferences_AgeRangeMin",
                table: "DomainUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DatingPreferences_InterestedInGenders",
                table: "DomainUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "DatingPreferences_MaximumDistance",
                table: "DomainUsers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
