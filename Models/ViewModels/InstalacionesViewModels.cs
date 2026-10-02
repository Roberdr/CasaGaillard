using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CasaGaillard.Models.ViewModels
{
    public class InstalacionFilaViewModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string TipoNombre { get; set; }
        public string UbicacionNombre { get; set; }
        public string Estado { get; set; }
        public byte Criticidad { get; set; }
        public int NumeroEquipos { get; set; }
    }

    public class EquipoFilaViewModel
    {
        public int Id { get; set; }
        public int InstalacionId { get; set; }
        public string InstalacionNombre { get; set; }
        public int Nivel { get; set; }
        public string TipoElemento { get; set; }
        public string PadreNombre { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Fabricante { get; set; }
        public string Modelo { get; set; }
        public string NumeroSerie { get; set; }
        public bool Activo { get; set; }
    }

    public class InstalacionFormViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Required, StringLength(255)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [StringLength(2000)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Tipo de instalación")]
        public int? TipoInstalacionId { get; set; }

        [Display(Name = "Ubicación")]
        public int? UbicacionId { get; set; }

        [Display(Name = "Responsable")]
        public int? ResponsablePersonaId { get; set; }

        [Display(Name = "Fecha de puesta en marcha")]
        [DataType(DataType.Date)]
        public DateTime? FechaPuestaMarcha { get; set; }

        [Display(Name = "Fecha de baja")]
        [DataType(DataType.Date)]
        public DateTime? FechaBaja { get; set; }

        [Range(1, 5)]
        [Display(Name = "Criticidad (1 a 5)")]
        public byte Criticidad { get; set; } = 3;

        [Required, StringLength(20)]
        public string Estado { get; set; } = "ACTIVA";

        [StringLength(50)]
        [Display(Name = "Código de plano")]
        public string CodigoPlano { get; set; }

        [Display(Name = "Crítica para producción")]
        public bool CriticaProduccion { get; set; }

        [Display(Name = "Crítica para seguridad")]
        public bool CriticaSeguridad { get; set; }

        [StringLength(4000)]
        public string Observaciones { get; set; }

        public IEnumerable<SelectListItem> Tipos { get; set; }
        public IEnumerable<SelectListItem> Ubicaciones { get; set; }
        public IEnumerable<SelectListItem> Responsables { get; set; }
    }

    public class EquipoFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Instalación")]
        public int InstalacionId { get; set; }

        [Display(Name = "Equipo padre")]
        public int? EquipoPadreId { get; set; }

        [Required, StringLength(30)]
        [Display(Name = "Tipo de elemento")]
        public string TipoElemento { get; set; } = "EQUIPO";

        [Required, StringLength(50)]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Required, StringLength(255)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [StringLength(2000)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [StringLength(150)] public string Fabricante { get; set; }
        [StringLength(150)] public string Modelo { get; set; }
        [StringLength(100)] [Display(Name = "Número de serie")] public string NumeroSerie { get; set; }
        [Range(typeof(decimal), "0", "999999999999999")] public decimal? Potencia { get; set; }
        [Range(typeof(decimal), "0", "999999999999999")] public decimal? Caudal { get; set; }
        [Range(typeof(decimal), "0", "999999999999999")] public decimal? Presion { get; set; }
        [Range(typeof(decimal), "0", "999999999999999")] public decimal? Voltaje { get; set; }

        [Display(Name = "Fecha de instalación")]
        [DataType(DataType.Date)]
        public DateTime? FechaInstalacion { get; set; }

        [Display(Name = "Fecha de baja")]
        [DataType(DataType.Date)]
        public DateTime? FechaBaja { get; set; }

        public bool Activo { get; set; } = true;

        [StringLength(4000)]
        public string Observaciones { get; set; }

        public IEnumerable<SelectListItem> Instalaciones { get; set; }
        public IEnumerable<SelectListItem> EquiposPadre { get; set; }
        public IEnumerable<SelectListItem> TiposElemento { get; set; }
    }
}
