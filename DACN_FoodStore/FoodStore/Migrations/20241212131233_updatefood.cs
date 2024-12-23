using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodStore.Migrations
{
    /// <inheritdoc />
    public partial class updatefood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypeFood",
                table: "Foods");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeFood",
                table: "Foods",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
