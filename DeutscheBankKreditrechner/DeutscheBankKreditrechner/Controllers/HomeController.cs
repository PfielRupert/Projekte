using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeutscheBankKreditrechner.web.Controllers
{
    public class HomeController : Controller
    {
        public static bool alleDatenAngeben = false;
        public ActionResult Index()
        {
            alleDatenAngeben = false;
            return View();
        }
    }
}