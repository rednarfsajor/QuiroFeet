using System;
using System.Data.Entity; // Para Include
using System.Linq;
using System.Web.Mvc;
using Analisis.BD;

namespace QuiroFeet.Controllers
{
    public class InventarioController : Controller
    {
        private QuiroFeetEntities1 db = new QuiroFeetEntities1();

        // Acción principal: Mostrar la vista de inventario con la lista de productos activos
        public ActionResult Inventario()
        {
            var productos = db.Productos
                             .Include(p => p.Proveedores)  // Asumiendo que la propiedad navegación es Proveedor (singular)
                             .Where(p => p.activo == 1)  // Solo productos activos
                             .ToList();

            return View(productos);
        }

        // GET: Mostrar formulario para registrar nuevo producto
        public ActionResult RegistrarProducto()
        {
            ViewBag.IdProveedor = new SelectList(db.Proveedores.ToList(), "Id", "Nombre");
            return View();
        }

        // POST: Recibir datos del formulario para registrar nuevo producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarProducto(Productos producto)
        {
            if (ModelState.IsValid)
            {
                producto.activo = 1; // Marcar como activo al crearlo

                db.Productos.Add(producto);
                db.SaveChanges();
                Productos productoBuscado = db.Productos.OrderByDescending(elproducto => elproducto.id).FirstOrDefault();

                // Crear registro en inventario con stock inicial en 0
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

            // Si hay error, recargar lista de proveedores para el dropdown
            ViewBag.IdProveedor = new SelectList(db.Proveedores.ToList(), "Id", "Nombre", producto.id_proveedor);
            return View(producto);
        }

        //EDitar
        public ActionResult EditarProducto(int id)
        {
            var producto = db.Productos.Find(id);
            if (producto == null)
                return HttpNotFound();

            var inventario = db.Inventario.FirstOrDefault(i => i.id_producto == id);
            ViewBag.Inventario = inventario;

            ViewBag.IdProveedor = new SelectList(db.Proveedores.ToList(), "Id", "Nombre", producto.id_proveedor);
            return View(producto);
        }


        // POST: Guardar cambios de edición
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarProducto(Productos producto, int stock)
        {
            if (ModelState.IsValid)
            {
                

                var inventario = db.Inventario.FirstOrDefault(i => i.id_producto == producto.id);
                if (inventario != null)
                {
                    inventario.stock = stock;
                    //inventario.publico = publico;
                    db.Entry(inventario).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("Inventario");
            }

            ViewBag.IdProveedor = new SelectList(db.Proveedores.ToList(), "Id", "Nombre", producto.id_proveedor);
            return View(producto);
        }


        //// GET: Confirmar inactivar producto
        //public ActionResult InactivarProducto(int id)
        //{
        //    var producto = db.Productos.Find(id);
        //    if (producto == null)
        //        return HttpNotFound();

        //    return View(producto);
        //}

        //// POST: Inactivar producto (cambiar activo a 0)
        //[HttpPost, ActionName("InactivarProducto")]
        //[ValidateAntiForgeryToken]
        //public ActionResult InactivarProductoConfirmado(int id)
        //{
        //    var producto = db.Productos.Find(id);
        //    if (producto == null)
        //        return HttpNotFound();

        //    producto.activo = 0;
        //    db.SaveChanges();

        //    return RedirectToAction("Inventario");
        //}


        // GET: Inventario/EliminarProducto/5
        public ActionResult EliminarProducto(int id)
        {
            // Buscar inventario asociado y eliminarlo
            var inventario = db.Inventario.FirstOrDefault(i => i.id_producto == id);
            if (inventario != null)
            {
                db.Inventario.Remove(inventario);
            }

            // Buscar producto y eliminarlo
            var producto = db.Productos.Find(id);
            if (producto != null)
            {
                db.Productos.Remove(producto);
            }

            db.SaveChanges();

            return RedirectToAction("Inventario");
        }



        // Acciones adicionales (solo vistas por ahora)
        public ActionResult RegistrarIngreso()
        {
            return View();
        }

        public ActionResult RegistrarSalida()
        {
            return View();
        }

        public ActionResult HistorialMovimientos()
        {
            return View();
        }

        //alertas
        public ActionResult AlertasStock()
        {
            int umbral = 5; // Puedes ajustar este valor según lo necesites

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
