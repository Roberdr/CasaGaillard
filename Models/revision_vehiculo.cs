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
    
    public partial class revision_vehiculo
    {
        public int id { get; set; }
        public Nullable<int> id_vehiculo { get; set; }
        public Nullable<int> id_tipo_revision { get; set; }
        public Nullable<System.DateTime> fecha_revision { get; set; }
        public string detalles { get; set; }
        public string ejecutor { get; set; }
    
        public virtual vehiculo vehiculo { get; set; }
        public virtual tipo_revision tipo_revision { get; set; }
    }
}
