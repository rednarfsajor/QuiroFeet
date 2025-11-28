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
        private QuiroFeetEntities6 db = new QuiroFeetEntities6();

        // GET: Clientes - Listado con filtros y paginación
        public ActionResult ClientIndex(string search, string estado = "Todos", int page = 1, int pageSize = 10)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (pageSize <= 0) pageSize = 10;

            // Query base
            var query = db.Clientes.AsQueryable();

            // Búsqueda
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    (c.nombre ?? "").Contains(search) ||
                    (c.correo ?? "").Contains(search) ||
                    (c.telefono ?? "").Contains(search) ||
                    (c.cedula ?? "").Contains(search));
            }

            // Filtro por estado
            switch (estado)
            {
                case "Activos":
                    query = query.Where(c => c.activo == true);
                    break;

                case "Inactivos":
                    query = query.Where(c => c.activo == false);
                    break;

                default: // "Todos"
                    // No se filtra por activo
                    break;
            }

            // Paginación
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (totalPages == 0) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var clientes = query
                .OrderBy(c => c.id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Datos para la vista
            ViewBag.FiltroEstado = estado;
            ViewBag.CurrentSearch = search ?? "";
            ViewBag.PageNumber = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(clientes); // Vista ClientIndex.cshtml
        }

        // LISTAR CLIENTES (si la sigues usando para reportes, etc.)
        public ActionResult ListClients()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var clientes = db.Clientes.ToList();
            return View(clientes);
        }

        // ===================== CREAR CLIENTE =====================

        // GET: Clientes/CreateClient
        [HttpGet]
        public ActionResult CreateClient()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            // Por defecto siempre activo
            var modelo = new Clientes
            {
                activo = true
            };

            return View(modelo);
        }

        // POST: Clientes/CreateClient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateClient(Clientes nuevoCliente)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            // Siempre crear como activo
            nuevoCliente.activo = true;

            // Validación de duplicados (cédula, correo o teléfono)
            bool existeDuplicado = db.Clientes.Any(c =>
                (!string.IsNullOrEmpty(nuevoCliente.cedula) && c.cedula == nuevoCliente.cedula) ||
                (!string.IsNullOrEmpty(nuevoCliente.correo) && c.correo == nuevoCliente.correo) ||
                (!string.IsNullOrEmpty(nuevoCliente.telefono) && c.telefono == nuevoCliente.telefono)
            );

            if (existeDuplicado)
            {
                ModelState.AddModelError("",
                    "Ya existe un cliente con datos similares (cédula, correo o teléfono). Por favor verifica la información.");
            }

            if (!ModelState.IsValid)
            {
                return View(nuevoCliente);
            }

            db.Clientes.Add(nuevoCliente);
            db.SaveChanges();

            return RedirectToAction("ClientIndex");
        }

        // ===================== EDITAR CLIENTE =====================

        // GET: Clientes/EditClient/5
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

        // POST: Clientes/EditClient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditClient(Clientes clienteEditado)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                db.Entry(clienteEditado).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ClientIndex");
            }

            return View(clienteEditado);
        }

        // ===================== ELIMINAR CLIENTE (si lo usas) =====================

        [HttpGet]
        public ActionResult DeleteClient(int id)
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
        public ActionResult DeleteClient(int id, FormCollection form)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var cliente = db.Clientes.Find(id);
            if (cliente == null)
                return HttpNotFound();

            db.Clientes.Remove(cliente);
            db.SaveChanges();
            return RedirectToAction("ClientIndex");
        }

        // ===================== CONFIRMACIONES ACTIVAR / DESACTIVAR =====================

        // GET: Clientes/ConfirmarDesactivar/5
        [HttpGet]
        public ActionResult ConfirmarDesactivar(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var cliente = db.Clientes.Find(id);
            if (cliente == null)
                return HttpNotFound();

            return View(cliente);  // Vista con el modal bonito para confirmar inactivar
        }

        // GET: Clientes/ConfirmarActivar/5
        [HttpGet]
        public ActionResult ConfirmarActivar(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var cliente = db.Clientes.Find(id);
            if (cliente == null)
                return HttpNotFound();

            return View(cliente);  // Vista con el modal bonito para confirmar reactivar
        }

        // ===================== ACCIONES REALES ACTIVAR / DESACTIVAR =====================

        // POST: Clientes/DesactivarCliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DesactivarCliente(int id)
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

        // POST: Clientes/ActivarCliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActivarCliente(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var cliente = db.Clientes.Find(id);
            if (cliente == null)
                return HttpNotFound();

            cliente.activo = true;
            db.SaveChanges();

            return RedirectToAction("ClientIndex");
        }

        // ===================== DISPOSE =====================

        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
