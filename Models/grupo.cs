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
    
    public partial class grupo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public grupo()
        {
            this.accesorio_grupo = new HashSet<accesorio_grupo>();
        }
    
        public int id { get; set; }
        public Nullable<int> tipo_grupo_id { get; set; }
        public Nullable<int> cuba_id { get; set; }
        public Nullable<int> compartimento_id { get; set; }
        public Nullable<int> situacion_id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<accesorio_grupo> accesorio_grupo { get; set; }
        public virtual compartimento compartimento { get; set; }
        public virtual cuba cuba { get; set; }
        public virtual situacion situacion { get; set; }
        public virtual tipo_grupo tipo_grupo { get; set; }
    }
}