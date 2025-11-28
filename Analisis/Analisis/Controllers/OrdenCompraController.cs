using Analisis.BD;
using Analisis.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Analisis.Controllers
{
    public class OrdenCompraController : Controller
    {
        private QuiroFeetEntities6 db = new QuiroFeetEntities6();

        // GET: /OrdenCompra/ListarOrdenes
        public ActionResult ListarOrdenes(string estado = "Todos", string search = "", int page = 1, int pageSize = 10)
        {
            var query = db.OrdenesCompra
                          .Include(o => o.Proveedores)
                          .AsQueryable();

            // Búsqueda por ID de orden o nombre de proveedor
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(o =>
                    o.id_orden.ToString().Contains(search) ||
                    (o.Proveedores != null && o.Proveedores.nombre.Contains(search)));
            }

            // Filtro por estado
            if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
            {
                query = query.Where(o =>
                    o.status != null &&
                    o.status.Equals(estado, StringComparison.OrdinalIgnoreCase));
            }

            // Totales para la paginación
            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (totalPages == 0) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var ordenes = query
                .OrderByDescending(o => o.fecha_creacion)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Datos para la vista
            ViewBag.FiltroEstado = estado;
            ViewBag.CurrentSearch = search;
            ViewBag.PageNumber = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(ordenes);
        }

        // GET: /OrdenCompra/CrearOrdenCompra
        public ActionResult CrearOrdenCompra()
        {
            var model = new OrdenCompraViewModel
            {
                fecha_creacion = DateTime.Now,
                Productos = new List<DetalleOrdenVM> { new DetalleOrdenVM() }
            };

            ViewBag.Proveedores = new SelectList(db.Proveedores, "id", "nombre");
            return View(model);
        }

        // POST: /OrdenCompra/CrearOrdenCompra
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearOrdenCompra(OrdenCompraViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Proveedores = new SelectList(db.Proveedores, "id", "nombre", model.proveedor_id);
                return View(model);
            }

            try
            {
                var orden = new OrdenesCompra
                {
                    proveedor_id = model.proveedor_id,
                    fecha_creacion = model.fecha_creacion,
                    status = "Pendiente",
                    total = 0
                };

                db.OrdenesCompra.Add(orden);
                db.SaveChanges(); // Para obtener id_orden

                decimal totalOrden = 0;

                foreach (var p in model.Productos)
                {
                    if (!decimal.TryParse(p.precio, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precioDecimal))
                    {
                        ModelState.AddModelError("", "Error al convertir el precio.");
                        ViewBag.Proveedores = new SelectList(db.Proveedores, "id", "nombre", model.proveedor_id);
                        return View(model);
                    }

                    var detalle = new DetalleOrden
                    {
                        id_orden = orden.id_orden,
                        id_producto = p.id_producto,
                        qty = p.qty,
                        precio_unidad = precioDecimal,
                        subtotal = precioDecimal * p.qty
                    };

                    totalOrden += detalle.subtotal ?? 0;
                    db.DetalleOrden.Add(detalle);
                }

                orden.total = totalOrden;
                db.SaveChanges();

                TempData["Success"] = "Orden registrada correctamente.";
                return RedirectToAction("ListarOrdenes");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar: " + ex.Message);
                ViewBag.Proveedores = new SelectList(db.Proveedores, "id", "nombre", model.proveedor_id);
                return View(model);
            }
        }

        // GET: /OrdenCompra/DetallesOrdenCompra/5
        public ActionResult DetallesOrdenCompra(int id)
        {
            var orden = db.OrdenesCompra
                          .Include("Proveedores")
                          .FirstOrDefault(o => o.id_orden == id);
            if (orden == null) return HttpNotFound();

            var detalles = db.DetalleOrden
                .Where(d => d.id_orden == id)
                .Join(db.Productos,
                      d => d.id_producto,
                      p => p.id,
                      (d, p) => new
                      {
                          Nombre = p.nombre,
                          Cantidad = d.qty,
                          PrecioUnidad = d.precio_unidad,
                          Subtotal = d.subtotal
                      })
                .ToList()
                .Select(x =>
                {
                    dynamic dyn = new ExpandoObject();
                    dyn.Nombre = x.Nombre;
                    dyn.Cantidad = x.Cantidad;
                    dyn.PrecioUnidad = x.PrecioUnidad;
                    dyn.Subtotal = x.Subtotal;
                    return dyn;
                }).ToList();

            ViewBag.Detalles = detalles;
            ViewBag.Orden = orden;
            return View();
        }

        // GET: /OrdenCompra/EditarOrdenCompra/5
        public ActionResult EditarOrdenCompra(int id)
        {
            var orden = db.OrdenesCompra
                          .Include(o => o.Proveedores)
                          .FirstOrDefault(o => o.id_orden == id);

            if (orden == null) return HttpNotFound();

            // Aquí se marca como seleccionado el proveedor actual de la orden
            ViewBag.proveedor_id = new SelectList(
                db.Proveedores.ToList(),
                "id",
                "nombre",
                orden.proveedor_id
            );

            var detalles = db.DetalleOrden
                .Where(d => d.id_orden == id)
                .Join(db.Productos,
                      d => d.id_producto,
                      p => p.id,
                      (d, p) => new
                      {
                          NombreProducto = p.nombre,
                          Cantidad = d.qty,
                          PrecioUnidad = d.precio_unidad,
                          Subtotal = d.subtotal
                      })
                .ToList()
                .Select(x =>
                {
                    dynamic dyn = new ExpandoObject();
                    dyn.NombreProducto = x.NombreProducto;
                    dyn.Cantidad = x.Cantidad;
                    dyn.PrecioUnidad = x.PrecioUnidad;
                    dyn.Subtotal = x.Subtotal;
                    return dyn;
                }).ToList();

            ViewBag.Detalles = detalles;

            return View(orden);
        }

        // POST: /OrdenCompra/EditarOrdenCompra/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarOrdenCompra(OrdenesCompra model)
        {
            if (!ModelState.IsValid)
            {
                // Si hay errores, recargamos combos y detalles,
                // manteniendo el proveedor seleccionado
                ViewBag.proveedor_id = new SelectList(
                    db.Proveedores.ToList(),
                    "id",
                    "nombre",
                    model.proveedor_id
                );

                var detallesError = db.DetalleOrden
                    .Where(d => d.id_orden == model.id_orden)
                    .Join(db.Productos,
                          d => d.id_producto,
                          p => p.id,
                          (d, p) => new
                          {
                              NombreProducto = p.nombre,
                              Cantidad = d.qty,
                              PrecioUnidad = d.precio_unidad,
                              Subtotal = d.subtotal
                          })
                    .ToList()
                    .Select(x =>
                    {
                        dynamic dyn = new ExpandoObject();
                        dyn.NombreProducto = x.NombreProducto;
                        dyn.Cantidad = x.Cantidad;
                        dyn.PrecioUnidad = x.PrecioUnidad;
                        dyn.Subtotal = x.Subtotal;
                        return dyn;
                    }).ToList();

                ViewBag.Detalles = detallesError;

                return View(model);
            }

            var orden = db.OrdenesCompra.Find(model.id_orden);
            if (orden == null) return HttpNotFound();

            // Actualizamos campos que vienen del formulario
            orden.proveedor_id = model.proveedor_id;
            orden.fecha_recepcion = model.fecha_recepcion;
            orden.status = model.status;

            db.Entry(orden).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Success"] = "Orden actualizada.";
            return RedirectToAction("ListarOrdenes");
        }

        // GET: /OrdenCompra/EliminarOrdenCompra/5
        public ActionResult EliminarOrdenCompra(int id)
        {
            var orden = db.OrdenesCompra
                          .Include(o => o.DetalleOrden)
                          .FirstOrDefault(o => o.id_orden == id);
            if (orden == null) return HttpNotFound();

            return View(orden);
        }

        // POST: /OrdenCompra/EliminarOrdenCompra/5
        [HttpPost, ActionName("EliminarOrdenCompra")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarOrdenConfirmed(int id)
        {
            var orden = db.OrdenesCompra
                          .Include(o => o.DetalleOrden)
                          .FirstOrDefault(o => o.id_orden == id);
            if (orden == null) return HttpNotFound();

            foreach (var detalle in orden.DetalleOrden.ToList())
            {
                db.DetalleOrden.Remove(detalle);
            }

            db.OrdenesCompra.Remove(orden);
            db.SaveChanges();

            TempData["Success"] = "Orden eliminada correctamente.";
            return RedirectToAction("ListarOrdenes");
        }

        // PDF de Orden
        public ActionResult DescargarOrdenPDF(int id)
        {
            var orden = db.OrdenesCompra
                          .Include("Proveedores")
                          .FirstOrDefault(o => o.id_orden == id);
            if (orden == null) return HttpNotFound();

            var detalles = db.DetalleOrden.Where(d => d.id_orden == id).ToList();

            using (var ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var subFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);

                doc.Add(new Paragraph($"Detalles de la Orden #{orden.id_orden}", titleFont));
                doc.Add(Chunk.NEWLINE);
                doc.Add(new Paragraph($"Proveedor: {orden.Proveedores?.nombre}", bodyFont));
                doc.Add(new Paragraph($"Fecha de creación: {orden.fecha_creacion}", bodyFont));
                doc.Add(new Paragraph($"Estado: {orden.status}", bodyFont));
                doc.Add(new Paragraph($"Total: ₡{orden.total:N2}", bodyFont));
                doc.Add(Chunk.NEWLINE);

                PdfPTable table = new PdfPTable(4) { WidthPercentage = 100 };
                table.AddCell(new Phrase("Producto", subFont));
                table.AddCell(new Phrase("Cantidad", subFont));
                table.AddCell(new Phrase("Precio Unitario", subFont));
                table.AddCell(new Phrase("Subtotal", subFont));

                foreach (var d in detalles)
                {
                    var producto = db.Productos.FirstOrDefault(p => p.id == d.id_producto);
                    table.AddCell(new Phrase(producto?.nombre ?? "Desconocido", bodyFont));
                    table.AddCell(new Phrase(d.qty.ToString(), bodyFont));
                    table.AddCell(new Phrase($"₡{d.precio_unidad:N2}", bodyFont));
                    table.AddCell(new Phrase($"₡{d.subtotal:N2}", bodyFont));
                }

                doc.Add(table);
                doc.Close();

                byte[] bytes = ms.ToArray();
                return File(bytes, "application/pdf", $"Orden_{orden.id_orden}.pdf");
            }
        }

        [HttpGet]
        public JsonResult GetProductosPorProveedor(int proveedorId)
        {
            var productos = db.Productos
                .Where(p => p.id_proveedor == proveedorId)
                .Select(p => new
                {
                    id = p.id,
                    nombre = p.nombre,
                    precio = p.precio
                })
                .ToList();

            return Json(productos, JsonRequestBehavior.AllowGet);
        }
    }
}
