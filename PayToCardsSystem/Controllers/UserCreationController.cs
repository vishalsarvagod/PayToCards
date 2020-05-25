using MySql.Data.MySqlClient;
using PayToCardsSystem.AppCode.CommonCode;
using PayToCardsSystem.Models;
using PayToCardsSystem.Service;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.EntityClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.HtmlControls;

namespace PayToCardsSystem.Controllers
{
    public class UserCreationController : Controller
    {
        CurrencyService currencyUser = new CurrencyService();
        UserService userService = new UserService();
        static int pkId = 0;
        static int rowFeeId = 0;
        //static DataTable dtFee;
        public object Body { get; private set; }

      
        private List<SelectListItem> GetFeeCalculation()
        {
            SelectListItem si = new SelectListItem();
            List<SelectListItem> FeeCalc = new List<SelectListItem>();
            FeeCalc.Add(new SelectListItem { Text = "AND", Value = "AND" });
            FeeCalc.Add(new SelectListItem { Text = "OR", Value = "OR" });
            return FeeCalc;
        }
        public ActionResult UserCreation(int? id)
        {
            try
            {
                if (Session["UserPersonalDetails"] != null)
                {
                    //InitializeFeeDt();
                    var userDetails = (user_personal_details)Session["UserPersonalDetails"];

                    vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
                    UserCreationModel userCreation = new UserCreationModel();
                    userCreation.RoleNameList = GetAllRoles(userDetails);
                    userCreation.CountryNameList = GetAllCountry();
                    userCreation.CurrencyList = currencyUser.GetAllCurrency(userDetails);
                    userCreation.SecurityList = GetAllSecurityQue();
                    userCreation.ListFeeCalculation = GetFeeCalculation();
                    userCreation.SelectedCurrency.ListFeeCal = GetFeeCalculation();
                    userCreation.SelectedCurrency.Cur = currencyUser.GetAllCurrency(userDetails);
                    userCreation.IsActive = true;
                    if (id != -1)
                    {
                        user_personal_details userPersonalDetails = dbContext.user_personal_details.FirstOrDefault(i => i.C_id == id);
                        userCreation.PkId = userPersonalDetails.C_id;
                        userCreation.FirstName = userPersonalDetails.FirstName;
                        userCreation.LastName = userPersonalDetails.LastName;
                        userCreation.UserName = userPersonalDetails.UserName;
                        userCreation.Password = userPersonalDetails.Password;
                        userCreation.RetypePassword = userPersonalDetails.Password;
                        userCreation.CompanyName = userPersonalDetails.Company;
                        userCreation.Address1 = userPersonalDetails.AddressLine1;
                        userCreation.Address2 = userPersonalDetails.AddressLine2;
                        userCreation.City = userPersonalDetails.City;
                        userCreation.CountryValue = userPersonalDetails.Country;
                        userCreation.PostalCode = userPersonalDetails.Zip;
                        userCreation.EmailId = userPersonalDetails.Email;
                        userCreation.Telephone = userPersonalDetails.ContactNo1;                        
                        //userCreation.CurrencyValue = userPersonalDetails.DefalutCurrency;
                        userCreation.CurrencyValue = "";
                        userCreation.SecurityValue = userPersonalDetails.SecurityQuestNo.ToString();
                        userCreation.Answer = userPersonalDetails.SecurityAnswer;
                        userCreation.RoleValue = userPersonalDetails.Role;
                        userCreation.AllowIpAddress = userPersonalDetails.AllowIPAddress;
                        userCreation.BlockIpAddress = userPersonalDetails.BlockIPAddress;
                        userCreation.IsActive = userPersonalDetails.IsActive;
                        userCreation.ApiPassword = userPersonalDetails.ApiPassword;
                        //var feeDetails = (from fee in dbContext.user_fee_details
                        //                  join currencyName in dbContext.currencies on fee.CurrencyId equals currencyName.C_id  
                        //                  select new { fee.C_id, fee.CurrencyId, currencyName.ShortCode, fee.PercentFee, fee.FlatFee });
                        var feeDetails = (from fee in dbContext.user_fee_details
                                          from currencyName in dbContext.currencies
                                          where fee.CurrencyId == currencyName.C_id && fee.UserId == id
                                          select new { fee.C_id, fee.CurrencyId, currencyName.CurrencyName, fee.PercentFee, fee.FlatFee, fee.IsActive, fee.FeeType });
                        foreach (var user in feeDetails)
                        {
                            AddUserCurrency feeCur = new AddUserCurrency();
                            feeCur.Cur = currencyUser.GetAllCurrencyWithInActive(userDetails);
                            feeCur.ListFeeCal = GetFeeCalculation();
                            feeCur.SrNo = (userCreation.listCurrency.Count + 1);
                            feeCur.Id = user.C_id;
                            feeCur.CurrencyId = user.CurrencyId;
                            feeCur.CurrencyName = user.CurrencyName;
                            feeCur.PercentAmount = user.PercentFee;
                            feeCur.FlatAmount = user.FlatFee;
                            feeCur.IsActive = (bool)user.IsActive;
                            feeCur.SelectItem(user.CurrencyId.ToString());
                            feeCur.FeeValue = user.FeeType;
                            feeCur.SelectItemFee(user.FeeType);
                                                              
                            userCreation.listCurrency.Add(feeCur);
                            //DataRow dr = dtFee.NewRow();
                            //dr[0] = dtFee.Rows.Count;
                            //dr[1] = user.C_id;
                            //dr[2] = user.CurrencyId;
                            //dr[3] = user.CurrencyName;
                            //dr[4] = user.PercentFee;
                            //dr[5] = user.FlatFee;
                            //dtFee.Rows.Add(dr);
                        }
                        userCreation.AgentCommInPer = 0;
                        userCreation.AgentCommInAmount = 0;
                    }
                    pkId = userCreation.PkId;
                    //ViewData["FeeDt"] = dtFee;

                    return View(userCreation);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }

       
        public ActionResult CheckUserName(string UserName)
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            var checkUserName = dbContext.user_personal_details.Any(i => i.UserName == UserName);
                if (checkUserName)
            {
                return Json("already Exists :" + UserName);
            }
            else
                return Json("success");

        }
        [HttpPost]
        [SubmitButtonSelector(Name = "Back")]
        public ActionResult Back(UserCreationModel objUser)
        {
            return RedirectToAction("ManageUser", "UserCreation");
        }

        [HttpPost]
        [SubmitButtonSelector(Name = "Save")]
        [ValidateAntiForgeryToken]
        public ActionResult Save(UserCreationModel objUser)
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            var userDetails = (user_personal_details)Session["UserPersonalDetails"];
            try
            {
                //------------------check username---------------//
                if (pkId < 1)
                {
                    var checkUserName = dbContext.user_personal_details.Any(i => i.UserName == objUser.UserName);
                    if (checkUserName)
                    {
                        FillDropdownAgain(objUser);
                        objUser.SelectedCurrency.Cur = currencyUser.GetAllCurrency(userDetails);
                        ModelState.AddModelError("UserName", "User Name Aready Exists");
                        return View(objUser);
                    }
                }
                //-----------------------------------------------//
                if (objUser.listCurrency.Count == 0)
                {
                    FillDropdownAgain(objUser);
                    objUser.SelectedCurrency.Cur = currencyUser.GetAllCurrency(userDetails);
                    objUser.SelectedCurrency.ListFeeCal = GetFeeCalculation();
                    ModelState.AddModelError("CurrencyCheck", "Atleast one Currency Row Required");
                    return View(objUser);

                }
                user_personal_details userPersonalDetails = dbContext.user_personal_details.FirstOrDefault(i => i.C_id == pkId);
                {
                    if (userPersonalDetails == null)
                        userPersonalDetails = new user_personal_details();

                    userPersonalDetails.FirstName = objUser.FirstName;
                    userPersonalDetails.LastName = objUser.LastName;
                    userPersonalDetails.UserName = objUser.UserName;
                    if (pkId <= 0)
                    {
                        userPersonalDetails.Password = Utility.MD5Hash(objUser.Password);
                        userPersonalDetails.ApiPassword = userService.generateApiPassword(8);
                        objUser.ApiPassword = userPersonalDetails.ApiPassword;
                    }

                    userPersonalDetails.Company = objUser.CompanyName;
                    userPersonalDetails.AddressLine1 = objUser.Address1;
                    userPersonalDetails.AddressLine2 = objUser.Address2;
                    userPersonalDetails.City = objUser.City;
                    userPersonalDetails.Country = objUser.CountryValue;
                    userPersonalDetails.Zip = objUser.PostalCode;
                    userPersonalDetails.Email = objUser.EmailId;
                    userPersonalDetails.ContactNo1 = objUser.Telephone;                    
                    userPersonalDetails.DefalutCurrency = (objUser.SelectedCurrency.CurrencyId).ToString();// currency value is null
                    userPersonalDetails.SecurityQuestNo = Convert.ToInt16(objUser.SecurityValue);
                    userPersonalDetails.SecurityAnswer = objUser.Answer;
                    userPersonalDetails.Role = objUser.RoleValue;
                    userPersonalDetails.AllowIPAddress = objUser.AllowIpAddress;
                    userPersonalDetails.BlockIPAddress = objUser.BlockIpAddress;
                    userPersonalDetails.IsActive = objUser.IsActive;
                    if (userDetails.Role == Constant.ROLE_DISTRIBUTOR)
                    {
                        userPersonalDetails.DistributorID = userDetails.C_id;
                        userPersonalDetails.AdminId = userDetails.AdminId;
                    }
                    else
                    {
                        userPersonalDetails.DistributorID = 0;
                        userPersonalDetails.AdminId = userDetails.C_id;
                    }
                    if (pkId == 0)
                    {
                        userPersonalDetails.C_id = pkId;
                        userPersonalDetails.IsPasswordChanged = false;
                        dbContext.user_personal_details.AddObject(userPersonalDetails);
                                           
                    }
                    dbContext.SaveChanges();                 

                };
                for (int i = 0; i < objUser.listCurrency.Count; i++)
                {
                    AddUserCurrency obj = objUser.listCurrency[i];
                    obj.Cur = currencyUser.GetAllCurrency(userDetails);
                    obj.SelectItem(obj.CurrencyId.ToString());
                    obj.ListFeeCal = GetFeeCalculation();
                    obj.SelectItemFee(obj.FeeValue);
                    //int curId = Convert.ToInt32(dtFee.Rows[i].ItemArray[2].ToString());
                    //user_fee_details userFeeDetails = dbContext.user_fee_details.FirstOrDefault(j => j.UserId == userPersonalDetails.C_id && j.CurrencyId== Convert.ToInt32(dt.Rows[i].ItemArray[1].ToString()));
                    //{
                    //    if (userFeeDetails == null)
                    //dbContext.user_fee_details.Where(f => f.C_id == obj.Id).FirstOrDefault();
                    user_fee_details userFeeDetails = dbContext.user_fee_details.Where(f => f.C_id == obj.Id).FirstOrDefault();
                    {
                        if (userFeeDetails == null)
                            userFeeDetails = new user_fee_details();
                        userFeeDetails.UserId = userPersonalDetails.C_id;
                        userFeeDetails.PercentFee = obj.PercentAmount;
                        userFeeDetails.FlatFee = obj.FlatAmount;
                        userFeeDetails.ComissionAmount = 0;
                        userFeeDetails.CurrencyId = obj.CurrencyId;
                        userFeeDetails.IsActive = obj.IsActive;
                        userFeeDetails.FeeType = obj.FeeValue;
                        if (userFeeDetails.C_id == 0)
                        {
                            dbContext.user_fee_details.AddObject(userFeeDetails);
                        }
                        dbContext.SaveChanges();
                    };

                    user_currency_details userCurrencyDetails = dbContext.user_currency_details.FirstOrDefault(j => j.UserId == userPersonalDetails.C_id && j.CurrencyId == obj.CurrencyId);
                    {
                        if (userCurrencyDetails == null)
                            userCurrencyDetails = new user_currency_details();
                        userCurrencyDetails.UserId = userPersonalDetails.C_id;
                        userCurrencyDetails.CurrencyId = obj.CurrencyId;
                        userCurrencyDetails.Currency = obj.CurrencyName;
                        userCurrencyDetails.IsActive = obj.IsActive;
                        if (userCurrencyDetails.C_id == 0)
                        {
                            dbContext.user_currency_details.AddObject(userCurrencyDetails);
                            userCurrencyDetails.Balance = 0;
                        }
                        dbContext.SaveChanges();
                    };
                    if (!obj.IsActive && pkId>0)
                    {
                        if (objUser.RoleValue.ToString().Equals(Constant.ROLE_DISTRIBUTOR))
                        {
                            string query = "UPDATE user_fee_details set IsActive = 0 where CurrencyId = "+obj.CurrencyId+ " AND UserID in (Select _id from user_personal_details where DistributorId = "+ pkId + ")";
                            var updateResult = dbContext.ExecuteStoreCommand(query);

                            query = "UPDATE user_currency_details set IsActive = 0 where CurrencyId = " + obj.CurrencyId + " AND UserID in (Select _id from user_personal_details where DistributorId = " + pkId + ")";
                            var updateResult1 = dbContext.ExecuteStoreCommand(query);
                        }
                        else if (objUser.RoleValue.ToString().Equals(Constant.ROLE_ADMIN))
                        {
                            string query = "UPDATE user_fee_details set IsActive = 0 where CurrencyId = "+obj.CurrencyId+ " AND UserID in (Select _id from user_personal_details where AdminId = "+ pkId + ")";
                            var updateResult = dbContext.ExecuteStoreCommand(query);

                            query = "UPDATE user_currency_details set IsActive = 0 where CurrencyId = " + obj.CurrencyId + " AND UserID in (Select _id from user_personal_details where AdminId = " + pkId + ")";
                            var updateResult1 = dbContext.ExecuteStoreCommand(query);
                        }
                    }
                }
                if (pkId == 0)
                {
                    SendEmailToUser(objUser);
                }
            }
            catch (Exception e)
            {
                //ViewBag.Message = e.ToString();
                return View(e.ToString());
            }
            finally
            {

            }
            return RedirectToAction("ManageUser", "UserCreation");
        }

        public void FillDropdownAgain(UserCreationModel objUser)
        {
            if (Session["UserPersonalDetails"] != null)
            {
                var userDetails = (user_personal_details)Session["UserPersonalDetails"];
                for (int i = 0; i < objUser.listCurrency.Count; i++)
                {
                    objUser.listCurrency[i].Cur = currencyUser.GetAllCurrency(userDetails);
                    objUser.listCurrency[i].ListFeeCal = GetFeeCalculation();
                }
                objUser.RoleNameList = GetAllRoles(userDetails);
                objUser.CountryNameList = GetAllCountry();
                objUser.CurrencyList = currencyUser.GetAllCurrency(userDetails);
                objUser.SecurityList = GetAllSecurityQue();
                objUser.PkId = pkId;
            }
        }
        [HttpPost]
        [SubmitButtonSelector(Name = "AddRow")]
        public ActionResult AddRow(UserCreationModel objUser)
        {
            if (Session["UserPersonalDetails"] != null)
            {
                var userDetails = (user_personal_details)Session["UserPersonalDetails"];
                FillDropdownAgain(objUser);

                var match = objUser.listCurrency.Where(currencyCheck => currencyCheck.CurrencyId == objUser.SelectedCurrency.CurrencyId).FirstOrDefault();
                if (match != null)
                {
                    objUser.SelectedCurrency.Cur = currencyUser.GetAllCurrency(userDetails);
                    objUser.SelectedCurrency.SelectItem(objUser.SelectedCurrency.CurrencyId.ToString());
                    objUser.SelectedCurrency.ListFeeCal = GetFeeCalculation();
                    objUser.SelectedCurrency.SelectItemFee(objUser.SelectedCurrency.FeeValue.ToString());
                    objUser.SelectedCurrency.IsActive = true;
                    ModelState.AddModelError("CurrencyCheck", "Currency Already Exists");

                    return View(objUser);
                }
                objUser.SelectedCurrency.Cur = currencyUser.GetAllCurrency(userDetails);
                objUser.SelectedCurrency.SelectItem(objUser.SelectedCurrency.CurrencyId.ToString());
                objUser.SelectedCurrency.ListFeeCal = GetFeeCalculation();
                objUser.SelectedCurrency.SelectItemFee(objUser.SelectedCurrency.FeeValue.ToString());
                objUser.SelectedCurrency.IsActive = true;
                objUser.listCurrency.Add(objUser.SelectedCurrency);
                objUser.SelectedCurrency = new AddUserCurrency();
                objUser.SelectedCurrency.Cur = currencyUser.GetAllCurrency(userDetails);
                objUser.SelectedCurrency.ListFeeCal = GetFeeCalculation();
                return View(objUser);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        // GET: ManageUser
        public ActionResult ManageUser(string UserName)
        {
            List<ManageUserModel> userList = GetAllUser(UserName);
            if (userList == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View(userList);
            }

        }
        //To view employee details with generic list
        public List<ManageUserModel> GetAllUser(string searchString)
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            if (Session["UserPersonalDetails"] != null)
            {
                List<ManageUserModel> manageUserList = new List<ManageUserModel>();
                var userDetails = (user_personal_details)Session["UserPersonalDetails"];
                ViewBag.Message = userDetails.FirstName;
                if (searchString != null && searchString.Length > 0)
                {
                    if (userDetails.Role.Equals(Constant.ROLE_SUPER_ADMIN))
                    {
                        var users = from user in dbContext.user_personal_details
                                    where user.C_id > 3
                                    join admin in dbContext.user_personal_details on user.AdminId equals admin.C_id into tempJoin
                                    from tbl in tempJoin.DefaultIfEmpty()
                                    join distributor in dbContext.user_personal_details on user.DistributorID equals distributor.C_id into finalTbl
                                    from val in finalTbl.DefaultIfEmpty()
                                    where (user.FirstName.ToLower().Contains(searchString.ToLower()) || user.LastName.ToLower().Contains(searchString.ToLower()))
                                    select new { user.C_id, user.FirstName, user.LastName, user.UserName, user.Role, user.Company,Admin = tbl == null ? "-" : tbl.FirstName, DistributorName = val == null ? "-" : val.FirstName,user.IsActive };
                        int i = 0;
                        foreach (var user in users)
                        {
                            i++;
                            manageUserList.Add(
                            new ManageUserModel
                            {
                                Id = user.C_id,
                                SrNo = i,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Role = user.Role,
                                UserName = user.UserName,
                                AdminName = user.Admin,
                                DistributorName = user.DistributorName,
                                CompanyName = user.Company,
                                IsActive = user.IsActive
                            }
                            );
                        }
                    }
                    if (userDetails.Role.Equals(Constant.ROLE_ADMIN))
                    {
                        var users = from user in dbContext.user_personal_details
                                    join admin in dbContext.user_personal_details on user.AdminId equals admin.C_id into tempJoin
                                    from tbl in tempJoin.DefaultIfEmpty()
                                    join distributor in dbContext.user_personal_details on user.DistributorID equals distributor.C_id into finalTbl
                                    from val in finalTbl.DefaultIfEmpty()
                                    where user.AdminId == userDetails.C_id && (user.FirstName.ToLower().Contains(searchString.ToLower()) || user.LastName.ToLower().Contains(searchString.ToLower()))
                                    select new { user.C_id, user.FirstName, user.LastName, user.UserName, user.Role, user.Company, Admin = tbl == null ? "-" : tbl.FirstName, DistributorName = val == null ? "-" : val.FirstName,user.IsActive };
                        int i = 0;
                        foreach (var user in users)
                        {
                            i++;
                            manageUserList.Add(
                            new ManageUserModel
                            {
                                Id = user.C_id,
                                SrNo = i,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Role = user.Role,
                                UserName = user.UserName,
                                AdminName = user.Admin,
                                DistributorName = user.DistributorName,
                                CompanyName = user.Company,
                                IsActive = user.IsActive
                            }
                            );
                        }
                    }
                    if (userDetails.Role.Equals(Constant.ROLE_DISTRIBUTOR))
                    {
                        var users = from user in dbContext.user_personal_details
                                    join admin in dbContext.user_personal_details on user.AdminId equals admin.C_id into tempJoin
                                    from tbl in tempJoin.DefaultIfEmpty()
                                    join distributor in dbContext.user_personal_details on user.DistributorID equals distributor.C_id into finalTbl
                                    from val in finalTbl.DefaultIfEmpty()
                                    where user.DistributorID == userDetails.C_id && (user.FirstName.ToLower().Contains(searchString.ToLower()) || user.LastName.ToLower().Contains(searchString.ToLower()))
                                    select new { user.C_id, user.FirstName, user.LastName, user.UserName, user.Role, user.Company, Admin = tbl == null ? "-" : tbl.FirstName, DistributorName = val == null ? "-" : val.FirstName,user.IsActive };
                        int i = 0;
                        foreach (var user in users)
                        {
                            i++;
                            manageUserList.Add(
                            new ManageUserModel
                            {
                                Id = user.C_id,
                                SrNo = i,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Role = user.Role,
                                UserName = user.UserName,
                                AdminName = user.Admin,
                                DistributorName = user.DistributorName,
                                CompanyName = user.Company,
                                IsActive = user.IsActive
                            }
                            );
                        }
                    }
                    if (userDetails.Role.Equals(Constant.ROLE_MERCHANT))
                    {
                        var users = from user in dbContext.user_personal_details
                                    join admin in dbContext.user_personal_details on user.AdminId equals admin.C_id into tempJoin
                                    from tbl in tempJoin.DefaultIfEmpty()
                                    join distributor in dbContext.user_personal_details on user.DistributorID equals distributor.C_id into finalTbl
                                    from val in finalTbl.DefaultIfEmpty()
                                    where user.C_id == userDetails.C_id && (user.FirstName.ToLower().Contains(searchString.ToLower()) || user.LastName.ToLower().Contains(searchString.ToLower()))
                                    select new { user.C_id, user.FirstName, user.LastName, user.UserName, user.Role, user.Company, Admin = tbl == null ? "-" : tbl.FirstName, DistributorName = val == null ? "-" : val.FirstName,user.IsActive };
                        int i = 0;
                        foreach (var user in users)
                        {
                            i++;
                            manageUserList.Add(
                            new ManageUserModel
                            {
                                Id = user.C_id,
                                SrNo = i,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Role = user.Role,
                                UserName = user.UserName,
                                AdminName = user.Admin,
                                DistributorName = user.DistributorName,
                                CompanyName = user.Company,
                                IsActive = user.IsActive
                            }
                            );
                        }
                    }
                }

                else
                {
                    if (userDetails.Role.Equals(Constant.ROLE_SUPER_ADMIN))
                    {
                        var users = from user in dbContext.user_personal_details
                                    where user.C_id > 3
                                    join admin in dbContext.user_personal_details on user.AdminId equals admin.C_id into tempJoin
                                    from tbl in tempJoin.DefaultIfEmpty()
                                    join distributor in dbContext.user_personal_details on user.DistributorID equals distributor.C_id into finalTbl
                                    from val in finalTbl.DefaultIfEmpty()
                                    select new { user.C_id, user.FirstName, user.LastName, user.UserName, user.Role, user.Company,Admin = tbl == null ? "-" : tbl.FirstName, DistributorName = val == null ? "-" : val.FirstName, user.IsActive };
                        int i = 0;
                        foreach (var user in users)
                        {
                            i++;
                            manageUserList.Add(
                            new ManageUserModel
                            {
                                Id = user.C_id,
                                SrNo = i,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Role = user.Role,
                                UserName = user.UserName,
                                AdminName = user.Admin,
                                DistributorName = user.DistributorName,
                                CompanyName = user.Company,
                                IsActive = user.IsActive
                            }
                            );
                        }
                    }
                    if (userDetails.Role.Equals(Constant.ROLE_ADMIN))
                    {
                        var users = from user in dbContext.user_personal_details
                                    join admin in dbContext.user_personal_details on user.AdminId equals admin.C_id into tempJoin
                                    from tbl in tempJoin.DefaultIfEmpty()
                                    join distributor in dbContext.user_personal_details on user.DistributorID equals distributor.C_id into finalTbl
                                    from val in finalTbl.DefaultIfEmpty()
                                    where user.AdminId == userDetails.C_id
                                    select new { user.C_id, user.FirstName, user.LastName, user.UserName, user.Role, user.Company,Admin = tbl == null ? "-" : tbl.FirstName, DistributorName = val == null ? "-" : val.FirstName,user.IsActive };
                        int i = 0;
                        foreach (var user in users)
                        {
                            i++;
                            manageUserList.Add(
                            new ManageUserModel
                            {
                                Id = user.C_id,
                                SrNo = i,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Role = user.Role,
                                UserName = user.UserName,
                                AdminName = user.Admin,
                                DistributorName = user.DistributorName,
                                CompanyName = user.Company,
                                IsActive = user.IsActive
                            }
                            );
                        }
                    }
                    if (userDetails.Role.Equals(Constant.ROLE_DISTRIBUTOR))
                    {
                        var users = from user in dbContext.user_personal_details
                                    join admin in dbContext.user_personal_details on user.AdminId equals admin.C_id into tempJoin
                                    from tbl in tempJoin.DefaultIfEmpty()
                                    join distributor in dbContext.user_personal_details on user.DistributorID equals distributor.C_id into finalTbl
                                    from val in finalTbl.DefaultIfEmpty()
                                    where user.DistributorID == userDetails.C_id
                                    select new { user.C_id, user.FirstName, user.LastName, user.UserName, user.Role, user.Company, Admin = tbl == null ? "-" : tbl.FirstName, DistributorName = val == null ? "-" : val.FirstName ,user.IsActive};
                        int i = 0;
                        foreach (var user in users)
                        {
                            i++;
                            manageUserList.Add(
                            new ManageUserModel
                            {
                                Id = user.C_id,
                                SrNo = i,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Role = user.Role,
                                UserName = user.UserName,
                                AdminName = user.Admin,
                                DistributorName = user.DistributorName,
                                CompanyName = user.Company,
                                IsActive = user.IsActive
                            }
                            );
                        }
                    }
                    if (userDetails.Role.Equals(Constant.ROLE_MERCHANT))
                    {
                        var users = from user in dbContext.user_personal_details
                                    join admin in dbContext.user_personal_details on user.AdminId equals admin.C_id into tempJoin
                                    from tbl in tempJoin.DefaultIfEmpty()
                                    join distributor in dbContext.user_personal_details on user.DistributorID equals distributor.C_id into finalTbl
                                    from val in finalTbl.DefaultIfEmpty()
                                    where user.C_id == userDetails.C_id
                                    select new { user.C_id, user.FirstName, user.LastName, user.UserName, user.Role, user.Company, Admin = tbl == null ? "-" : tbl.FirstName, DistributorName = val == null ? "-" : val.FirstName , user.IsActive};
                        int i = 0;
                        foreach (var user in users)
                        {
                            i++;
                            manageUserList.Add(
                            new ManageUserModel
                            {
                                Id = user.C_id,
                                SrNo = i,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Role = user.Role,
                                UserName = user.UserName,
                                AdminName = user.Admin,
                                DistributorName = user.DistributorName,
                                CompanyName = user.Company,
                                IsActive = user.IsActive
                            }
                            );
                        }
                    }
                }
                return manageUserList;
            }
            return null;


        }
        private List<SelectListItem> GetAllRoles(user_personal_details userDetails)
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            var role = dbContext.user_roles.Where(i => i.RoleName.Equals(userDetails.Role));
            int roleId = Convert.ToInt32(role.First().C_id);
            List<user_roles> userRole = new List<user_roles>();
            if (userDetails.Role == Constant.ROLE_DISTRIBUTOR)
            {
                var roles = dbContext.user_roles.Where(s => s.C_id > roleId);
                userRole = roles.ToList();
            }
            else
            {
                var roles = dbContext.user_roles.Where(s => s.C_id > roleId && s.C_id != 4);
                userRole = roles.ToList();
            }
            List<SelectListItem> temproleNames = new List<SelectListItem>();
            userRole.ForEach(x =>
            {
                temproleNames.Add(new SelectListItem { Text = x.RoleName, Value = x.RoleName });
            });
            return temproleNames;
        }
        private List<SelectListItem> GetAllCountry()
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();

            List<country> userRole = dbContext.countries.ToList();
            List<SelectListItem> temproleNames = new List<SelectListItem>();
            userRole.ForEach(x =>
            {
                temproleNames.Add(new SelectListItem { Text = x.Name, Value = x.C_id.ToString() });
            });
            return temproleNames;
        }
        private List<SelectListItem> GetAllSecurityQue()
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            List<security_questions> userRole = dbContext.security_questions.ToList();
            List<SelectListItem> temproleNames = new List<SelectListItem>();
            userRole.ForEach(x =>
            {
                temproleNames.Add(new SelectListItem { Text = x.Question, Value = x.C_id.ToString() });
            });
            return temproleNames;
        }
        private void SendEmailToUser(UserCreationModel userDetails)
        {
            try
            {
                EmailModel emailModel = new EmailModel();
                emailModel.To = userDetails.EmailId;
                emailModel.Subject = "Credentials for pay to cards";
                emailModel.Body = "<p>Hello " + userDetails.FirstName + ",</p><p>Your user credentials for login are: username/password -<br/>"
                    + userDetails.UserName + "/" + userDetails.Password + "<br/>The API password for this login is " + userDetails.ApiPassword + "</p><p>Thank you.</p>Regards,<br/>Administrator.<br/><br/><small>Please do not reply to this mail.This is an auto generated mail and replies to this email id are not attended.</small>";
                emailModel.CC = "vishalexample26@gmail.com";
                Utility.SendEmail(emailModel.To, emailModel.Subject, emailModel.Body, emailModel.CC);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}