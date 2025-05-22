using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Analisis.Controllers
{
    public class ServiciosController : Controller
    {
        // GET: Servicios
        public ActionResult ServiceIndex()
        {
            return View();
        }

        public ActionResult CreateService()
        {
            return View();
        }
        public ActionResult ListServices()
        {
            return View();
        }
        public ActionResult EditService()
        {
            return View();
        }
    }
}