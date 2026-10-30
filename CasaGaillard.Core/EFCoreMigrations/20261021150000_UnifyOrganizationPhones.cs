using CasaGaillard.Core.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaGaillard.Core.EFCoreMigrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261021150000_UnifyOrganizationPhones")]
public sealed class UnifyOrganizationPhones : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Telefono",
            schema: "gaillard",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                Telefono = table.Column<string>(type: "nchar(10)", maxLength: 10, nullable: false),
                PersonaID = table.Column<int>(type: "int", nullable: true),
                EntidadID = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Telefono", x => x.ID);
                table.CheckConstraint("CK_Telefono_UnSoloTitular", "([PersonaID] IS NOT NULL AND [EntidadID] IS NULL) OR ([PersonaID] IS NULL AND [EntidadID] IS NOT NULL)");
                table.ForeignKey("FK_Telefono_Persona", x => x.PersonaID, "Persona", "ID", "gaillard");
                table.ForeignKey("FK_Telefono_Entidad", x => x.EntidadID, "Entidad", "ID", "gaillard");
            });

        migrationBuilder.CreateIndex("IX_Telefono_PersonaID", "Telefono", "PersonaID", "gaillard", filter: "[PersonaID] IS NOT NULL");
        migrationBuilder.CreateIndex("IX_Telefono_EntidadID", "Telefono", "EntidadID", "gaillard", filter: "[EntidadID] IS NOT NULL");
        migrationBuilder.Sql(@"INSERT INTO gaillard.Telefono(Telefono, PersonaID)
            SELECT LTRIM(RTRIM(TelefonoPersona)), PersonaID FROM gaillard.TelefonoPersona;
            INSERT INTO gaillard.Telefono(Telefono, EntidadID)
            SELECT LTRIM(RTRIM(Telefono)), EntidadID FROM gaillard.TelefonoEntidad;");
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "Telefono", schema: "gaillard");
}
