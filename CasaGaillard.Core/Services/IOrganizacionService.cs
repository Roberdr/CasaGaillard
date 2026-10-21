using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services;

public interface IOrganizacionService
{
    Task<IReadOnlyList<DireccionRow>> GetDireccionesAsync();
    Task<DireccionForm?> GetDireccionAsync(int? id);
    Task PrepareDireccionAsync(DireccionForm model);
    Task<int?> SaveDireccionAsync(DireccionForm model);
    Task<bool> DeleteDireccionAsync(int id);
    Task<IReadOnlyList<PersonaRow>> GetPersonasAsync();
    Task<PersonaForm?> GetPersonaAsync(int? id);
    Task PreparePersonaAsync(PersonaForm model);
    Task<int?> SavePersonaAsync(PersonaForm model);
    Task<bool> DeletePersonaAsync(int id);
    Task<bool> AddPersonaTelefonoAsync(int personaId, string telefono);
    Task<bool> UpdatePersonaTelefonoAsync(int personaId, int id, string telefono);
    Task<bool> DeletePersonaTelefonoAsync(int personaId, int id);
    Task<bool> AddPersonaEntidadAsync(int personaId, int entidadId, int? cargoId, DateTime? fechaAlta);
    Task<bool> UpdatePersonaEntidadAsync(int personaId, int id, int? cargoId, DateTime? fechaAlta, DateTime? fechaBaja);
    Task<bool> DarDeBajaPersonaEntidadAsync(int personaId, int id);
    Task<IReadOnlyList<EntidadRow>> GetEntidadesAsync();
    Task<EntidadForm?> GetEntidadAsync(int? id);
    Task PrepareEntidadAsync(EntidadForm model);
    Task<int?> SaveEntidadAsync(EntidadForm model);
    Task<bool> DeleteEntidadAsync(int id);
    Task<bool> AddEntidadTelefonoAsync(int entidadId, int personaId, string telefono);
    Task<bool> UpdateEntidadTelefonoAsync(int entidadId, int id, int? personaId, string telefono);
    Task<bool> DeleteEntidadTelefonoAsync(int entidadId, int id);
    Task<bool> AddEntidadPersonaAsync(int entidadId, int personaId, int? cargoId, DateTime? fechaAlta);
    Task<bool> UpdateEntidadPersonaAsync(int entidadId, int id, int? cargoId, DateTime? fechaAlta, DateTime? fechaBaja);
    Task<bool> DarDeBajaEntidadPersonaAsync(int entidadId, int id);
}
