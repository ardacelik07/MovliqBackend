using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunningApplicationNew.Migrations
{
    /// <inheritdoc />
    public partial class doubleconvertedtoInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OeneralDistance",
                table: "LeaderBoards");

            migrationBuilder.AlterColumn<int>(
                name: "steps",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OutdoorSteps",
                table: "LeaderBoards",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "IndoorSteps",
                table: "LeaderBoards",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "GeneralDistance",
                table: "LeaderBoards",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeneralDistance",
                table: "LeaderBoards");

            migrationBuilder.AlterColumn<double>(
                name: "steps",
                table: "Users",
                type: "float",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OutdoorSteps",
                table: "LeaderBoards",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IndoorSteps",
                table: "LeaderBoards",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OeneralDistance",
                table: "LeaderBoards",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
