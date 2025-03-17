using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunningApplicationNew.Migrations
{
    /// <inheritdoc />
    public partial class UserResultColumnsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GeneralRank",
                table: "UserResults",
                newName: "avarageSpeed");

            migrationBuilder.AddColumn<int>(
                name: "Calories",
                table: "UserResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "UserResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "UserResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserResults",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Calories",
                table: "UserResults");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "UserResults");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "UserResults");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserResults");

            migrationBuilder.RenameColumn(
                name: "avarageSpeed",
                table: "UserResults",
                newName: "GeneralRank");
        }
    }
}
