using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Analisis.BD;
using System.Data.Entity;

namespace Analisis.Controllers
{
    public class VentasController : Controller
    {
        private QuiroFeetEntities2 db = new QuiroFeetEntities2();

        // GET: Ventas/DeleteSale/5
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

        // GET: Ventas/ConfirmSale
        public ActionResult ConfirmSale()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var recibo = TempData["ReciboVenta"] as Ventas;
            return View(recibo);
        }

        // GET: Ventas/ErrorSale
        public ActionResult ErrorSale()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // GET: Ventas/ListSales
        public ActionResult ListSales(string busqueda = "")
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var ventas = db.Ventas.Include("Clientes").AsQueryable();

            if (!string.IsNullOrEmpty(busqueda))
            {
                ventas = ventas.Where(v =>
                    v.Clientes.nombre.Contains(busqueda) ||
                    v.detalle.Contains(busqueda)
                );
            }

            ViewBag.Busqueda = busqueda;

            return View(ventas.OrderByDescending(v => v.fecha).ToList());
        }

        // GET: Ventas/RegisterSale
        [HttpGet]
        public ActionResult RegisterSale()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Usuarios");

            ViewBag.Clientes = new SelectList(db.Clientes.ToList(), "id", "nombre");

            var productosConStock = db.Productos
                .Include(p => p.Inventario)
                .Where(p => p.Inventario.Any(i => i.stock > 0))
                .ToList();

            ViewBag.Productos = productosConStock;

            return View();
        }

        // POST: Ventas/RegisterSale
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterSale(int id_cliente, List<int> productosSeleccionados, List<int> cantidades)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (productosSeleccionados == null || productosSeleccionados.Count == 0)
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

                if (inventario == null || inventario.stock < cantidad || cantidad <= 0)
                {
                    TempData["Error"] = $"Stock insuficiente para el producto ID: {idProducto}";
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
                fecha = DateTime.Now
            };

            db.Ventas.Add(venta);
            db.SaveChanges();

            for (int i = 0; i < productosSeleccionados.Count; i++)
            {
                int idProducto = productosSeleccionados[i];
                int cantidad = cantidades[i];

                var inventario = db.Inventario.FirstOrDefault(inv => inv.id_producto == idProducto);
                if (inventario != null)
                {
                    inventario.stock -= cantidad;
                    db.Entry(inventario).State = EntityState.Modified;
                }
            }

            db.SaveChanges();
            TempData["ReciboVenta"] = venta;

            return RedirectToAction("ConfirmSale");
        }

        // GET: Ventas/RegisterServiceSale
        [HttpGet]
        public ActionResult RegisterServiceSale()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Clientes = new SelectList(db.Clientes, "id", "nombre");
            ViewBag.Servicios = new SelectList(db.Servicios, "Id", "Nombre");
            return View();
        }

        // POST: Ventas/RegisterServiceSale
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
                fecha = DateTime.Now
            };

            db.Ventas.Add(venta);
            db.SaveChanges();

            TempData["ReciboVenta"] = venta;
            return RedirectToAction("ConfirmSale");
        }

        // GET: Ventas/Sales (opcional)
        public ActionResult Sales()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // ✅ GET: Ventas/EditSale/5
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

        // Ventas/EditSale
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


    }

}
