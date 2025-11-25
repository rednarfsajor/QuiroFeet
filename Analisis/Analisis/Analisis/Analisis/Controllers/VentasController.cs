using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Analisis.BD;
using System.Data.Entity;

namespace Analisis.Controllers
{
    public class VentasController : Controller
    {
        private QuiroFeetEntities6 db = new QuiroFeetEntities6();

        // ==============================
        // CONFIRMACIÓN DE VENTA
        // ==============================
        [HttpGet]
        public ActionResult ConfirmSale()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            // Ahora solo guardamos el ID en TempData
            var ventaIdObj = TempData["ReciboVentaId"];
            if (ventaIdObj == null)
                return RedirectToAction("ErrorSale");

            int ventaId = (int)ventaIdObj;

            var ventaConDetalle = db.Ventas
                .Include(v => v.Clientes)
                .Include(v => v.DetalleVenta.Select(dv => dv.Productos))
                .FirstOrDefault(v => v.id == ventaId);

            if (ventaConDetalle == null)
                return RedirectToAction("ErrorSale");

            return View(ventaConDetalle);
        }

        public ActionResult ErrorSale()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            // Si quieres, aquí puedes leer TempData["Error"] y mostrarlo en la vista
            return View();
        }

        // ==============================
        // LISTADO DE VENTAS
        // ==============================
        public ActionResult ListSales(string busqueda = "")
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var ventas = db.Ventas
                           .Include(v => v.Clientes)
                           .AsQueryable();

            if (!string.IsNullOrEmpty(busqueda))
            {
                ventas = ventas.Where(v =>
                    (v.Clientes.nombre.Contains(busqueda)) ||
                    (v.detalle != null && v.detalle.Contains(busqueda))
                );
            }

            var lista = ventas
                .OrderByDescending(v => v.fecha)
                .ToList();

            return View(lista);
        }

        // ==============================
        // REGISTRO DE VENTA DE PRODUCTOS
        // ==============================
        [HttpGet]
        public ActionResult RegisterSale()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Clientes = new SelectList(db.Clientes, "id", "nombre");

            var productos = db.Productos
                .Include(p => p.Inventario)
                .Where(p => p.Inventario.Any(i => i.stock > 0))
                .ToList();

            ViewBag.Productos = productos;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterSale(int id_cliente, List<int> productosSeleccionados, List<int> cantidades)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (productosSeleccionados == null || !productosSeleccionados.Any())
            {
                TempData["Error"] = "Debe seleccionar al menos un producto.";
                return RedirectToAction("ErrorSale");
            }

            decimal total = 0;
            string detalle = "";

            var venta = new Ventas
            {
                id_cliente = id_cliente,
                servicio = "False",
                detalle = "",
                monto = 0,
                fecha = DateTime.Now,
                NumeroRecibo = new Random().Next(100000, 999999).ToString(),
                Estado = "Activo",
                DetalleVenta = new List<DetalleVenta>()
            };

            try
            {
                for (int i = 0; i < productosSeleccionados.Count; i++)
                {
                    int idProducto = productosSeleccionados[i];
                    int cantidad = cantidades[i];

                    var inventario = db.Inventario
                        .Include(inv => inv.Productos)
                        .FirstOrDefault(inv => inv.id_producto == idProducto);

                    if (inventario == null || inventario.stock < cantidad)
                    {
                        TempData["Error"] = $"Stock insuficiente para el producto seleccionado.";
                        return RedirectToAction("ErrorSale");
                    }

                    decimal precioUnitario = inventario.Productos.precio.GetValueOrDefault();
                    decimal monto = precioUnitario * cantidad;

                    // Detalle de venta
                    var detalleVenta = new DetalleVenta
                    {
                        id_producto = idProducto,
                        cantidad = cantidad,
                        monto = monto
                    };
                    venta.DetalleVenta.Add(detalleVenta);

                    total += monto;
                    detalle += $"{inventario.Productos.nombre} x{cantidad}, ";

                    // Actualizar stock
                    inventario.stock -= cantidad;
                    db.Entry(inventario).State = EntityState.Modified;
                }

                venta.monto = total;
                venta.detalle = detalle.TrimEnd(',', ' ');

                db.Ventas.Add(venta);
                db.SaveChanges();

                // Guardamos solo el ID para la vista de confirmación
                TempData["ReciboVentaId"] = venta.id;
                return RedirectToAction("ConfirmSale");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var error in ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors))
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Propiedad: {error.PropertyName}, Error: {error.ErrorMessage}");
                }
                TempData["Error"] = "Ocurrió un error al registrar la venta.";
                return RedirectToAction("ErrorSale");
            }
        }

        // ==============================
        // REGISTRO DE VENTA DE SERVICIO
        // ==============================
        [HttpGet]
        public ActionResult RegisterServiceSale()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Clientes = new SelectList(db.Clientes, "id", "nombre");
            ViewBag.Servicios = new SelectList(db.Servicios, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterServiceSale(int id_cliente, int id_servicio)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var servicio = db.Servicios.Find(id_servicio);
            if (servicio == null)
            {
                TempData["Error"] = "Servicio no encontrado.";
                return RedirectToAction("ErrorSale");
            }

            var venta = new Ventas
            {
                id_cliente = id_cliente,
                servicio = "True",
                detalle = servicio.Nombre,
                monto = servicio.Precio,
                fecha = DateTime.Now,
                NumeroRecibo = new Random().Next(100000, 999999).ToString(),
                Estado = "Activo"
            };

            try
            {
                db.Ventas.Add(venta);
                db.SaveChanges();

                TempData["ReciboVentaId"] = venta.id;
                return RedirectToAction("ConfirmSale");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var error in ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors))
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Propiedad: {error.PropertyName}, Error: {error.ErrorMessage}");
                }
                TempData["Error"] = "Ocurrió un error al registrar la venta de servicio.";
                return RedirectToAction("ErrorSale");
            }
        }

        // ==============================
        // MENÚ PRINCIPAL MÓDULO VENTAS
        // ==============================
        public ActionResult Sales()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // ==============================
        // EDITAR VENTA (CABECERA)
        // ==============================
        [HttpGet]
        public ActionResult EditSale(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var venta = db.Ventas.Find(id);
            if (venta == null)
                return HttpNotFound();

            ViewBag.Clientes = new SelectList(db.Clientes, "id", "nombre", venta.id_cliente);
            return View(venta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSale(Ventas venta)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                db.Entry(venta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ListSales");
            }

            ViewBag.Clientes = new SelectList(db.Clientes, "id", "nombre", venta.id_cliente);
            return View(venta);
        }

        // ==============================
        // ELIMINAR VENTA
        // ==============================
        public ActionResult DeleteSale(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var venta = db.Ventas.Find(id);
            if (venta == null)
                return HttpNotFound();

            db.Ventas.Remove(venta);
            db.SaveChanges();

            return RedirectToAction("ListSales");
        }
    }
}
