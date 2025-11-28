using System;
using System.Data.Entity; // Para Include
using System.Linq;
using System.Web.Mvc;
using Analisis.BD;

namespace QuiroFeet.Controllers
{
    public class InventarioController : Controller
    {
        private QuiroFeetEntities6 db = new QuiroFeetEntities6();

        // LISTADO PRINCIPAL: Productos (activos e inactivos) con paginación
        public ActionResult Inventario(int page = 1, int pageSize = 10)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            // Mostramos todos para poder ver estado y reactivar
            var query = db.Productos
                          .Include(p => p.Proveedores);

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (totalPages == 0) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var productos = query
                .OrderBy(p => p.id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.PageNumber = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(productos);
        }

        // GET: Registrar nuevo producto
        public ActionResult RegistrarProducto()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            ViewBag.IdProveedor = new SelectList(db.Proveedores.ToList(), "Id", "Nombre");
            return View();
        }

        // POST: Registrar nuevo producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarProducto(Productos producto)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                // Siempre se crea activo
                producto.activo = 1;

                db.Productos.Add(producto);
                db.SaveChanges();

                var inventario = new Inventario
                {
                    id_producto = producto.id,
                    stock = 0,
                    publico = 0
                };

                db.Inventario.Add(inventario);
                db.SaveChanges();

                return RedirectToAction("Inventario");
            }

            ViewBag.IdProveedor = new SelectList(db.Proveedores.ToList(), "Id", "Nombre", producto.id_proveedor);
            return View(producto);
        }

        // GET: Editar producto
        public ActionResult EditarProducto(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var producto = db.Productos.Find(id);
            if (producto == null)
                return HttpNotFound();

            var inventario = db.Inventario.FirstOrDefault(i => i.id_producto == id);
            ViewBag.Inventario = inventario;

            ViewBag.IdProveedor = new SelectList(db.Proveedores.ToList(), "Id", "Nombre", producto.id_proveedor);
            return View(producto);
        }

        // POST: Editar producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarProducto(Productos producto, int stock)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var productoDb = db.Productos.Find(producto.id);
                if (productoDb == null)
                    return HttpNotFound();

                productoDb.nombre = producto.nombre;
                productoDb.descripcion = producto.descripcion;
                productoDb.precio = producto.precio;
                productoDb.id_proveedor = producto.id_proveedor;

                db.Entry(productoDb).State = EntityState.Modified;

                var inventario = db.Inventario.FirstOrDefault(i => i.id_producto == producto.id);
                if (inventario != null)
                {
                    inventario.stock = stock;
                    db.Entry(inventario).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("Inventario");
            }

            var invent = db.Inventario.FirstOrDefault(i => i.id_producto == producto.id);
            ViewBag.Inventario = invent;

            ViewBag.IdProveedor = new SelectList(db.Proveedores.ToList(), "Id", "Nombre", producto.id_proveedor);
            return View(producto);
        }

        // ========================
        //   INACTIVAR / ACTIVAR
        // ========================

        // GET: InactivarProducto
        // Se llama desde SweetAlert: /Inventario/InactivarProducto/{id}
        [HttpGet]
        public ActionResult InactivarProducto(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var producto = db.Productos.Find(id);
            if (producto == null)
                return HttpNotFound();

            // Si ya está inactivo, solo regresamos
            if (producto.activo == 0)
                return RedirectToAction("Inventario");

            producto.activo = 0;
            db.Entry(producto).State = EntityState.Modified;
            db.SaveChanges();

            // Podrías usar TempData si quieres mostrar un mensaje en la vista
            // TempData["InventarioMensaje"] = "Producto inactivado correctamente.";

            return RedirectToAction("Inventario");
        }

        // GET: ActivarProducto (reactivar sin vista intermedia)
        [HttpGet]
        public ActionResult ActivarProducto(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var producto = db.Productos.Find(id);
            if (producto == null)
                return HttpNotFound();

            producto.activo = 1;
            db.Entry(producto).State = EntityState.Modified;
            db.SaveChanges();

            // TempData["InventarioMensaje"] = "Producto activado nuevamente.";

            return RedirectToAction("Inventario");
        }

        // (Opcional) ELIMINAR PRODUCTO FÍSICAMENTE – casi no lo usarías ya
        public ActionResult EliminarProducto(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var inventario = db.Inventario.FirstOrDefault(i => i.id_producto == id);
            if (inventario != null)
            {
                db.Inventario.Remove(inventario);
            }

            var producto = db.Productos.Find(id);
            if (producto != null)
            {
                db.Productos.Remove(producto);
            }

            db.SaveChanges();
            return RedirectToAction("Inventario");
        }

        // VISTAS ADICIONALES
        public ActionResult RegistrarIngreso()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        public ActionResult RegistrarSalida()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        public ActionResult HistorialMovimientos()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        public ActionResult AlertasStock()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            int umbral = 5;

            var alertas = db.Inventario
                .Where(i => i.stock <= umbral)
                .Select(i => new
                {
                    Inventario = i,
                    Producto = i.Productos
                })
                .ToList()
                .Select(x => new Tuple<Inventario, Productos>(x.Inventario, x.Producto));

            return View(alertas);
        }

        public ActionResult CrearOrdenCompra()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

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
