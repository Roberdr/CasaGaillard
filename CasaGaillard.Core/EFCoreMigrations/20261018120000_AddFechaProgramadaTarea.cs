using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using CasaGaillard.Core.Data;

#nullable disable

namespace CasaGaillard.Core.EFCoreMigrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261018120000_AddFechaProgramadaTarea")]
public sealed class AddFechaProgramadaTarea : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) =>
        migrationBuilder.AddColumn<DateTime>(
            name: "FechaProgramada",
            schema: "dbo",
            table: "TareasMantenimiento",
            type: "date",
            nullable: true);

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropColumn(
            name: "FechaProgramada",
            schema: "dbo",
            table: "TareasMantenimiento");
}
