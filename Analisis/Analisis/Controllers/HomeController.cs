using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Analisis.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        ///
        public ActionResult CancelReceipt()
        {
            return View("~/Views/Recibos/CancelReceipt.cshtml");
        }

        public ActionResult ConfirmCancelReceipt()
        {
            return View("~/Views/Recibos/ConfirmCancelReceipt.cshtml");
        }

        public ActionResult ConfirmReceiptUpdate()
        {
            return View("~/Views/Recibos/ConfirmReceiptUpdate.cshtml");
        }

        public ActionResult EditReceipt()
        {
            return View("~/Views/Recibos/EditReceipt.cshtml");
        }

        public ActionResult ErrorPermissionReceipt()
        {
            return View("~/Views/Recibos/ErrorPermissionReceipt.cshtml");
        }

        public ActionResult ListAnularReceipt()
        {
            return View("~/Views/Recibos/ListAnularReceipt.cshtml");
        }

        public ActionResult ListReceipts()
        {
            return View("~/Views/Recibos/ListReceipts.cshtml");
        }

        public ActionResult Receipts()
        {
            return View("~/Views/Recibos/Receipts.cshtml");
        }


        /// ///////////////////////////////////////////////////Acción para la vista de Crear Usuario
        public ActionResult ConfirmDeleteUser()
        {
            return View("~/Views/Usuarios/ConfirmDeleteUser.cshtml");
        }

        public ActionResult ConfirmEditUser()
        {
            return View("~/Views/Usuarios/ConfirmEditUser.cshtml");
        }

        public ActionResult CreateUser()
        {
            return View("~/Views/Usuarios/CreateUser.cshtml");
        }

        public ActionResult DeleteUser()
        {
            return View("~/Views/Usuarios/DeleteUser.cshtml");
        }

        public ActionResult EditUser()
        {
            return View("~/Views/Usuarios/EditUser.cshtml");
        }

        public ActionResult ListUser()
        {
            return View("~/Views/Usuarios/ListUser.cshtml");
        }

        public ActionResult Users()
        {
            return View("~/Views/Usuarios/Users.cshtml");
        }

        /////////////////////////////////////////////////
        public ActionResult ConfirmSale()
        {
            return View("~/Views/Ventas/ConfirmSale.cshtml");
        }

        public ActionResult ErrorSale()
        {
            return View("~/Views/Ventas/ErrorSale.cshtml");
        }

        public ActionResult ListSales()
        {
            return View("~/Views/Ventas/ListSales.cshtml");
        }

        public ActionResult RegisterSale()
        {
            return View("~/Views/Ventas/RegisterSale.cshtml");
        }

        public ActionResult Sales()
        {
            return View("~/Views/Ventas/Sales.cshtml");
        }


    }

}
