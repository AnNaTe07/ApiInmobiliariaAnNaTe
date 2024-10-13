using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiInmobiliariaAnNaTe.Migrations
{
    /// <inheritdoc />
    public partial class borrarDatos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_Propietarios_PropId",
                table: "Contratos");

            migrationBuilder.DropIndex(
                name: "IX_Contratos_PropId",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "TipoDescripcion",
                table: "Inmuebles");

            migrationBuilder.DropColumn(
                name: "PropId",
                table: "Contratos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoDescripcion",
                table: "Inmuebles",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "PropId",
                table: "Contratos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_PropId",
                table: "Contratos",
                column: "PropId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_Propietarios_PropId",
                table: "Contratos",
                column: "PropId",
                principalTable: "Propietarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
