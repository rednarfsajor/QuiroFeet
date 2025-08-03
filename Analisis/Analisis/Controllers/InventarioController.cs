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

        // Acción principal: Mostrar la vista de inventario con la lista de productos activos
        public ActionResult Inventario()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var productos = db.Productos
                             .Include(p => p.Proveedores)
                             .Where(p => p.activo == 1)
                             .ToList();

            return View(productos);
        }

        // GET: Mostrar formulario para registrar nuevo producto
        public ActionResult RegistrarProducto()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            ViewBag.IdProveedor = new SelectList(db.Proveedores.ToList(), "Id", "Nombre");
            return View();
        }

        // POST: Recibir datos del formulario para registrar nuevo producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarProducto(Productos producto)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                producto.activo = 1;

                db.Productos.Add(producto);
                db.SaveChanges();

                Productos productoBuscado = db.Productos.OrderByDescending(elproducto => elproducto.id).FirstOrDefault();

                var inventario = new Inventario
                {
                    id_producto = productoBuscado.id,
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

        // EDITAR PRODUCTO - GET
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

        // EDITAR PRODUCTO - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarProducto(Productos producto, int stock)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var inventario = db.Inventario.FirstOrDefault(i => i.id_producto == producto.id);
                if (inventario != null)
                {
                    inventario.stock = stock;
                    db.Entry(inventario).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("Inventario");
            }

            ViewBag.IdProveedor = new SelectList(db.Proveedores.ToList(), "Id", "Nombre", producto.id_proveedor);
            return View(producto);
        }

        // ELIMINAR PRODUCTO
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
