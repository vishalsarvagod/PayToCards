

using PayToCardsSystem.AppCode.CommonCode;
using PayToCardsSystem.DAL;
using PayToCardsSystem.Models;
using PayToCardsSystem.Service;
using PayToCardsSystem.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PayToCardsSystem.Controllers
{
    public class LoginController : Controller
    {
        UserService userSerivce = new UserService();
        TransactionService transService = new TransactionService();
        // GET: Login
        public ActionResult Index()
        {
            // string ip = Request.UserHostAddress;
            //ip = transService.generatePassword(10);
            // ViewBag.Message = ip;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(LoginViewNewModel objUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string ip = Request.UserHostAddress;
                    string MD5Pass = Utility.MD5Hash(objUser.Password);
                    user_personal_details userPersonalDetails = userSerivce.checkLoginDetails(objUser.UserName, MD5Pass);
                    {
                        if (userPersonalDetails != null)
                        {
                            if (userSerivce.IsAllowedIPAddress(userPersonalDetails, ip))
                            {
                                Session["UserPersonalDetails"] = userPersonalDetails;
                                if (!userPersonalDetails.IsPasswordChanged)
                                {                                   
                                    return RedirectToAction("ChangePasswordFirstTime", "Login");
                                }
                                else
                                {                                    
                                    return RedirectToAction("Dashboard", "Dashboard");
                                }
                            }
                            else
                            {
                                    ViewBag.Message = "Sorry you can't access this system. Please contact administrator.";
                                    return View();
                                }
                            }
                        }
                    }
                    ViewBag.Message = "Invalid Username or Password";
                    return View();
                }
            catch (Exception ex)
            {
                ViewBag.Message = "Error : " + ex.Message;
                return View();
            }

        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(LoginViewNewModel objUser)
        {
            if (ModelState.IsValid)

            {

            }
            ViewBag.Message = "Invalid Username or Password";
            return View(); //return the same view with message "

        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {

            Session.RemoveAll();
            return RedirectToAction("Index", "Login");

        }
        public ActionResult ChangePassword()
        {
            if (Session["UserPersonalDetails"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]

        public string CheckOldPassword(string oldPassword)
        {
            if (Session["UserPersonalDetails"] != null)
            {
                string MD5OldPass = Utility.MD5Hash(oldPassword);
                var userDetails = (user_personal_details)Session["UserPersonalDetails"];
                vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
                var checkPassword = dbContext.user_personal_details.Any(i => i.Password == MD5OldPass && i.C_id == userDetails.C_id);
                if (checkPassword)
                {
                    return "Success";
                }
            }
            return "Fail";
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel changePasswordModel)
        {
            if (Session["UserPersonalDetails"] != null)
            {
                var userDetails = (user_personal_details)Session["UserPersonalDetails"];
                vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
                user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(i => i.C_id == userDetails.C_id);
                {
                    userPersonal.Password = Utility.MD5Hash(changePasswordModel.NewPassword);
                }
                dbContext.SaveChanges();
            }
            ModelState.Clear();
            return View();
        }
        public ActionResult ChangePasswordFirstTime()
        {
            if (Session["UserPersonalDetails"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePasswordFirstTime(ChangePasswordModel changePasswordModel)
        {              
            if (Session["UserPersonalDetails"] != null)
            {
                var userDetails = (user_personal_details)Session["UserPersonalDetails"];
                vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
                user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(i => i.C_id == userDetails.C_id);
                {
                    userPersonal.Password = Utility.MD5Hash(changePasswordModel.NewPassword);
                    userPersonal.IsPasswordChanged = true;
                }
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard", "Dashboard");
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }
    }
}