using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BarrageDemo2.Website.Controllers
{
    public class BarrageController : Controller
    {
        // GET: Barrage
        public ActionResult Index()
        {
            return View();
        }
    }
}