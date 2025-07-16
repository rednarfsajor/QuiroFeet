using System;
using System.Linq;
using System.Web.Mvc;
using Analisis.BD;
using System.Data.Entity;



namespace Analisis.Controllers
{
    public class RecibosController : Controller
    {
        private QuiroFeetEntities5 db = new QuiroFeetEntities5();

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
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var venta = db.Ventas
                .Include(v => v.Clientes) 
                .FirstOrDefault(v => v.id == id);

            if (venta == null)
                return HttpNotFound();

            return View(venta);
        }

        public ActionResult Print(int id)
        {
            var venta = db.Ventas.Include("Clientes").FirstOrDefault(v => v.id == id);
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

       /* public ActionResult ListAnularReceipt()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            var anulables = db.Ventas.Where(v => v.fecha >= DateTime.Today.AddDays(-30)).ToList(); // ejemplo
            return View(anulables);
        }*/
    }
}
