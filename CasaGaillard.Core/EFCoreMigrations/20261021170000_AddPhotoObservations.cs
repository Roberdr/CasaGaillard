using CasaGaillard.Core.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaGaillard.Core.EFCoreMigrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261021170000_AddPhotoObservations")]
public sealed class AddPhotoObservations : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(name: "Observacion", schema: "dbo", table: "AccesorioFotos", type: "nvarchar(500)", maxLength: 500, nullable: true);
        migrationBuilder.AddColumn<string>(name: "Observacion", schema: "dbo", table: "InstalacionEquipoFotos", type: "nvarchar(500)", maxLength: 500, nullable: true);
        migrationBuilder.AddColumn<string>(name: "Observacion", schema: "dbo", table: "TareaMantenimientoFotos", type: "nvarchar(500)", maxLength: 500, nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Observacion", schema: "dbo", table: "AccesorioFotos");
        migrationBuilder.DropColumn(name: "Observacion", schema: "dbo", table: "InstalacionEquipoFotos");
        migrationBuilder.DropColumn(name: "Observacion", schema: "dbo", table: "TareaMantenimientoFotos");
    }
}
