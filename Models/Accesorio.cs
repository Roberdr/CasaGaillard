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
    
    public partial class Accesorio
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Accesorio()
        {
            this.DetallesAccesorio = new HashSet<DetalleAccesorio>();
            this.AccesoriosGrupo = new HashSet<AccesorioGrupo>();
        }
    
        public int ID { get; set; }
        public int TipoAccesorioID { get; set; }
        public int MaterialID { get; set; }
    
        public virtual Material Material { get; set; }
        public virtual TipoAccesorio TipoAccesorio { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetalleAccesorio> DetallesAccesorio { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AccesorioGrupo> AccesoriosGrupo { get; set; }
    }
}
