using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiInmobiliariaAnNaTe.Migrations
{
    /// <inheritdoc />
    public partial class NombreDeLaMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inmuebles_Propietarios_PropietarioInmuebleId",
                table: "Inmuebles");

            migrationBuilder.DropIndex(
                name: "IX_Inmuebles_PropietarioInmuebleId",
                table: "Inmuebles");

            migrationBuilder.DropColumn(
                name: "PropietarioInmuebleId",
                table: "Inmuebles");

            migrationBuilder.CreateIndex(
                name: "IX_Inmuebles_IdPropietario",
                table: "Inmuebles",
                column: "IdPropietario");

            migrationBuilder.AddForeignKey(
                name: "FK_Inmuebles_Propietarios_IdPropietario",
                table: "Inmuebles",
                column: "IdPropietario",
                principalTable: "Propietarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inmuebles_Propietarios_IdPropietario",
                table: "Inmuebles");

            migrationBuilder.DropIndex(
                name: "IX_Inmuebles_IdPropietario",
                table: "Inmuebles");

            migrationBuilder.AddColumn<int>(
                name: "PropietarioInmuebleId",
                table: "Inmuebles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inmuebles_PropietarioInmuebleId",
                table: "Inmuebles",
                column: "PropietarioInmuebleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inmuebles_Propietarios_PropietarioInmuebleId",
                table: "Inmuebles",
                column: "PropietarioInmuebleId",
                principalTable: "Propietarios",
                principalColumn: "Id");
        }
    }
}
