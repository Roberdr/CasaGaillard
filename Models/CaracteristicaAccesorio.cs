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
    
    public partial class CaracteristicaAccesorio
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CaracteristicaAccesorio()
        {
            this.DetalleAccesorios = new HashSet<DetalleAccesorio>();
        }
    
        public int ID { get; set; }
        public string CaracteristicaAccesorio1 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetalleAccesorio> DetalleAccesorios { get; set; }
    }
}