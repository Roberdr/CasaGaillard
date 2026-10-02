using System;
using System.Collections.Generic;
using System.Web;

namespace CasaGaillard.Models.ViewModels
{
    public class InstalacionEquipoFotosViewModel
    {
        public string Tipo { get; set; }
        public int ActivoID { get; set; }
        public string Titulo { get; set; }
        public IEnumerable<InstalacionEquipoFotoFilaViewModel> Fotos { get; set; }
    }

    public class InstalacionEquipoFotoFilaViewModel
    {
        public int ID { get; set; }
        public string NombreOriginal { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
