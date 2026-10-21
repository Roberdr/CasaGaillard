using CasaGaillard.Core.Areas.Mantenimiento.Controllers;
using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CasaGaillard.Core.Tests;

public sealed class TareasMantenimientoTests
{
    [Fact]
    public async Task GetAllAsync_FiltraPorEstadoYFechasProgramadas()
    {
        var options=new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db=new AppDbContext(options);
        db.TareasMantenimiento.AddRange(
            Tarea("TM-1","En curso",new DateTime(2026,10,20)),
            Tarea("TM-2","Cerrada",new DateTime(2026,10,21)),
            Tarea("TM-3","En curso",new DateTime(2026,11,1)));
        await db.SaveChangesAsync();

        var result=await new TareasMantenimientoService(db).GetAllAsync(null,"En curso",null,1,
            new DateTime(2026,10,20),new DateTime(2026,10,31));

        Assert.Single(result.Tareas);
        Assert.Equal("TM-1",result.Tareas[0].Codigo);
    }

    [Fact]
    public void Controller_RestringeAccesoALectoresDeMantenimiento()
    {
        var authorize=typeof(TareasMantenimientoController).GetCustomAttributes(typeof(AuthorizeAttribute),true)
            .Cast<AuthorizeAttribute>().Single();
        Assert.Equal(AppRoles.LecturaMantenimiento,authorize.Roles);
    }

    [Fact]
    public void ValidadorAceptaFotoValidaYRechazaTipoOExtensionIncompatibles()
    {
        var foto=Foto("foto.jpg","image/jpeg",1024);
        var archivoRenombrado=Foto("informe.exe","image/jpeg",1024);
        var contenidoNoImagen=Foto("foto.jpg","text/plain",1024);

        Assert.True(TareaFotoValidator.TipoPermitido(foto));
        Assert.False(TareaFotoValidator.TipoPermitido(archivoRenombrado));
        Assert.False(TareaFotoValidator.TipoPermitido(contenidoNoImagen));
        Assert.True(TareaFotoValidator.TamanoPermitido(foto));
    }

    private static TareaMantenimiento Tarea(string codigo,string estado,DateTime prevista)=>new()
    {
        Codigo=codigo,Titulo=codigo,Descripcion="Incidencia de prueba",Estado=estado,Prioridad="Media",
        FechaCreacion=new DateTime(2026,10,1),FechaDeteccion=new DateTime(2026,10,1),FechaProgramada=prevista
    };

    private static IFormFile Foto(string nombre,string tipo,int bytes)
    {
        var file=new FormFile(new MemoryStream(new byte[bytes]),0,bytes,"FotosAdjuntas",nombre);
        file.Headers=new HeaderDictionary { ["Content-Type"]=tipo };
        return file;
    }
}
