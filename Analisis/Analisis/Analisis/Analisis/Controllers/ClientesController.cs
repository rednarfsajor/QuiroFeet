using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Analisis.BD;
using System.Data.Entity;

namespace Analisis.Controllers
{
    public class ClientesController : Controller
    {
        // Contexto
        QuiroFeetEntities6 db = new QuiroFeetEntities6();

        // ============================
        // LISTADO PRINCIPAL (PAGINADO)
        // ============================
        public ActionResult ClientIndex(int page = 1)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            const int pageSize = 10;

            // Solo clientes activos
            var query = db.Clientes
                          .Where(c => c.activo)
                          .OrderBy(c => c.nombre);

            int totalRegistros = query.Count();

            var clientes = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Variables para la vista
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);

            return View(clientes);
        }

        // Compatibilidad
        public ActionResult ListClients()
        {
            return RedirectToAction("ClientIndex");
        }

        // ============================
        // CREAR CLIENTE
        // ============================

        [HttpGet]
        public ActionResult CreateClient()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateClient(Clientes nuevoCliente)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                // crear activo por defecto
                nuevoCliente.activo = true;

                db.Clientes.Add(nuevoCliente);
                db.SaveChanges();

                return RedirectToAction("ClientIndex");
            }

            return View(nuevoCliente);
        }

        // ============================
        // EDITAR CLIENTE
        // ============================

        [HttpGet]
        public ActionResult EditClient(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var cliente = db.Clientes.Find(id);
            if (cliente == null)
                return HttpNotFound();

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditClient(Clientes clienteEditado)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(clienteEditado);

            var clienteDb = db.Clientes.Find(clienteEditado.id);
            if (clienteDb == null)
                return HttpNotFound();

            // Campos editables
            clienteDb.nombre = clienteEditado.nombre;
            clienteDb.cedula = clienteEditado.cedula;
            clienteDb.telefono = clienteEditado.telefono;

            // Correo NO se modifica

            db.SaveChanges();

            return RedirectToAction("ClientIndex");
        }

        // ============================
        // INACTIVAR (SOFT DELETE)
        // ============================

        [HttpGet]
        public ActionResult InactivarClient(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var cliente = db.Clientes.Find(id);
            if (cliente == null)
                return HttpNotFound();

            return View(cliente); // Vista: InactivarClient.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InactivarClient(int id, FormCollection form)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var cliente = db.Clientes.Find(id);
            if (cliente == null)
                return HttpNotFound();

            cliente.activo = false;
            db.SaveChanges();

            return RedirectToAction("ClientIndex");
        }

        // ============================
        // DISPOSE
        // ============================

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
