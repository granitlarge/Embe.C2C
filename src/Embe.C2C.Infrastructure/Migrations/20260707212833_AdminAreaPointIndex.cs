using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Embe.C2C.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdminAreaPointIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.Sql("""
                CREATE SPATIAL INDEX IX_AdminAreas_Point
                ON dbo.AdminAreas(Point);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP INDEX IX_AdminAreas_Point
                ON dbo.AdminAreas;
                """);
        }
    }
}
