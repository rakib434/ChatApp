using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignalRGChat.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        //================ Message =================
        public ActionResult Message()
        {
            return View();
        }

        public ActionResult MessageAndGroupMessage()
        {
            return View();
        }

        public ActionResult GroupMessage()
        {
            return View();
        }
        //=============== Message End ================
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}