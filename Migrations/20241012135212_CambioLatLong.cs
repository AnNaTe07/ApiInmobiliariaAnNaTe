using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiInmobiliariaAnNaTe.Migrations
{
    /// <inheritdoc />
    public partial class CambioLatLong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitud",
                table: "Inmuebles",
                type: "decimal(12,7)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitud",
                table: "Inmuebles",
                type: "decimal(12,7)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,8)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitud",
                table: "Inmuebles",
                type: "decimal(12,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,7)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitud",
                table: "Inmuebles",
                type: "decimal(12,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,7)");
        }
    }
}
