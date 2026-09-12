using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CasaGaillard.Models.ViewModels
{
    public class VehiculosIndexViewModel
    {
        public IPagedList<Vehiculo> PagedList { get; set; }
    }
}