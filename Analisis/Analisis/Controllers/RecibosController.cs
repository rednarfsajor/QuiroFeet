using System;
using System.Web.Mvc;

namespace Analisis.Controllers
{
    public class RecibosController : Controller
    {
        // GET: Recibos

        public ActionResult CancelReceipt()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        public ActionResult ConfirmCancelReceipt()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        public ActionResult ConfirmReceiptUpdate()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        public ActionResult EditReceipt()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        public ActionResult ErrorPermissionReceipt()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        public ActionResult ListAnularReceipt()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        public ActionResult ListReceipts()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        public ActionResult Receipts()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }
    }
}
