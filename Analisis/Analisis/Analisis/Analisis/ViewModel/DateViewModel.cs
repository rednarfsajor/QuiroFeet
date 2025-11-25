using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Analisis.ViewModel
{
    public class DateViewModel
    {
        public int id { get; set; }
        public System.DateTime fecha { get; set; }
        public System.TimeSpan hora { get; set; }
  
        public string Estado { get; set; }

        
        public string NombreCliente { get; set; }
        public string NombreProfesional { get; set; }
        public string NombreServicio { get; set; }
    }
}