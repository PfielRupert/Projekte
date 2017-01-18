using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeutscheBankKreditrechner.logic;

namespace DeutscheBankKreditrechner.web.Controllers
{
    public class HomeController : Controller
    {
        public static bool alleDatenAngeben = false;
        public ActionResult Index()
        {
            KonsumKReditVerwaltung.FirstMailSenden();


            alleDatenAngeben = false;
            return View();
        }
    }
}