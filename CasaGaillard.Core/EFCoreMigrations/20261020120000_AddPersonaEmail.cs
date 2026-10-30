using CasaGaillard.Core.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaGaillard.Core.EFCoreMigrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261020120000_AddPersonaEmail")]
public sealed class AddPersonaEmail : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Email",
            schema: "gaillard",
            table: "Persona",
            type: "nvarchar(256)",
            maxLength: 256,
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "UX_Persona_Email",
            schema: "gaillard",
            table: "Persona",
            column: "Email",
            unique: true,
            filter: "[Email] IS NOT NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "UX_Persona_Email", schema: "gaillard", table: "Persona");
        migrationBuilder.DropColumn(name: "Email", schema: "gaillard", table: "Persona");
    }
}
