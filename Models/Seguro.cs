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
    
    public partial class Seguro
    {
        public int ID { get; set; }
        public string Compania { get; set; }
        public string NumPoliza { get; set; }
        public Nullable<decimal> Importe { get; set; }
        public Nullable<System.DateTime> FechaInicio { get; set; }
        public Nullable<int> PeriodoCubiertoMeses { get; set; }
        public Nullable<System.DateTime> FechaVto { get; set; }
        public string Observaciones { get; set; }
    }
}