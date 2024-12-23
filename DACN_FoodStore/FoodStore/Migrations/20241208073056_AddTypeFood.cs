using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodStore.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeFood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeFood",
                table: "Foods",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypeFood",
                table: "Foods");
        }
    }
}
