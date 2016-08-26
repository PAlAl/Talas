using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Talas.Models;
using System.Web.Configuration;
using Objects;

namespace Talas.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login()
        {  
        return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                AuthenticateState authenticateResult = Authenticator.Authenticate(model.Login,model.Password);
                switch (authenticateResult)
                {
                    case AuthenticateState.PasswordNotCorrect:
                        ModelState.AddModelError("", "Password is not correct");
                        break;

                    case AuthenticateState.UserNotFound:
                        ModelState.AddModelError("", "Login and password are not found");
                        break;

                    case AuthenticateState.Succes:
                        FormsAuthentication.SetAuthCookie(model.Login, model.RememberMe);
                        HttpCookie cookie = new HttpCookie("Talas");
                        cookie.Value = Authenticator.Id;
                        if (model.RememberMe)
                        {
                            Int32 timeOut = GetTimeOut();
                            cookie.Expires = DateTime.Now.AddMinutes(timeOut);
                        }
                        Response.Cookies.Add(cookie);
                        return RedirectToAction("Index", "Home");                                      
                } 
            }
            return View(model);
        }

        private int GetTimeOut()
        {
            Object section = WebConfigurationManager.GetSection("system.web/authentication");
            Double time  = ((System.Web.Configuration.AuthenticationSection)section).Forms.Timeout.TotalMinutes;
            return Int32.Parse(time.ToString());
        }

      /*  public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                using (UserContext db = new UserContext())
                {
                    user = db.Users.FirstOrDefault(u => u.Email == model.Name);
                }
                if (user == null)
                {
                    // создаем нового пользователя
                    using (UserContext db = new UserContext())
                    {
                        db.Users.Add(new User { Email = model.Name, Password = model.Password, Age = model.Age });
                        db.SaveChanges();

                        user = db.Users.Where(u => u.Email == model.Name && u.Password == model.Password).FirstOrDefault();
                    }
                    // если пользователь удачно добавлен в бд
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.Name, true);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь с таким логином уже существует");
                }
            }

            return View(model);
        }*/
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();

            HttpCookie cookie = new HttpCookie("Talas");
            cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie);

            return RedirectToAction("Index", "Home");
        }
    }
}