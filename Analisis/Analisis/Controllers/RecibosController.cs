using System;
using System.Linq;
using System.Web.Mvc;
using Analisis.BD;
using System.Data.Entity;



namespace Analisis.Controllers
{
    public class RecibosController : Controller
    {
        private QuiroFeetEntities6 db = new QuiroFeetEntities6();

        // GET: Recibos/ListReceipts

public ActionResult ListReceipts()
    {
        var recibos = db.Ventas
            .Include(r => r.Clientes) 
            .OrderByDescending(r => r.fecha)
            .ToList();

        return View(recibos);
    }



    // GET: Recibos/Receipts/
    public ActionResult Receipts()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        public ActionResult Details(int id)
        {
            var venta = db.Ventas
                .Include(v => v.Clientes)
                .Include(v => v.DetalleVenta.Select(dv => dv.Productos)) 
                .FirstOrDefault(v => v.id == id);

            if (venta == null)
                return HttpNotFound();

            return View(venta);
        }




        public ActionResult Print(int id)
        {
            var venta = db.Ventas
                .Include(v => v.Clientes)
                .Include(v => v.DetalleVenta.Select(dv => dv.Productos)) 
                .FirstOrDefault(v => v.id == id);

            if (venta == null)
                return HttpNotFound();

            return View("Print", venta);
        }



        // GET: Recibos/EditReceipt/{id}
        public ActionResult EditReceipt(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var recibo = db.Ventas.Find(id);
            if (recibo == null)
                return HttpNotFound();

            ViewBag.Clientes = new SelectList(db.Clientes, "id", "nombre", recibo.id_cliente);
            return View(recibo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditReceipt(Ventas recibo)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                db.Entry(recibo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ListReceipts");
            }

            ViewBag.Clientes = new SelectList(db.Clientes, "id", "nombre", recibo.id_cliente);
            return View(recibo);
        }

        // GET: Recibos/CancelReceipt/{id}
        public ActionResult CancelReceipt(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var recibo = db.Ventas.Find(id);
            if (recibo == null)
                return HttpNotFound();

            return View(recibo); // confirmación
        }

        [HttpPost, ActionName("CancelReceipt")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmCancelReceipt(int id)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var recibo = db.Ventas.Find(id);
            if (recibo == null)
                return HttpNotFound();

            db.Ventas.Remove(recibo);
            db.SaveChanges();

            return RedirectToAction("ListReceipts");
        }

        public ActionResult ErrorPermissionReceipt()
        {
            return View();
        }

        // POST: Recibos/Anular/5
        [HttpPost]
        public ActionResult Anular(int id)
        {
            var venta = db.Ventas.Find(id);
            if (venta == null)
            {
                return HttpNotFound();
            }

            // Marca la venta como anulada
            venta.Estado = "Anulado"; 
            db.SaveChanges();

            return Json(new { success = true });
        }

    }
}
