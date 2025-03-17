using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunningApplicationNew.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaderBoardUserOneToOneRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeaderBoards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OeneralDistance = table.Column<double>(type: "float", nullable: false),
                    OutdoorSteps = table.Column<int>(type: "int", nullable: false),
                    IndoorSteps = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderBoards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaderBoards_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeaderBoards_UserId",
                table: "LeaderBoards",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaderBoards");
        }
    }
}
