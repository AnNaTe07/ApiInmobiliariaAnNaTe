using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiInmobiliariaAnNaTe.Migrations
{
    /// <inheritdoc />
    public partial class CorreccionDecimal3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Superficie",
                table: "Inmuebles",
                type: "decimal(6,1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "Inmuebles",
                type: "decimal(9,1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,0)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Superficie",
                table: "Inmuebles",
                type: "decimal(5,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,1)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "Inmuebles",
                type: "decimal(8,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,1)");
        }
    }
}
