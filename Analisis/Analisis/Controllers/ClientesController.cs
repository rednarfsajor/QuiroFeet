using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Analisis.Controllers
{
    public class ClientesController : Controller
    {
        // GET: Clientes
        public ActionResult ClientIndex()
        {
            return View();
        }

        public ActionResult CreateClient()
        {
            return View();
        }

        public ActionResult EditClient()
        {
            return View();
        }

        public ActionResult ListClients()
        {
            return View();
        }

    }
}