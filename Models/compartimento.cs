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
    
    public partial class compartimento
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public compartimento()
        {
            this.grupoes = new HashSet<grupo>();
        }
    
        public int id { get; set; }
        public Nullable<int> cuba_id { get; set; }
        public Nullable<int> numero { get; set; }
        public Nullable<int> capacidad { get; set; }
        public Nullable<int> producto_id { get; set; }
        public Nullable<int> material_interior_id { get; set; }
    
        public virtual cuba cuba { get; set; }
        public virtual material material { get; set; }
        public virtual producto producto { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<grupo> grupoes { get; set; }
    }
}