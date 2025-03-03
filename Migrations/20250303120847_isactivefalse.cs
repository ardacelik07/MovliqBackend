using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunningApplicationNew.Migrations
{
    /// <inheritdoc />
    public partial class isactivefalse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "RaceRooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "RaceRooms");
        }
    }
}
