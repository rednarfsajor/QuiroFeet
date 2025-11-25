using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Analisis.BD;

namespace Analisis.Controllers
{
    public class ClientesController : Controller
    {
        QuiroFeetEntities6 db = new QuiroFeetEntities6();

        // GET: Clientes (opcional se puede usar como dashboard)
        public ActionResult ClientIndex()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // LISTAR CLIENTES
        public ActionResult ListClients()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var clientes = db.Clientes.ToList();
            return View(clientes);
        }

        // CREAR CLIENTE - GET
        [HttpGet]
        public ActionResult CreateClient()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // CREAR CLIENTE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateClient(Clientes nuevoCliente)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                db.Clientes.Add(nuevoCliente);
                db.SaveChanges();
                return RedirectToAction("ListClients");
            }

            return View(nuevoCliente);
        }

        // EDITAR CLIENTE - GET
        [HttpGet]
        public ActionResult EditClient(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cliente = db.Clientes.Find(id);
            if (cliente == null)
                return HttpNotFound();

            return View(cliente);
        }

        // EDITAR CLIENTE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditClient(Clientes clienteEditado)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                db.Entry(clienteEditado).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ListClients");
            }

            return View(clienteEditado);
        }

        // ELIMINAR CLIENTE - GET
        [HttpGet]
        public ActionResult DeleteClient(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cliente = db.Clientes.Find(id);
            if (cliente == null)
                return HttpNotFound();

            return View(cliente);
        }

        // ELIMINAR CLIENTE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteClient(int id, FormCollection form)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cliente = db.Clientes.Find(id);
            if (cliente == null)
                return HttpNotFound();

            db.Clientes.Remove(cliente);
            db.SaveChanges();
            return RedirectToAction("ListClients");
        }
    }
}
