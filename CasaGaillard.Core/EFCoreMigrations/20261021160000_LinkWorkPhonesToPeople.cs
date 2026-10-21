using CasaGaillard.Core.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaGaillard.Core.EFCoreMigrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261021160000_LinkWorkPhonesToPeople")]
public sealed class LinkWorkPhonesToPeople : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.Sql(@"
        ALTER TABLE gaillard.Telefono DROP CONSTRAINT CK_Telefono_UnSoloTitular;
        ALTER TABLE gaillard.Telefono ADD CONSTRAINT CK_Telefono_AlMenosUnTitular
            CHECK (PersonaID IS NOT NULL OR EntidadID IS NOT NULL);");

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.Sql(@"
        IF EXISTS (SELECT 1 FROM gaillard.Telefono WHERE PersonaID IS NOT NULL AND EntidadID IS NOT NULL)
            THROW 51001, 'No se puede revertir: hay teléfonos de trabajo vinculados simultáneamente a persona y entidad.', 1;
        ALTER TABLE gaillard.Telefono DROP CONSTRAINT CK_Telefono_AlMenosUnTitular;
        ALTER TABLE gaillard.Telefono ADD CONSTRAINT CK_Telefono_UnSoloTitular
            CHECK ((PersonaID IS NOT NULL AND EntidadID IS NULL) OR (PersonaID IS NULL AND EntidadID IS NOT NULL));");
}
