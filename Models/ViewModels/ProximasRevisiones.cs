using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CasaGaillard.Models;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models.ViewModels
{
    public class ProximasRevisionesViewModel
    {
        public IEnumerable<Cuba> Cubas { get; set; }
        public IEnumerable<Revision> Revisiones { get; set; }

    }

    public class RevisionHistorialItemViewModel
    {
        public int ID { get; set; }
        public DateTime FechaRevision { get; set; }
        public string Descripcion { get; set; }
        public DateTime? ValidaHasta { get; set; }
        public string DescripcionProxima { get; set; }
        public string Autorizado { get; set; }
    }

    public class RevisionCubaGrupoViewModel
    {
        public int CubaID { get; set; }
        public string MatriculaCuba { get; set; }
        public RevisionHistorialItemViewModel UltimaRevision { get; set; }
        public List<RevisionHistorialItemViewModel> Historial { get; set; } = new List<RevisionHistorialItemViewModel>();
    }

    public class RevisionVehiculoHistorialItemViewModel
    {
        public int ID { get; set; }
        public int? TipoRevisionID { get; set; }
        public string TipoRevision { get; set; }
        public DateTime? FechaRevision { get; set; }
        public DateTime? Caducidad { get; set; }
        public string Detalles { get; set; }
        public string Ejecutor { get; set; }
    }

    public class RevisionVehiculoTipoGrupoViewModel
    {
        public int? TipoRevisionID { get; set; }
        public string TipoRevision { get; set; }
        public RevisionVehiculoHistorialItemViewModel UltimaRevision { get; set; }
        public List<RevisionVehiculoHistorialItemViewModel> Historial { get; set; } = new List<RevisionVehiculoHistorialItemViewModel>();
    }

    public class RevisionVehiculoGrupoViewModel
    {
        public int VehiculoID { get; set; }
        public string MatriculaVehiculo { get; set; }
        public List<RevisionVehiculoTipoGrupoViewModel> Tipos { get; set; } = new List<RevisionVehiculoTipoGrupoViewModel>();
        public int TotalRevisiones { get; set; }
    }

    public class ProximaRevisionVehiculoViewModel
    {
        public string MatriculaVehiculo { get; set; }
        public string TipoRevision { get; set; }
        public DateTime? Caducidad { get; set; }
    }
}
