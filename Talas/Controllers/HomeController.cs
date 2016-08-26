using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Talas.Models;
using Objects;
using System;

namespace Talas.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            if (HttpContext.Request.Cookies["Talas"] == null)
            {
                return RedirectToAction("Login", "Account");
            }           
            User user = null;
            Int32 id = Int32.Parse(HttpContext.Request.Cookies["Talas"].Value);           
            using (AppContext db = new AppContext())
            {
                user = db.Users.FirstOrDefault(u => u.Id == id);
                user.Engines = db.Engines.Where(e => e.UserId == id).ToList();
                ViewBag.Greeting = user.Greeting;
            }
            return View(user.Engines);
        }

        public JsonResult JsonGetInfo(string[] engines)
        {           
            List<EngineState> jsondata=new List<EngineState>();
            using (AppContext db = new AppContext())
            {
                Int32 id;
                foreach (string engine in engines)
                {
                    id = Int32.Parse(engine);
                    jsondata.Add(db.EngineStates.Where(es => es.EngineId == id).OrderByDescending(es => es.Date).FirstOrDefault());
                }
            }
            return Json(jsondata);
        }

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