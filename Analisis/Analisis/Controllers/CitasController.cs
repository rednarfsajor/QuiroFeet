using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Analisis.Controllers
{
    public class CitasController : Controller
    {
        // GET: Citas
        public ActionResult Calendar()
        {
            return View();
        }

        
        public ActionResult Day(String fecha)
        {
            ViewBag.Fecha = fecha;
            return View();
        }


        public ActionResult CreateDate(String fecha)
        {
            ViewBag.Fecha = fecha;
            return View();
        }
    }
}