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
        private QuiroFeetEntities5 db = new QuiroFeetEntities5();

        public ActionResult ConfirmSale()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var venta = TempData["ReciboVenta"] as Ventas;

            if (venta == null)
                return RedirectToAction("ErrorSale");

            var ventaConCliente = db.Ventas
                .Include(v => v.Clientes)
                .FirstOrDefault(v => v.id == venta.id);

            return View(ventaConCliente);
        }

        [HttpPost]
        public ActionResult ConfirmSale(Ventas venta)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            venta.fecha = DateTime.Now;
            venta.NumeroRecibo = new Random().Next(100000, 999999).ToString();
            db.Ventas.Add(venta);
            db.SaveChanges();

            TempData["ReciboVenta"] = venta;

            return RedirectToAction("ConfirmSale");
        }

        public ActionResult ErrorSale()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        public ActionResult ListSales(string busqueda = "")
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var ventas = db.Ventas.Include(v => v.Clientes).AsQueryable();

            if (!string.IsNullOrEmpty(busqueda))
            {
                ventas = ventas.Where(v =>
                    v.Clientes.nombre.Contains(busqueda) ||
                    v.detalle.Contains(busqueda));
            }

            return View(ventas.OrderByDescending(v => v.fecha).ToList());
        }

        [HttpGet]
        public ActionResult RegisterSale()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

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

            for (int i = 0; i < productosSeleccionados.Count; i++)
            {
                int idProducto = productosSeleccionados[i];
                int cantidad = cantidades[i];

                var inventario = db.Inventario
                    .Include(inv => inv.Productos)
                    .FirstOrDefault(inv => inv.id_producto == idProducto);

                if (inventario == null || inventario.stock < cantidad)
                {
                    TempData["Error"] = $"Stock insuficiente para producto ID {idProducto}";
                    return RedirectToAction("ErrorSale");
                }

                total += inventario.Productos.precio.GetValueOrDefault() * cantidad;
                detalle += $"{inventario.Productos.nombre} x{cantidad}, ";
            }

            var venta = new Ventas
            {
                id_cliente = id_cliente,
                servicio = "False",
                detalle = detalle.TrimEnd(',', ' '),
                monto = total,
                fecha = DateTime.Now,
                NumeroRecibo = new Random().Next(100000, 999999).ToString()
            };

            db.Ventas.Add(venta);
            db.SaveChanges();

            // Actualización de inventario
            for (int j = 0; j < productosSeleccionados.Count; j++)
            {
                int idProducto = productosSeleccionados[j];
                int cantidad = cantidades[j];

                var inventario = db.Inventario.FirstOrDefault(inv => inv.id_producto == idProducto);
                if (inventario != null)
                {
                    inventario.stock -= cantidad;
                    db.Entry(inventario).State = EntityState.Modified;
                }
            }

            db.SaveChanges();

            // Asignación explícita del cliente para evitar errores en la vista
            venta.Clientes = db.Clientes.FirstOrDefault(c => c.id == id_cliente);

            TempData["ReciboVenta"] = venta;

            return RedirectToAction("ConfirmSale");
        }

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
                NumeroRecibo = new Random().Next(100000, 999999).ToString()
            };

            db.Ventas.Add(venta);
            db.SaveChanges();

            // Cargar cliente manualmente para evitar error en la vista
            venta.Clientes = db.Clientes.FirstOrDefault(c => c.id == id_cliente);

            TempData["ReciboVenta"] = venta;

            return RedirectToAction("ConfirmSale");
        }

        public ActionResult Sales()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

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
