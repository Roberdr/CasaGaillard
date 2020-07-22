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
    
    public partial class Compartimento
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Compartimento()
        {
            this.Grupos = new HashSet<Grupo>();
        }
    
        public int ID { get; set; }
        public Nullable<int> CubaID { get; set; }
        public Nullable<int> Numero { get; set; }
        public Nullable<int> Capacidad { get; set; }
        public Nullable<int> ProductoID { get; set; }
        public Nullable<int> MaterialInteriorID { get; set; }
    
        public virtual Cuba Cuba { get; set; }
        public virtual Material Material { get; set; }
        public virtual Producto Productos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Grupo> Grupos { get; set; }
    }
}
