//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CasaGaillard.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class revision
    {
        public int id { get; set; }
        public Nullable<int> cuba_id { get; set; }
        public Nullable<System.DateTime> fecha_revision { get; set; }
        public string descripcion { get; set; }
        public Nullable<System.DateTime> valida_hasta { get; set; }
        public string descripcion_proxima { get; set; }
        public string autorizado { get; set; }
    
        public virtual cuba cuba { get; set; }
    }
}
