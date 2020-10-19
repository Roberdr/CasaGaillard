using System;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(VehiculoMetaData))]
    public partial class Vehiculo
    {
    }
    public class VehiculoMetaData
    {
        [Display(Name = "Matrícula")]
        public string MatriculaVehiculo { get; set; }

        [Display(Name = "Modelo Tacógrafo")]
        public string ModeloTacografo { get; set; }

        [Display(Name = "P.M.A.")]
        public Nullable<int> Pma { get; set; }

        [Display(Name = "T.A.R.A.")]
        public Nullable<int> Tara { get; set; }

        [Display(Name = "Fecha Compra")]
        public Nullable<System.DateTime> FechaCompra { get; set; }

        [Display(Name = "Carga Útil")]
        public Nullable<int> CargaUtil { get; set; }

        [Display(Name = "Km Fecha Compra")]
        public Nullable<int> KilometrajeCompra { get; set; }

        [Display(Name = "Núm. Bastidor")]
        public string NumBastidor { get; set; }

        [Display(Name = "Núm. Ejes")]
        public Nullable<int> NumEjes { get; set; }

        [Display(Name = "CV Potencia")]
        public Nullable<int> PotenciaCV { get; set; }

    }
}