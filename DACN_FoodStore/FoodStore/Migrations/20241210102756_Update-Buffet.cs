using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodStore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBuffet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Buffets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buffets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BuffetDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuffetId = table.Column<int>(type: "int", nullable: false),
                    FoodId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuffetDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuffetDetails_Buffets_BuffetId",
                        column: x => x.BuffetId,
                        principalTable: "Buffets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuffetDetails_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuffetDetails_BuffetId",
                table: "BuffetDetails",
                column: "BuffetId");

            migrationBuilder.CreateIndex(
                name: "IX_BuffetDetails_FoodId",
                table: "BuffetDetails",
                column: "FoodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuffetDetails");

            migrationBuilder.DropTable(
                name: "Buffets");
        }
    }
}
