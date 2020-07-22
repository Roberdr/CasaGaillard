using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CasaGaillard.Models;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models.ViewModels
{
    public class ProximasRevisionesViewModel
    {
        public IEnumerable<Cuba> Cubas { get; set; }
        public IEnumerable<Revision> Revisiones { get; set; }

    }
}