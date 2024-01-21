using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPFYEND1.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult register()
        {
            return View();
        }

        public ActionResult forgotpass()
        {
            return View();
        }

    }
}