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
    
    public partial class Cuba
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cuba()
        {
            this.Compartimentos = new HashSet<Compartimento>();
            this.Grupos = new HashSet<Grupo>();
            this.Revisions = new HashSet<Revision>();
            this.Operaciones = new HashSet<Operacion>();
        }
    
        public int ID { get; set; }
        public string MatriculaCuba { get; set; }
        public Nullable<int> NumCuadro { get; set; }
        public string Codigo { get; set; }
        public string Constructor { get; set; }
        public string NumFabricacion { get; set; }
        public string NumHomologacion { get; set; }
        public Nullable<System.DateTime> FechaConstruccion { get; set; }
        public string PaisFabricacion { get; set; }
        public string NumTipoIMO { get; set; }
        public string PaisAprobacion { get; set; }
        public string Autoridad { get; set; }
        public string CodigoDiseno { get; set; }
        public Nullable<System.DateTime> PruebaHidraulica { get; set; }
        public Nullable<decimal> PresionServicioADR { get; set; }
        public Nullable<decimal> PresionServicioIMO { get; set; }
        public Nullable<decimal> PresionExterior { get; set; }
        public Nullable<decimal> PresionTaradoValvulas { get; set; }
        public string TemperaturaCalculoReferencia { get; set; }
        public Nullable<int> PesoBruto { get; set; }
        public Nullable<int> Tara { get; set; }
        public Nullable<int> PesoMaxProducto { get; set; }
        public Nullable<int> MaterialExteriorID { get; set; }
        public Nullable<decimal> EspesorCuerpo { get; set; }
        public Nullable<decimal> EspesorFondo { get; set; }
        public string EspesorEquivalente { get; set; }
        public string TipoForro { get; set; }
        public string NumAprobacionCSC { get; set; }
        public string Modelo { get; set; }
        public Nullable<int> PesoMaxApilamiento { get; set; }
        public Nullable<int> CargaRigidez { get; set; }
        public Nullable<decimal> PresionPrueba { get; set; }
        public string TemperaturaMinCarga { get; set; }
        public Nullable<int> PlataformaID { get; set; }
        public Nullable<int> Longitud { get; set; }
        public Nullable<int> Ancho { get; set; }
        public Nullable<int> Alto { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string NumAprobacionIMDG { get; set; }
        public string NumAprobacionADR_RID { get; set; }
        public string UNPortableTank { get; set; }
        public string NumAprobacion { get; set; }
    
        public virtual Material Material { get; set; }
        public virtual Plataforma Plataforma { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Compartimento> Compartimentos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Grupo> Grupos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Revision> Revisions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Operacion> Operaciones { get; set; }
    }
}
