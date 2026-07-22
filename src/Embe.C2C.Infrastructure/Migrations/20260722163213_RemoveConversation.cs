using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Embe.C2C.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveConversation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversation_ConversationId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "Conversation");

            migrationBuilder.RenameColumn(
                name: "ConversationId",
                table: "Messages",
                newName: "MatchingId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages",
                newName: "IX_Messages_MatchingId");

            migrationBuilder.AddColumn<Guid>(
                name: "LastMessageId",
                table: "Matchings",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matchings_LastMessageId",
                table: "Matchings",
                column: "LastMessageId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Matchings_Messages_LastMessageId",
                table: "Matchings",
                column: "LastMessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Matchings_MatchingId",
                table: "Messages",
                column: "MatchingId",
                principalTable: "Matchings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matchings_Messages_LastMessageId",
                table: "Matchings");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Matchings_MatchingId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Matchings_LastMessageId",
                table: "Matchings");

            migrationBuilder.DropColumn(
                name: "LastMessageId",
                table: "Matchings");

            migrationBuilder.RenameColumn(
                name: "MatchingId",
                table: "Messages",
                newName: "ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_MatchingId",
                table: "Messages",
                newName: "IX_Messages_ConversationId");

            migrationBuilder.CreateTable(
                name: "Conversation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LastMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    MatchingId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageCount = table.Column<long>(type: "bigint", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UserId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId2 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversation_Matchings_MatchingId",
                        column: x => x.MatchingId,
                        principalTable: "Matchings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Conversation_Messages_LastMessageId",
                        column: x => x.LastMessageId,
                        principalTable: "Messages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_LastMessageId",
                table: "Conversation",
                column: "LastMessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_MatchingId",
                table: "Conversation",
                column: "MatchingId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Conversation_ConversationId",
                table: "Messages",
                column: "ConversationId",
                principalTable: "Conversation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
