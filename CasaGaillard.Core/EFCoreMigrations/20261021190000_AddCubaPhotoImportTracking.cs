using CasaGaillard.Core.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaGaillard.Core.EFCoreMigrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261021190000_AddCubaPhotoImportTracking")]
public sealed class AddCubaPhotoImportTracking : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.CreateTable(
        name: "CubaFotoImportadas",
        schema: "dbo",
        columns: table => new
        {
            CubaId = table.Column<int>(type: "int", nullable: false),
            NombreArchivo = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
            ImportadaEn = table.Column<DateTime>(type: "datetime2", nullable: false)
        },
        constraints: table => table.PrimaryKey("PK_CubaFotoImportadas", x => new { x.CubaId, x.NombreArchivo }));

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "CubaFotoImportadas", schema: "dbo");
}
