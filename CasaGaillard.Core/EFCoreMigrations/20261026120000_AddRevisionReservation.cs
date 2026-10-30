using CasaGaillard.Core.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaGaillard.Core.EFCoreMigrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261026120000_AddRevisionReservation")]
public sealed class AddRevisionReservation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(name: "DiaReservado", schema: "gaillard", table: "Revision", type: "bit", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<DateTime>(name: "FechaReservada", schema: "gaillard", table: "Revision", type: "date", nullable: true);
        migrationBuilder.AddColumn<bool>(name: "DiaReservado", schema: "gaillard", table: "RevisionVehiculo", type: "bit", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<DateTime>(name: "FechaReservada", schema: "gaillard", table: "RevisionVehiculo", type: "date", nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "DiaReservado", schema: "gaillard", table: "Revision");
        migrationBuilder.DropColumn(name: "FechaReservada", schema: "gaillard", table: "Revision");
        migrationBuilder.DropColumn(name: "DiaReservado", schema: "gaillard", table: "RevisionVehiculo");
        migrationBuilder.DropColumn(name: "FechaReservada", schema: "gaillard", table: "RevisionVehiculo");
    }
}
