using ReportSchedular.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace ReportSchedular.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Authorize(ReportSchedular.Models.User userModel)
        {

            using (LoginDatabaseEntities db = new LoginDatabaseEntities())
            {
                // var userDetails = db.Users.Where(x => x.UserName == userModel.UserName && x.Password == userModel.Password).FirstOrDefault();
                var userDetails = db.Users.Where(x => x.UserName == userModel.UserName && x.Password == userModel.Password).FirstOrDefault();

                if (userDetails == null)
                {

                    userModel.LoginErrorMessage = "Wrong UserName or Password ...";
                    return View("Index", userModel);
                }
                else
                {
                    Session["userID"] = userDetails.UserID;
                    Session["userName"] = userDetails.UserName;
                    return RedirectToAction("Index", "Home");

                }
            }

        }
        public ActionResult LogOut()
        {
            int userId = (int)Session["userID"];
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
    }
}
