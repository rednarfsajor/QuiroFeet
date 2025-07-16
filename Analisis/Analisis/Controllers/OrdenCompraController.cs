/*using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Analisis.BD;
using System.Dynamic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Analisis.Controllers
{
    public class OrdenCompraController : Controller
    {
        private QuiroFeetEntities2 db = new QuiroFeetEntities2();

        // GET: /OrdenCompra/ListarOrdenes
        public ActionResult ListarOrdenes()
        {
            var ordenes = db.OrdenesCompra.Include(o => o.Proveedores).ToList();
            return View(ordenes);
        }

        // GET: /OrdenCompra/DetallesOrdenCompra/5
        public ActionResult DetallesOrdenCompra(int id)
        {
            var orden = db.OrdenesCompra
            .Include("Proveedores")
            .FirstOrDefault(o => o.id_orden == id);

            if (orden == null)
            {
                return HttpNotFound();
            }

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
    })
    .ToList();

            ViewBag.Detalles = detalles;
            ViewBag.Orden = orden;

            return View();
        }

        // GET: /OrdenCompra/CrearOrdenCompra
        public ActionResult CrearOrdenCompra()
        {
            ViewBag.Proveedores = new SelectList(db.Proveedores, "id", "nombre");
            ViewBag.Productos = db.Productos.ToList(); // para mostrar productos en el formulario
            return View();
        }

        // POST: /OrdenCompra/CrearOrdenCompra
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearOrdenCompra(int proveedor_id, string[] id_producto, int[] qty, decimal[] precio)
        {
            /*if (id_producto.Length != qty.Length || qty.Length != precio.Length)
            {
                ModelState.AddModelError("", "Los detalles de productos están desalineados.");
            }*/

/*if (!ModelState.IsValid)
{
    ViewBag.Proveedores = new SelectList(db.Proveedores, "id", "nombre", proveedor_id);
    ViewBag.Productos = db.Productos.ToList();
    return View();
}*/
/*System.Diagnostics.Debug.WriteLine("Proveedor: " + proveedor_id);
            System.Diagnostics.Debug.WriteLine("Productos: " + string.Join(",", id_producto ?? new string[0]));
            System.Diagnostics.Debug.WriteLine("Cantidades: " + string.Join(",", qty ?? new int[0]));
            System.Diagnostics.Debug.WriteLine("Precios: " + string.Join(",", precio ?? new decimal[0]));

            var orden = new OrdenesCompra
            {
                proveedor_id = proveedor_id,
                fecha_creacion = DateTime.Now,
                status = "Pendiente",
                total = 0 // se actualizará luego
            };

            db.OrdenesCompra.Add(orden);
            db.SaveChanges(); // Guardamos para obtener el ID

            decimal totalOrden = 0;
            
            for (int i = 0; i < id_producto.Length; i++)
            {
                var detalle = new DetalleOrden
                {
                    id_orden = orden.id_orden,
                    id_producto = int.Parse(id_producto[i]),
                    qty = qty[i],
                    precio_unidad = precio[i],
                    subtotal = qty[i] * precio[i]
                };

                totalOrden += detalle.subtotal.Value;
                db.DetalleOrden.Add(detalle);
            }

            orden.total = totalOrden;
            db.SaveChanges();

            TempData["Success"] = "Orden registrada correctamente.";
            return RedirectToAction("ListarOrdenes");
        }

        // GET: /OrdenCompra/EditarOrdenCompra/5
        public ActionResult EditarOrdenCompra(int id)
        {
            var orden = db.OrdenesCompra.Find(id);
            if (orden == null)
                return HttpNotFound();

            // Pasar lista de proveedores
            ViewBag.proveedor_id = new SelectList(
                db.Proveedores.ToList(),
                "id", "nombre",
                orden.proveedor_id);

            // Cargar detalles como dinámicos con nombres legibles
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
                    dynamic dyn = new System.Dynamic.ExpandoObject();
                    dyn.NombreProducto = x.NombreProducto;
                    dyn.Cantidad = x.Cantidad;
                    dyn.PrecioUnidad = x.PrecioUnidad;
                    dyn.Subtotal = x.Subtotal;
                    return dyn;
                })
                .ToList();

            ViewBag.Detalles = detalles;

            return View(orden);
        }



        // POST: /OrdenCompra/EditarOrdenCompra/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarOrdenCompra(int id, string status)
        {
            var orden = db.OrdenesCompra.Find(id);
            if (orden == null)
                return HttpNotFound();

            orden.status = status;
            db.Entry(orden).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Success"] = "Orden actualizada.";
            return RedirectToAction("ListarOrdenes");
        }

        // GET: /OrdenCompra/EliminarOrdenCompra/5
        public ActionResult EliminarOrdenCompra(int id)
        {
            var orden = db.OrdenesCompra.Include(o => o.DetalleOrden).FirstOrDefault(o => o.id_orden == id);
            if (orden == null)
                return HttpNotFound();

            return View(orden);
        }

        // POST: /OrdenCompra/EliminarOrdenCompra/5
        [HttpPost, ActionName("EliminarOrdenCompra")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarOrdenConfirmed(int id)
        {
            var orden = db.OrdenesCompra.Include(o => o.DetalleOrden).FirstOrDefault(o => o.id_orden == id);

            if (orden == null)
                return HttpNotFound();

            // Eliminar detalles primero
            foreach (var detalle in orden.DetalleOrden.ToList())
            {
                db.DetalleOrden.Remove(detalle);
            }

            db.OrdenesCompra.Remove(orden);
            db.SaveChanges();

            TempData["Success"] = "Orden eliminada correctamente.";
            return RedirectToAction("ListarOrdenes");
        }

        public ActionResult DescargarOrdenPDF(int id)
        {
            var orden = db.OrdenesCompra
                .Include("Proveedores")
                .FirstOrDefault(o => o.id_orden == id);

            var detalles = db.DetalleOrden
                .Where(d => d.id_orden == id)
                .ToList();

            if (orden == null)
                return HttpNotFound();

            using (var ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var subFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);

                // Título
                doc.Add(new Paragraph($"Detalles de la Orden #{orden.id_orden}", titleFont));
                doc.Add(Chunk.NEWLINE);

                // Info de la orden
                doc.Add(new Paragraph($"Proveedor: {orden.Proveedores.nombre}", bodyFont));
                doc.Add(new Paragraph($"Fecha de creación: {orden.fecha_creacion}", bodyFont));
                doc.Add(new Paragraph($"Estado: {orden.status}", bodyFont));
                doc.Add(new Paragraph($"Total: ₡{orden.total:N2}", bodyFont));
                doc.Add(Chunk.NEWLINE);

                // Tabla de productos
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
                .Select(p => new {
                    id = p.id,
                    nombre = p.nombre,
                    precio = p.precio
                })
                .ToList();

            return Json(productos, JsonRequestBehavior.AllowGet);
        }

    }
}
*/