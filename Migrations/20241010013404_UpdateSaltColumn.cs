using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiInmobiliariaAnNaTe.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSaltColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Salt",
                table: "Propietarios",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salt",
                table: "Propietarios");
        }
    }
}
