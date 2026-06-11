using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Embe.C2C.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SnapshotIsolation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            ALTER DATABASE CURRENT
            SET ALLOW_SNAPSHOT_ISOLATION ON;
            ", suppressTransaction: true);

            migrationBuilder.Sql(@"
            ALTER DATABASE CURRENT
            SET READ_COMMITTED_SNAPSHOT ON;
            ", suppressTransaction: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            ALTER DATABASE CURRENT
            SET READ_COMMITTED_SNAPSHOT OFF;
            ", suppressTransaction: true);

            migrationBuilder.Sql(@"
            ALTER DATABASE CURRENT
            SET ALLOW_SNAPSHOT_ISOLATION OFF;
            ", suppressTransaction: true);
        }
    }
}
