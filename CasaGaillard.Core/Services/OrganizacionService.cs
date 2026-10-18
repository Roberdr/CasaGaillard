using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class OrganizacionService : IOrganizacionService
{
    private readonly AppDbContext _db;
    public OrganizacionService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<DireccionRow>> GetDireccionesAsync() => await _db.Database.SqlQueryRaw<DireccionRow>(@"
        SELECT d.ID Id, tv.TipoVia TipoVia, d.NombreVia NombreVia, d.Numero Numero, d.CP CodigoPostal,
               p.NombrePoblacion Poblacion, d.Pais Pais
        FROM gaillard.Direccion d LEFT JOIN gaillard.TipoVia tv ON tv.ID=d.TipoViaID
        LEFT JOIN gaillard.Poblacion p ON p.ID=d.PoblacionID ORDER BY p.NombrePoblacion,d.NombreVia,d.Numero").ToListAsync();

    public async Task<DireccionForm?> GetDireccionAsync(int? id)
    {
        if (!id.HasValue) return new DireccionForm();
        var rows = await _db.Database.SqlQueryRaw<DireccionDbRow>(@"
            SELECT d.ID Id,d.TipoViaID TipoViaId,d.NombreVia,d.Distrito,d.Numero,d.Escalera,d.Piso,d.Puerta,
                   d.PoblacionID PoblacionId,d.CP CodigoPostal,d.Pais,
                   tv.TipoVia TipoViaNombre,p.NombrePoblacion PoblacionNombre
            FROM gaillard.Direccion d LEFT JOIN gaillard.TipoVia tv ON tv.ID=d.TipoViaID
            LEFT JOIN gaillard.Poblacion p ON p.ID=d.PoblacionID WHERE d.ID={0}", id.Value).ToListAsync();
        var row = rows.FirstOrDefault();
        return row is null ? null : new DireccionForm
        {
            Id = row.Id, TipoViaId = row.TipoViaId, NombreVia = row.NombreVia, Distrito = row.Distrito,
            Numero = row.Numero, Escalera = row.Escalera, Piso = row.Piso, Puerta = row.Puerta,
            PoblacionId = row.PoblacionId, CodigoPostal = row.CodigoPostal, Pais = row.Pais,
            TipoViaNombre = row.TipoViaNombre, PoblacionNombre = row.PoblacionNombre
        };
    }

    public async Task PrepareDireccionAsync(DireccionForm model)
    {
        model.TiposVia = await _db.Database.SqlQueryRaw<OptionRow>("SELECT ID Id,TipoVia Texto FROM gaillard.TipoVia ORDER BY TipoVia")
            .SelectAsync(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(x.Texto, x.Id.ToString(), x.Id == model.TipoViaId));
        model.Poblaciones = await _db.Database.SqlQueryRaw<OptionRow>("SELECT ID Id,NombrePoblacion Texto FROM gaillard.Poblacion ORDER BY NombrePoblacion")
            .SelectAsync(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(x.Texto, x.Id.ToString(), x.Id == model.PoblacionId));
    }

    public async Task<int?> SaveDireccionAsync(DireccionForm model)
    {
        if (model.Id == 0)
        {
            return await ExecuteScalarAsync<int>(@"INSERT INTO gaillard.Direccion(TipoViaID,NombreVia,Distrito,Numero,Escalera,Piso,Puerta,PoblacionID,CP,Pais)
                OUTPUT INSERTED.ID VALUES(@tipoVia,@nombre,@distrito,@numero,@escalera,@piso,@puerta,@poblacion,@cp,@pais)",
                ("@tipoVia", model.TipoViaId), ("@nombre", model.NombreVia), ("@distrito", model.Distrito), ("@numero", model.Numero),
                ("@escalera", model.Escalera), ("@piso", model.Piso), ("@puerta", model.Puerta), ("@poblacion", model.PoblacionId),
                ("@cp", model.CodigoPostal), ("@pais", model.Pais));
        }
        var count = await _db.Database.ExecuteSqlInterpolatedAsync($@"UPDATE gaillard.Direccion SET TipoViaID={model.TipoViaId},NombreVia={model.NombreVia},
            Distrito={model.Distrito},Numero={model.Numero},Escalera={model.Escalera},Piso={model.Piso},Puerta={model.Puerta},
            PoblacionID={model.PoblacionId},CP={model.CodigoPostal},Pais={model.Pais} WHERE ID={model.Id}");
        return count == 0 ? null : model.Id;
    }

    public async Task<bool> DeleteDireccionAsync(int id) => await _db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM gaillard.Direccion WHERE ID={id}") > 0;

    public async Task<IReadOnlyList<PersonaRow>> GetPersonasAsync() => await _db.Database.SqlQueryRaw<PersonaRow>(@"
        SELECT p.ID Id,p.NombrePersona Nombre,p.Apellido1,p.Apellido2,p.NIF Nif,
               CASE WHEN d.ID IS NULL THEN N'Sin dirección' ELSE CONCAT(COALESCE(tv.TipoVia,N''),N' ',COALESCE(d.NombreVia,N''),N' ',COALESCE(CONVERT(nvarchar(20),d.Numero),N''),N', ',COALESCE(po.NombrePoblacion,N'')) END Direccion
        FROM gaillard.Persona p LEFT JOIN gaillard.Direccion d ON d.ID=p.DireccionID
        LEFT JOIN gaillard.TipoVia tv ON tv.ID=d.TipoViaID LEFT JOIN gaillard.Poblacion po ON po.ID=d.PoblacionID
        ORDER BY p.NombrePersona,p.Apellido1,p.Apellido2").ToListAsync();

    public async Task<PersonaForm?> GetPersonaAsync(int? id)
    {
        if (!id.HasValue) return new PersonaForm();
        var rows = await _db.Database.SqlQueryRaw<PersonaDbRow>(@"SELECT p.ID Id,p.NombrePersona Nombre,p.Apellido1,p.Apellido2,p.NIF Nif,p.DireccionID DireccionId,
            CONCAT(COALESCE(tv.TipoVia,N''),N' ',COALESCE(d.NombreVia,N''),N' ',COALESCE(CONVERT(nvarchar(20),d.Numero),N''),N', ',COALESCE(po.NombrePoblacion,N'')) DireccionTexto
            FROM gaillard.Persona p LEFT JOIN gaillard.Direccion d ON d.ID=p.DireccionID LEFT JOIN gaillard.TipoVia tv ON tv.ID=d.TipoViaID
            LEFT JOIN gaillard.Poblacion po ON po.ID=d.PoblacionID WHERE p.ID={0}", id.Value).ToListAsync();
        var row = rows.FirstOrDefault();
        if (row is null) return null;
        var model = new PersonaForm { Id = row.Id, Nombre = row.Nombre, Apellido1 = row.Apellido1, Apellido2 = row.Apellido2, Nif = row.Nif, DireccionId = row.DireccionId, DireccionTexto = row.DireccionTexto };
        model.Telefonos = await _db.Database.SqlQueryRaw<OrganizacionTelefono>("SELECT ID Id,TelefonoPersona Numero FROM gaillard.TelefonoPersona WHERE PersonaID={0} ORDER BY ID", id.Value).ToListAsync();
        model.Relaciones = await _db.Database.SqlQueryRaw<PersonaRelacion>(@"SELECT pe.ID Id,pe.PersonaID PersonaId,pe.EntidadID EntidadId,
            e.NombreEntidad Nombre,c.Cargo Cargo,pe.CargoID CargoId,pe.FechaAlta,pe.FechaBaja FROM gaillard.PersonasEntidad pe
            JOIN gaillard.Entidad e ON e.ID=pe.EntidadID LEFT JOIN gaillard.Cargo c ON c.ID=pe.CargoID
            WHERE pe.PersonaID={0} ORDER BY pe.FechaBaja,pe.FechaAlta DESC", id.Value).ToListAsync();
        model.Entidades = await GetEntityOptionsAsync();
        model.Cargos = await GetCargoOptionsAsync();
        return model;
    }

    public async Task PreparePersonaAsync(PersonaForm model)
    {
        model.Direcciones = await GetAddressOptionsAsync(model.DireccionId);
        if (model.Id > 0)
        {
            var current = await GetPersonaAsync(model.Id);
            if (current is not null)
            {
                model.Telefonos = current.Telefonos;
                model.Relaciones = current.Relaciones;
                model.Entidades = current.Entidades;
                model.Cargos = current.Cargos;
            }
        }
    }

    public async Task<int?> SavePersonaAsync(PersonaForm model)
    {
        if (model.Id == 0)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            var ids = await _db.Database.SqlQueryRaw<int>("SELECT ISNULL(MAX(ID),0)+1 Id FROM gaillard.Persona WITH (UPDLOCK,HOLDLOCK)").ToListAsync();
            var id = ids.Single();
            await _db.Database.ExecuteSqlInterpolatedAsync($@"INSERT INTO gaillard.Persona(ID,NombrePersona,Apellido1,Apellido2,NIF,DireccionID)
                VALUES({id},{model.Nombre},{model.Apellido1},{model.Apellido2},{model.Nif},{model.DireccionId})");
            await transaction.CommitAsync();
            return id;
        }
        var count = await _db.Database.ExecuteSqlInterpolatedAsync($@"UPDATE gaillard.Persona SET NombrePersona={model.Nombre},Apellido1={model.Apellido1},
            Apellido2={model.Apellido2},NIF={model.Nif},DireccionID={model.DireccionId} WHERE ID={model.Id}");
        return count == 0 ? null : model.Id;
    }

    public async Task<bool> DeletePersonaAsync(int id) => await _db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM gaillard.Persona WHERE ID={id}") > 0;

    public Task<bool> AddPersonaTelefonoAsync(int personaId, string telefono) => AddPhoneAsync("gaillard.TelefonoPersona", personaId, telefono);
    public Task<bool> UpdatePersonaTelefonoAsync(int personaId, int id, string telefono) => UpdatePhoneAsync("gaillard.TelefonoPersona", personaId, id, telefono);
    public Task<bool> DeletePersonaTelefonoAsync(int personaId, int id) => DeletePhoneAsync("gaillard.TelefonoPersona", personaId, id);
    public Task<bool> AddEntidadTelefonoAsync(int entidadId, string telefono) => AddPhoneAsync("gaillard.TelefonoEntidad", entidadId, telefono);
    public Task<bool> UpdateEntidadTelefonoAsync(int entidadId, int id, string telefono) => UpdatePhoneAsync("gaillard.TelefonoEntidad", entidadId, id, telefono);
    public Task<bool> DeleteEntidadTelefonoAsync(int entidadId, int id) => DeletePhoneAsync("gaillard.TelefonoEntidad", entidadId, id);

    public Task<bool> AddPersonaEntidadAsync(int personaId, int entidadId, int? cargoId, DateTime? fechaAlta) =>
        AddRelationAsync(personaId, entidadId, cargoId, fechaAlta);
    public Task<bool> AddEntidadPersonaAsync(int entidadId, int personaId, int? cargoId, DateTime? fechaAlta) =>
        AddRelationAsync(personaId, entidadId, cargoId, fechaAlta);
    public Task<bool> UpdatePersonaEntidadAsync(int personaId, int id, int? cargoId, DateTime? fechaAlta, DateTime? fechaBaja) =>
        UpdateRelationAsync(id, personaId, null, cargoId, fechaAlta, fechaBaja);
    public Task<bool> UpdateEntidadPersonaAsync(int entidadId, int id, int? cargoId, DateTime? fechaAlta, DateTime? fechaBaja) =>
        UpdateRelationAsync(id, null, entidadId, cargoId, fechaAlta, fechaBaja);
    public Task<bool> DarDeBajaPersonaEntidadAsync(int personaId, int id) => EndRelationAsync(id, personaId, null);
    public Task<bool> DarDeBajaEntidadPersonaAsync(int entidadId, int id) => EndRelationAsync(id, null, entidadId);

    public async Task<IReadOnlyList<EntidadRow>> GetEntidadesAsync() => await _db.Database.SqlQueryRaw<EntidadRow>(@"
        SELECT e.ID Id,e.NombreEntidad Nombre,e.CIF Cif,
               CONCAT(COALESCE(tv.TipoVia,N''),N' ',COALESCE(d.NombreVia,N''),N' ',COALESCE(CONVERT(nvarchar(20),d.Numero),N''),N', ',COALESCE(p.NombrePoblacion,N'')) Direccion
        FROM gaillard.Entidad e LEFT JOIN gaillard.Direccion d ON d.ID=e.DireccionID
        LEFT JOIN gaillard.TipoVia tv ON tv.ID=d.TipoViaID LEFT JOIN gaillard.Poblacion p ON p.ID=d.PoblacionID
        ORDER BY e.NombreEntidad").ToListAsync();

    public async Task<EntidadForm?> GetEntidadAsync(int? id)
    {
        if (!id.HasValue) return new EntidadForm();
        var rows = await _db.Database.SqlQueryRaw<EntidadDbRow>(@"SELECT e.ID Id,e.NombreEntidad Nombre,e.CIF Cif,e.DireccionID DireccionId,
            CONCAT(COALESCE(tv.TipoVia,N''),N' ',COALESCE(d.NombreVia,N''),N' ',COALESCE(CONVERT(nvarchar(20),d.Numero),N''),N', ',COALESCE(p.NombrePoblacion,N'')) DireccionTexto
            FROM gaillard.Entidad e LEFT JOIN gaillard.Direccion d ON d.ID=e.DireccionID LEFT JOIN gaillard.TipoVia tv ON tv.ID=d.TipoViaID
            LEFT JOIN gaillard.Poblacion p ON p.ID=d.PoblacionID WHERE e.ID={0}", id.Value).ToListAsync();
        var row = rows.FirstOrDefault();
        if (row is null) return null;
        var model = new EntidadForm { Id = row.Id, Nombre = row.Nombre, Cif = row.Cif, DireccionId = row.DireccionId, DireccionTexto = row.DireccionTexto };
        model.Telefonos = await _db.Database.SqlQueryRaw<OrganizacionTelefono>("SELECT ID Id,Telefono Numero FROM gaillard.TelefonoEntidad WHERE EntidadID={0} ORDER BY ID", id.Value).ToListAsync();
        model.Relaciones = await _db.Database.SqlQueryRaw<PersonaRelacion>(@"SELECT pe.ID Id,pe.PersonaID PersonaId,pe.EntidadID EntidadId,
            LTRIM(RTRIM(CONCAT(COALESCE(p.NombrePersona,N''),N' ',COALESCE(p.Apellido1,N''),N' ',COALESCE(p.Apellido2,N'')))) Nombre,
            c.Cargo Cargo,pe.CargoID CargoId,pe.FechaAlta,pe.FechaBaja FROM gaillard.PersonasEntidad pe
            JOIN gaillard.Persona p ON p.ID=pe.PersonaID LEFT JOIN gaillard.Cargo c ON c.ID=pe.CargoID
            WHERE pe.EntidadID={0} ORDER BY pe.FechaBaja,pe.FechaAlta DESC", id.Value).ToListAsync();
        model.Personas = await GetPersonOptionsAsync();
        model.Cargos = await GetCargoOptionsAsync();
        return model;
    }

    public async Task PrepareEntidadAsync(EntidadForm model) => model.Direcciones = await GetAddressOptionsAsync(model.DireccionId);

    public async Task<int?> SaveEntidadAsync(EntidadForm model)
    {
        if (model.Id == 0)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            var ids = await _db.Database.SqlQueryRaw<int>("SELECT ISNULL(MAX(ID),0)+1 Id FROM gaillard.Entidad WITH (UPDLOCK,HOLDLOCK)").ToListAsync();
            var id = ids.Single();
            await _db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO gaillard.Entidad(ID,NombreEntidad,CIF,DireccionID) VALUES({id},{model.Nombre},{model.Cif},{model.DireccionId})");
            await transaction.CommitAsync();
            return id;
        }
        var count = await _db.Database.ExecuteSqlInterpolatedAsync($"UPDATE gaillard.Entidad SET NombreEntidad={model.Nombre},CIF={model.Cif},DireccionID={model.DireccionId} WHERE ID={model.Id}");
        return count == 0 ? null : model.Id;
    }

    public async Task<bool> DeleteEntidadAsync(int id) => await _db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM gaillard.Entidad WHERE ID={id}") > 0;

    private async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetAddressOptionsAsync(int? selectedId)
    {
        var rows = await _db.Database.SqlQueryRaw<AddressOption>(@"SELECT d.ID Id,
            CONCAT(LTRIM(RTRIM(COALESCE(tv.TipoVia,N''))),N' ',LTRIM(RTRIM(COALESCE(d.NombreVia,N''))),N' ',COALESCE(CONVERT(nvarchar(20),d.Numero),N''),N' — ',LTRIM(RTRIM(COALESCE(p.NombrePoblacion,N'')))) Texto
            FROM gaillard.Direccion d LEFT JOIN gaillard.TipoVia tv ON tv.ID=d.TipoViaID LEFT JOIN gaillard.Poblacion p ON p.ID=d.PoblacionID
            ORDER BY p.NombrePoblacion,d.NombreVia,d.Numero").ToListAsync();
        return rows.Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(x.Texto, x.Id.ToString(), x.Id == selectedId)).ToList();
    }

    private async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetEntityOptionsAsync() =>
        (await _db.Database.SqlQueryRaw<OptionRow>("SELECT ID Id,NombreEntidad Texto FROM gaillard.Entidad ORDER BY NombreEntidad").ToListAsync())
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(x.Texto, x.Id.ToString())).ToList();

    private async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetPersonOptionsAsync() =>
        (await _db.Database.SqlQueryRaw<PersonOption>(@"SELECT ID Id,LTRIM(RTRIM(CONCAT(COALESCE(NombrePersona,N''),N' ',COALESCE(Apellido1,N''),N' ',COALESCE(Apellido2,N'')))) Texto
            FROM gaillard.Persona ORDER BY Apellido1,Apellido2,NombrePersona").ToListAsync())
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(x.Texto, x.Id.ToString())).ToList();

    private async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetCargoOptionsAsync() =>
        (await _db.Database.SqlQueryRaw<OptionRow>("SELECT ID Id,Cargo Texto FROM gaillard.Cargo ORDER BY Cargo").ToListAsync())
            .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(x.Texto, x.Id.ToString())).ToList();

    private async Task<bool> AddPhoneAsync(string table, int ownerId, string phone)
    {
        phone = phone.Trim();
        if (phone.Length is 0 or > 10) return false;
        var ownerExists = table switch
        {
            "gaillard.TelefonoPersona" => await _db.Database.SqlQueryRaw<int>("SELECT 1 Value FROM gaillard.Persona WHERE ID={0}", ownerId).AnyAsync(),
            "gaillard.TelefonoEntidad" => await _db.Database.SqlQueryRaw<int>("SELECT 1 Value FROM gaillard.Entidad WHERE ID={0}", ownerId).AnyAsync(),
            _ => false
        };
        if (!ownerExists) return false;
        await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var ids = table == "gaillard.TelefonoPersona"
            ? await _db.Database.SqlQueryRaw<int>("SELECT ISNULL(MAX(ID),0)+1 Id FROM gaillard.TelefonoPersona WITH (UPDLOCK,HOLDLOCK)").ToListAsync()
            : await _db.Database.SqlQueryRaw<int>("SELECT ISNULL(MAX(ID),0)+1 Id FROM gaillard.TelefonoEntidad WITH (UPDLOCK,HOLDLOCK)").ToListAsync();
        var id = ids.Single();
        var affected = table == "gaillard.TelefonoPersona"
            ? await _db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO gaillard.TelefonoPersona(ID,TelefonoPersona,PersonaID) VALUES({id},{phone},{ownerId})")
            : await _db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO gaillard.TelefonoEntidad(ID,Telefono,EntidadID) VALUES({id},{phone},{ownerId})");
        await transaction.CommitAsync();
        return affected > 0;
    }

    private async Task<bool> UpdatePhoneAsync(string table, int ownerId, int id, string phone)
    {
        phone = phone.Trim();
        if (phone.Length is 0 or > 10) return false;
        var affected = table == "gaillard.TelefonoPersona"
            ? await _db.Database.ExecuteSqlInterpolatedAsync($"UPDATE gaillard.TelefonoPersona SET TelefonoPersona={phone} WHERE ID={id} AND PersonaID={ownerId}")
            : await _db.Database.ExecuteSqlInterpolatedAsync($"UPDATE gaillard.TelefonoEntidad SET Telefono={phone} WHERE ID={id} AND EntidadID={ownerId}");
        return affected > 0;
    }

    private async Task<bool> DeletePhoneAsync(string table, int ownerId, int id) =>
        await (table == "gaillard.TelefonoPersona"
            ? _db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM gaillard.TelefonoPersona WHERE ID={id} AND PersonaID={ownerId}")
            : _db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM gaillard.TelefonoEntidad WHERE ID={id} AND EntidadID={ownerId}")) > 0;

    private async Task<bool> AddRelationAsync(int personId, int entityId, int? cargoId, DateTime? date)
    {
        if (!await _db.Database.SqlQueryRaw<int>("SELECT 1 Value FROM gaillard.Persona WHERE ID={0}", personId).AnyAsync() ||
            !await _db.Database.SqlQueryRaw<int>("SELECT 1 Value FROM gaillard.Entidad WHERE ID={0}", entityId).AnyAsync()) return false;
        if (cargoId.HasValue && !await _db.Database.SqlQueryRaw<int>("SELECT 1 Value FROM gaillard.Cargo WHERE ID={0}", cargoId.Value).AnyAsync()) return false;
        if (await _db.Database.SqlQueryRaw<int>("SELECT 1 Value FROM gaillard.PersonasEntidad WHERE PersonaID={0} AND EntidadID={1} AND FechaBaja IS NULL", personId, entityId).AnyAsync()) return false;
        await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var ids = await _db.Database.SqlQueryRaw<int>("SELECT ISNULL(MAX(ID),0)+1 Id FROM gaillard.PersonasEntidad WITH (UPDLOCK,HOLDLOCK)").ToListAsync();
        var id = ids.Single();
        var affected = await _db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO gaillard.PersonasEntidad(ID,EntidadID,PersonaID,CargoID,FechaAlta,FechaBaja) VALUES({id},{entityId},{personId},{cargoId},{(date ?? DateTime.Today).Date},NULL)");
        await transaction.CommitAsync(); return affected > 0;
    }

    private async Task<bool> UpdateRelationAsync(int id, int? personId, int? entityId, int? cargoId, DateTime? fechaAlta, DateTime? fechaBaja)
    {
        if (fechaAlta.HasValue && fechaBaja.HasValue && fechaBaja.Value.Date < fechaAlta.Value.Date) return false;
        var rows = await _db.Database.SqlQueryRaw<RelationOwner>("SELECT ID Id,PersonaID PersonaId,EntidadID EntidadId FROM gaillard.PersonasEntidad WHERE ID={0}", id).ToListAsync();
        var relation = rows.FirstOrDefault();
        if (relation is null || (personId.HasValue && relation.PersonaId != personId) || (entityId.HasValue && relation.EntidadId != entityId)) return false;
        if (cargoId.HasValue && !await _db.Database.SqlQueryRaw<int>("SELECT 1 Value FROM gaillard.Cargo WHERE ID={0}", cargoId.Value).AnyAsync()) return false;
        if (!fechaBaja.HasValue && await _db.Database.SqlQueryRaw<int>("SELECT 1 Value FROM gaillard.PersonasEntidad WHERE ID<>{0} AND PersonaID={1} AND EntidadID={2} AND FechaBaja IS NULL", id, relation.PersonaId, relation.EntidadId).AnyAsync()) return false;
        return await _db.Database.ExecuteSqlInterpolatedAsync($"UPDATE gaillard.PersonasEntidad SET CargoID={cargoId},FechaAlta={fechaAlta?.Date},FechaBaja={fechaBaja?.Date} WHERE ID={id}") > 0;
    }

    private async Task<bool> EndRelationAsync(int id, int? personId, int? entityId)
    {
        var rows = personId.HasValue
            ? await _db.Database.SqlQueryRaw<DateTime?>("SELECT FechaAlta Value FROM gaillard.PersonasEntidad WHERE ID={0} AND PersonaID={1} AND FechaBaja IS NULL", id, personId.Value).ToListAsync()
            : await _db.Database.SqlQueryRaw<DateTime?>("SELECT FechaAlta Value FROM gaillard.PersonasEntidad WHERE ID={0} AND EntidadID={1} AND FechaBaja IS NULL", id, entityId!.Value).ToListAsync();
        if (rows.Count == 0 || (rows[0].HasValue && rows[0]!.Value.Date > DateTime.Today)) return false;
        return await _db.Database.ExecuteSqlInterpolatedAsync($"UPDATE gaillard.PersonasEntidad SET FechaBaja={DateTime.Today} WHERE ID={id} AND FechaBaja IS NULL") > 0;
    }

    private async Task<T> ExecuteScalarAsync<T>(string sql, params (string Name, object? Value)[] values)
    {
        var connection = _db.Database.GetDbConnection();
        var close = connection.State != ConnectionState.Open;
        if (close) await _db.Database.OpenConnectionAsync();
        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            foreach (var (name, value) in values)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = name;
                parameter.Value = value ?? DBNull.Value;
                if (value is null) parameter.DbType = DbType.String;
                command.Parameters.Add(parameter);
            }
            var result = await command.ExecuteScalarAsync();
            return (T)Convert.ChangeType(result!, typeof(T));
        }
        finally
        {
            if (close) await _db.Database.CloseConnectionAsync();
        }
    }

    private sealed class OptionRow { public int Id { get; set; } public string Texto { get; set; } = ""; }
    private sealed class AddressOption { public int Id { get; set; } public string Texto { get; set; } = ""; }
    private sealed class PersonOption { public int Id { get; set; } public string Texto { get; set; } = ""; }
    private sealed class RelationOwner { public int Id { get; set; } public int PersonaId { get; set; } public int EntidadId { get; set; } }
    private sealed class DireccionDbRow
    {
        public int Id { get; set; } public int TipoViaId { get; set; } public string? NombreVia { get; set; } public string? Distrito { get; set; }
        public int? Numero { get; set; } public string? Escalera { get; set; } public string? Piso { get; set; } public string? Puerta { get; set; }
        public int PoblacionId { get; set; } public string? CodigoPostal { get; set; } public string? Pais { get; set; }
        public string? TipoViaNombre { get; set; } public string? PoblacionNombre { get; set; }
    }
    private sealed class PersonaDbRow
    {
        public int Id { get; set; } public string? Nombre { get; set; } public string? Apellido1 { get; set; } public string? Apellido2 { get; set; }
        public string? Nif { get; set; } public int? DireccionId { get; set; } public string? DireccionTexto { get; set; }
    }
    private sealed class EntidadDbRow
    {
        public int Id { get; set; } public string? Nombre { get; set; } public string? Cif { get; set; } public int DireccionId { get; set; }
        public string? DireccionTexto { get; set; }
    }
}

internal static class QueryableSelectListExtensions
{
    public static async Task<List<TOut>> SelectAsync<TIn, TOut>(this IQueryable<TIn> query, Func<TIn, TOut> selector) =>
        (await query.ToListAsync()).Select(selector).ToList();
}
