using System;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(CubaMetaData))]
    public partial class Cuba
    {
    }
    public class CubaMetaData
    {
        [Display(Name = "Matrícula")]
        public string MatriculaCuba { get; set; }

        [Display(Name = "Cuadro")]
        public Nullable<int> NumCuadro { get; set; }

        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Display(Name = "Núm. Fabricación")]
        public string NumFabricacion { get; set; }

        [Display(Name = "Núm. Homologación")]
        public string NumHomologacion { get; set; }

        [Display(Name = "Fecha de Construcción")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-yyyy}")]
        public Nullable<System.DateTime> FechaConstruccion { get; set; }

        [Display(Name = "País de Fabricación")]
        public string PaisFabricacion { get; set; }

        [Display(Name = "Tipo IMO")]
        public string NumTipoIMO { get; set; }

        [Display(Name = "País de Aprobación")]
        public string PaisAprobacion { get; set; }

        [Display(Name = "Código de Diseño")]
        public string CodigoDiseno { get; set; }

        [Display(Name = "Fecha 1ª Prueba Hidráulica")]
        public Nullable<System.DateTime> PruebaHidraulica { get; set; }

        [Display(Name = "Presión de servicio ADR")]
        public Nullable<decimal> PresionServicioADR { get; set; }

        [Display(Name = "Presión de servicio IMO")]
        public Nullable<decimal> PresionServicioIMO { get; set; }

        [Display(Name = "Presión Máx. Exterior")]
        public Nullable<decimal> PresionExterior { get; set; }

        [Display(Name = "Presión Tarado Válvulas")]
        public Nullable<decimal> PresionTaradoValvulas { get; set; }

        [Display(Name = "Temperatura Cálculo Referencia")]
        public string TemperaturaCalculoReferencia { get; set; }

        [Display(Name = "Peso Bruto")]
        public Nullable<int> PesoBruto { get; set; }

        [Display(Name = "T.A.R.A.")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:##,###}")]
        public Nullable<int> Tara { get; set; }

        [Display(Name = "Peso Máx. Contenido")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N0}")]
        public Nullable<int> PesoMaxProducto { get; set; }

        [Display(Name = "Material Fabricación")]
        public Nullable<int> MaterialExteriorID { get; set; }

        [Display(Name = "Espesor del Cuerpo")]
        public Nullable<decimal> EspesorCuerpo { get; set; }

        [Display(Name = "Espesor de los Fondos")]
        public Nullable<decimal> EspesorFondo { get; set; }

        [Display(Name = "Espesor Equivalente")]
        public string EspesorEquivalente { get; set; }

        [Display(Name = "Material Parasol")]
        public string TipoForro { get; set; }

        [Display(Name = "Núm. Aprobación CSC")]
        public string NumAprobacionCSC { get; set; }

        [Display(Name = "Peso Máx. de Apilamiento")]
        public Nullable<int> PesoMaxApilamiento { get; set; }

        [Display(Name = "Carga de Rigidez")]
        public Nullable<int> CargaRigidez { get; set; }

        [Display(Name = "Presión de Prueba")]
        public Nullable<decimal> PresionPrueba { get; set; }

        [Display(Name = "Temperatura Mínima de Carga")]
        public string TemperaturaMinCarga { get; set; }

        [Display(Name = "Matrícula Plataforma")]
        public Nullable<int> PlataformaID { get; set; }

        [Display(Name = "Núm de Aprobación IMDG")]
        public string NumAprobacionIMDG { get; set; }

        [Display(Name = "Núm. de Aprobación ADR/RID")]
        public string NumAprobacionADR_RID { get; set; }

        [Display(Name = "Núm. de Aprobación")]
        public string NumAprobacion { get; set; }
    }
   
}