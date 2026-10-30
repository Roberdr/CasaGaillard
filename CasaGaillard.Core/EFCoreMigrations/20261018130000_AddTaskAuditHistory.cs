using CasaGaillard.Core.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaGaillard.Core.EFCoreMigrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261018130000_AddTaskAuditHistory")]
public sealed class AddTaskAuditHistory : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TareaMantenimientoHistorial",
            schema: "dbo",
            columns: table => new
            {
                ID = table.Column<long>(type: "bigint", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                TareaMantenimientoID = table.Column<int>(type: "int", nullable: false),
                Usuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                Accion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                CambiosJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_TareaMantenimientoHistorial", x => x.ID));

        migrationBuilder.CreateIndex(name: "IX_TareaMantenimientoHistorial_TareaMantenimientoID_Fecha",
            schema: "dbo", table: "TareaMantenimientoHistorial", columns: new[] { "TareaMantenimientoID", "Fecha" });
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "TareaMantenimientoHistorial", schema: "dbo");
}
