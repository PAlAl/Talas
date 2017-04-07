﻿using System.Collections.Generic;
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
                if (user!=null && user.IsClamp)
                    return RedirectToAction("IndexClamp", new { id = id });
                ViewBag.Greeting = user.Greeting;
                ViewBag.UserName = user.Login;
                ViewBag.Location = string.IsNullOrEmpty(user.Location)?user.Login:user.Location;
                ViewBag.IsGeneral = user.Login == "General" ?"1" : "0";
            }
            return View();
        }

        public ActionResult IndexClamp(Int32 id)
        {
            User user = null;
            using (AppContext db = new AppContext())
            {
                user = db.Users.FirstOrDefault(u => u.Id == id);
                ViewBag.Greeting = user.Greeting;
                ViewBag.UserName = user.Login;
                ViewBag.Location = string.IsNullOrEmpty(user.Location) ? user.Login : user.Location;
                ViewBag.IsGeneral = user.Login == "General" ? "1" : "0";
            }
            return View();
        }

        public ActionResult Ind()
        {
            return View();
        }

        [Authorize]
        public ActionResult Engine()
        {
            User user = null;
            Int32 id = Int32.Parse(HttpContext.Request.Cookies["Talas"].Value);
            using (AppContext db = new AppContext())
            {
                user = db.Users.FirstOrDefault(u => u.Id == id);
                if (user.Login == "General")
                    user.Engines = db.Engines.Where(x=>!x.IsDelete).OrderBy(x=>x.Order).ToList();
                else
                    user.Engines = db.Engines.Where(e => e.UserId == id && !e.IsDelete).OrderBy(x => x.Order).ToList();
            }
            if (user.IsClamp)
                return PartialView("~/views/Engine/EngineClamp.cshtml", user.Engines);
            else
                return PartialView("~/views/Engine/Engine.cshtml", user.Engines);
        }

        public JsonResult JsonGetInfo(string[] engines)
        {           
            List<LastEngineState> jsondata=new List<LastEngineState>();
            if (engines !=null && engines.Length != 0)
            {
                using (AppContext db = new AppContext())
                {
                    Int32 id;
                    foreach (string engine in engines)
                    {
                        id = Int32.Parse(engine);
                        jsondata.Add(db.LastEngineStates.FirstOrDefault(es => es.EngineId == id));
                    }
                }
            }
            return Json(jsondata);
        }
        public JsonResult JsonGetNumberNewMessages()
        {
            Int32 result = 0;
            List<Int32> listEngineId = new List<int>();
            Int32 idUser = Int32.Parse(HttpContext.Request.Cookies["Talas"].Value);          
            using (AppContext db = new AppContext())
            {
                User user = db.Users.FirstOrDefault(u => u.Id == idUser);
                if (user.Login == "General")
                    listEngineId = db.Engines.Where(e=>!e.IsDelete).Select(e => e.Id).ToList();
                else
                    listEngineId = db.Engines.Where(e => e.UserId == idUser && !e.IsDelete).Select(e => e.Id).ToList();
                /*IQueryable<Event> events = from e in db.Events
                                          .Include("EngineState")
                                          .Include("EngineState.Engine")
                                          .Where(e =>e.IsNew && listEngineId.Contains(e.EngineState.EngineId))
                                          .OrderByDescending(e => e.Date)
                     select e;*/
                result = db.Events.Where(e => e.IsNew && listEngineId.Contains(e.EngineId)).Count();
                //result = events.Count();
            }          
            return Json(result, JsonRequestBehavior.AllowGet);
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