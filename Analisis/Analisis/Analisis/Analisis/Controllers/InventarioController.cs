using System;
using System.Data.Entity; // Para Include y EntityState
using System.Linq;
using System.Web.Mvc;
using Analisis.BD;

namespace QuiroFeet.Controllers
{
    public class InventarioController : Controller
    {
        private QuiroFeetEntities6 db = new QuiroFeetEntities6();

        // ===============================
        // LISTADO PRINCIPAL INVENTARIO
        // ===============================
        public ActionResult Inventario()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var productos = db.Productos
                              .Include(p => p.Proveedores)
                              .Where(p => p.activo == 1) // solo activos
                              .ToList();

            return View(productos);
        }

        // ===============================
        // REGISTRAR PRODUCTO - GET
        // ===============================
        public ActionResult RegistrarProducto()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            ViewBag.IdProveedor = new SelectList(db.Proveedores.ToList(), "Id", "Nombre");
            return View();
        }

        // ===============================
        // REGISTRAR PRODUCTO - POST
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarProducto(Productos producto, int stock)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                // Producto nuevo siempre activo
                producto.activo = 1;

                db.Productos.Add(producto);
                db.SaveChanges(); // aquí ya tiene producto.id

                // Crear registro en Inventario usando el stock inicial
                var inventario = new Inventario
                {
                    id_producto = producto.id,
                    stock = stock,
                    publico = 0
                };

                db.Inventario.Add(inventario);
                db.SaveChanges();

                return RedirectToAction("Inventario");
            }

            ViewBag.IdProveedor = new SelectList(db.Proveedores.ToList(), "Id", "Nombre", producto.id_proveedor);
            return View(producto);
        }

        // ===============================
        // EDITAR PRODUCTO - GET
        // ===============================
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

        // ===============================
        // EDITAR PRODUCTO - POST
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarProducto(Productos producto, int stock)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                // Volver a llenar combos y datos de inventario si hay error
                var inventarioError = db.Inventario.FirstOrDefault(i => i.id_producto == producto.id);
                ViewBag.Inventario = inventarioError;
                ViewBag.IdProveedor = new SelectList(db.Proveedores.ToList(), "Id", "Nombre", producto.id_proveedor);
                return View(producto);
            }

            var productoDb = db.Productos.Find(producto.id);
            if (productoDb == null)
                return HttpNotFound();

            // Actualizar campos editables del producto
            productoDb.nombre = producto.nombre;
            productoDb.descripcion = producto.descripcion;
            productoDb.precio = producto.precio;
            productoDb.id_proveedor = producto.id_proveedor;

            db.Entry(productoDb).State = EntityState.Modified;

            // Actualizar stock en Inventario
            var inventario = db.Inventario.FirstOrDefault(i => i.id_producto == producto.id);
            if (inventario != null)
            {
                inventario.stock = stock;
                db.Entry(inventario).State = EntityState.Modified;
            }

            db.SaveChanges();
            return RedirectToAction("Inventario");
        }

        // ===============================
        // INACTIVAR PRODUCTO (SOFT DELETE)
        // ===============================

        // GET: muestra confirmación
        [HttpGet]
        public ActionResult InactivarProducto(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var producto = db.Productos.Find(id);
            if (producto == null)
                return HttpNotFound();

            return View(producto); // Vista: InactivarProducto.cshtml
        }

        // POST: realiza el cambio de estado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InactivarProducto(int id, FormCollection form)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var producto = db.Productos.Find(id);
            if (producto == null)
                return HttpNotFound();

            producto.activo = 0; // soft delete
            db.Entry(producto).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Inventario");
        }

        // ===============================
        // OTRAS VISTAS / FUNCIONALIDADES
        // ===============================

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

            // De momento vista estática / futura lógica de historial real
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

        // ===============================
        // ORDENES DE COMPRA
        // ===============================

        // GET: formulario
        public ActionResult CrearOrdenCompra()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: recibe datos del formulario de orden de compra
        [HttpPost]
        public ActionResult RegistrarOrdenCompra(string producto, int cantidad)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            // Aquí podrías guardar en una tabla OrdenesCompra.
            // De momento solo redirigimos al inventario.
            // TempData["MensajeOrden"] = "Orden creada correctamente.";

            return RedirectToAction("Inventario");
        }

        // ===============================
        // DISPOSE
        // ===============================
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
