using CasaGaillard.Core.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaGaillard.Core.EFCoreMigrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261024120000_AddPlanificadorEstados")]
public sealed class AddPlanificadorEstados : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.CreateTable(
        name: "PlanificadorEstados",
        schema: "dbo",
        columns: table => new
        {
            Clave = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
            Json = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ActualizadoUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            ActualizadoPor = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
        },
        constraints: table => table.PrimaryKey("PK_PlanificadorEstados", x => x.Clave));

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "PlanificadorEstados", schema: "dbo");
}
