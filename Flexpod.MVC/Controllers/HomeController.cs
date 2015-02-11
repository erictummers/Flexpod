using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Flexpod.MVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Awesome demo app for powershell modules";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "You've been granted access.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Ignore the details here.";

            return View();
        }
    }
}
