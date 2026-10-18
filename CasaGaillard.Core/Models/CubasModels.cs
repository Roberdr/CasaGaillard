using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasaGaillard.Core.Models;

public class CubaFilaViewModel
{
    public int Id { get; set; }
    [Required, StringLength(10, MinimumLength = 1)]
    public string Matricula { get; set; } = "";
    public int? NumCuadro { get; set; }
    public string? Codigo { get; set; }
    public string? Vehiculo { get; set; }
    public string? Material { get; set; }
    public int? PesoBruto { get; set; }
    public int? Tara { get; set; }
    public int? PesoMaxProducto { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool Baja { get; set; }
}

public sealed class CubaFormViewModel : CubaDetalleViewModel
{
    [Display(Name = "Material exterior")] public int? MaterialId { get; set; }
    [Display(Name = "Vehículo asociado")] public int? PlataformaId { get; set; }
    public IReadOnlyList<SelectListItem> Materiales { get; set; } = [];
    public IReadOnlyList<SelectListItem> Vehiculos { get; set; } = [];
}

public class CubaDetalleViewModel : CubaFilaViewModel
{
    public IReadOnlyList<CubaCompartimentoViewModel> Compartimentos { get; set; } = [];
    public IReadOnlyList<string> Fotos { get; set; } = [];
    [Display(Name="Constructor"),StringLength(100)] public string? Constructor { get; set; }
    [Display(Name="Número de fabricación"),StringLength(12)] public string? NumFabricacion { get; set; }
    [Display(Name="Número de homologación"),StringLength(15)] public string? NumHomologacion { get; set; }
    [Display(Name="Fecha de construcción"),DataType(DataType.Date)] public DateTime? FechaConstruccion { get; set; }
    [Display(Name="País de fabricación"),StringLength(15)] public string? PaisFabricacion { get; set; }
    [Display(Name="Número tipo IMO"),StringLength(6)] public string? NumTipoIMO { get; set; }
    [Display(Name="País de aprobación"),StringLength(15)] public string? PaisAprobacion { get; set; }
    [Display(Name="Autoridad"),StringLength(45)] public string? Autoridad { get; set; }
    [Display(Name="Código de diseño"),StringLength(25)] public string? CodigoDiseno { get; set; }
    [Display(Name="Prueba hidráulica"),DataType(DataType.Date)] public DateTime? PruebaHidraulica { get; set; }
    [Display(Name="Presión de servicio ADR"),Range(typeof(decimal),"-99.99","99.99")] public decimal? PresionServicioADR { get; set; }
    [Display(Name="Presión de servicio IMO"),Range(typeof(decimal),"-99.99","99.99")] public decimal? PresionServicioIMO { get; set; }
    [Display(Name="Presión exterior"),Range(typeof(decimal),"-99.99","99.99")] public decimal? PresionExterior { get; set; }
    [Display(Name="Presión tarado válvulas"),Range(typeof(decimal),"-99.99","99.99")] public decimal? PresionTaradoValvulas { get; set; }
    [Display(Name="Temperatura de cálculo de referencia"),StringLength(12)] public string? TemperaturaCalculoReferencia { get; set; }
    [Display(Name="Espesor cuerpo"),Range(typeof(decimal),"-99.99","99.99")] public decimal? EspesorCuerpo { get; set; }
    [Display(Name="Espesor fondo"),Range(typeof(decimal),"-99.99","99.99")] public decimal? EspesorFondo { get; set; }
    [Display(Name="Espesor equivalente"),StringLength(15)] public string? EspesorEquivalente { get; set; }
    [Display(Name="Tipo de forro"),StringLength(15)] public string? TipoForro { get; set; }
    [Display(Name="Número aprobación CSC"),StringLength(15)] public string? NumAprobacionCSC { get; set; }
    [Display(Name="Modelo"),StringLength(25)] public string? Modelo { get; set; }
    [Display(Name="Peso máximo apilamiento")] public int? PesoMaxApilamiento { get; set; }
    [Display(Name="Carga rigidez")] public int? CargaRigidez { get; set; }
    [Display(Name="Presión de prueba"),Range(typeof(decimal),"-99.99","99.99")] public decimal? PresionPrueba { get; set; }
    [Display(Name="Temperatura mínima de carga"),StringLength(5)] public string? TemperaturaMinCarga { get; set; }
    [Display(Name="Longitud")] public int? Longitud { get; set; }
    [Display(Name="Ancho")] public int? Ancho { get; set; }
    [Display(Name="Alto")] public int? Alto { get; set; }
    [Display(Name="Aprobación IMDG"),StringLength(20)] public string? NumAprobacionIMDG { get; set; }
    [Display(Name="Aprobación ADR/RID"),StringLength(20)] public string? NumAprobacionADR_RID { get; set; }
    [Display(Name="UN Portable Tank"),StringLength(15)] public string? UNPortableTank { get; set; }
    [Display(Name="Número de aprobación"),StringLength(20)] public string? NumAprobacion { get; set; }
}

public sealed class CubaCompartimentoViewModel
{
    public int Numero { get; set; }
    public int? Capacidad { get; set; }
    public string? Material { get; set; }
}
