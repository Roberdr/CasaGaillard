using CasaGaillard.Core.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaGaillard.Core.EFCoreMigrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261021180000_AddCubaPhotoObservations")]
public sealed class AddCubaPhotoObservations : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CubaFotoObservaciones",
            schema: "dbo",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                CubaId = table.Column<int>(type: "int", nullable: false),
                NombreArchivo = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                Observacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CubaFotoObservaciones", x => x.ID);
                table.UniqueConstraint("AK_CubaFotoObservaciones_CubaId_NombreArchivo", x => new { x.CubaId, x.NombreArchivo });
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "CubaFotoObservaciones", schema: "dbo");
}
