using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayToCardsSystem.Models;
using PayToCardsSystem.Service;
using PayToCardsSystem.AppCode.CommonCode;

namespace PayToCardsSystem.Controllers
{
    public class MyProfileController : Controller
    {
        // GET: MyProfile
        CurrencyService currencyUser = new CurrencyService();
        public ActionResult MyProfile()
        {
            try
            {
                if (Session["UserPersonalDetails"] != null)
                {
                    var userDetailsObject = (user_personal_details)Session["UserPersonalDetails"];                  
                    vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
                    UserCreationModel userCreation = new UserCreationModel();
                    user_personal_details userDetails = dbContext.user_personal_details.FirstOrDefault(i => i.C_id == userDetailsObject.C_id);

                    userCreation.CountryNameList = GetAllCountry();
                    userCreation.CurrencyList = currencyUser.GetAllCurrencyWithInActive(userDetails);
                    userCreation.SecurityList = GetAllSecurityQue();
                    userCreation.ListFeeCalculation = GetFeeCalculation();
                    userCreation.SelectedCurrency.ListFeeCal = GetFeeCalculation();
                    userCreation.SelectedCurrency.Cur = currencyUser.GetAllCurrencyWithInActive(userDetails);
                   // userCreation.IsActive = true;

                    userCreation.FirstName = userDetails.FirstName;
                    userCreation.LastName = userDetails.LastName;
                    userCreation.UserName = userDetails.UserName;
                    userCreation.CompanyName = userDetails.Company;
                    userCreation.Address1 = userDetails.AddressLine1;
                    userCreation.Address2 = userDetails.AddressLine2;
                    userCreation.City = userDetails.City;
                    userCreation.CountryValue = userDetails.Country;
                    userCreation.PostalCode = userDetails.Zip;
                    userCreation.EmailId = userDetails.Email;
                    userCreation.Telephone = userDetails.ContactNo1;
                    userCreation.SecurityValue = userDetails.SecurityQuestNo.ToString();
                    userCreation.Answer = userDetails.SecurityAnswer;
                  
                    userCreation.AllowIpAddress = userDetails.AllowIPAddress;
                    userCreation.BlockIpAddress = userDetails.BlockIPAddress;
                    userCreation.IsActive = userDetails.IsActive;
                    int id = Convert.ToInt32(userDetails.C_id);
                    var feeDetails = (from fee in dbContext.user_fee_details
                                      from currencyName in dbContext.currencies
                                      where fee.CurrencyId == currencyName.C_id && fee.UserId == id
                                      select new { fee.C_id, fee.CurrencyId, currencyName.ShortCode, fee.PercentFee, fee.FlatFee, fee.IsActive, fee.FeeType });
                    foreach (var user in feeDetails)
                    {
                        AddUserCurrency feeCur = new AddUserCurrency();
                        feeCur.Cur = currencyUser.GetAllCurrencyWithInActive(userDetails);
                        feeCur.ListFeeCal = GetFeeCalculation();
                        feeCur.SrNo = (userCreation.listCurrency.Count + 1);
                        feeCur.Id = user.C_id;
                        feeCur.CurrencyId = user.CurrencyId;
                        feeCur.CurrencyName = user.ShortCode;
                        feeCur.PercentAmount = user.PercentFee;
                        feeCur.FlatAmount = user.FlatFee;
                        feeCur.IsActive = (bool)user.IsActive;
                        feeCur.SelectItem(user.CurrencyId.ToString());
                        feeCur.FeeValue = user.FeeType;
                        feeCur.SelectItemFee(user.FeeType);
                        userCreation.listCurrency.Add(feeCur);                      
                    }

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MyProfile(UserCreationModel userCreationObject)
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            var userDetails = (user_personal_details)Session["UserPersonalDetails"];
            try
            {
                int userId = Convert.ToInt32(userDetails.C_id);             
                user_personal_details userPersonalDetails = dbContext.user_personal_details.FirstOrDefault(i => i.C_id == userId);
                {
                    userPersonalDetails.FirstName = userCreationObject.FirstName;
                    userPersonalDetails.LastName = userCreationObject.LastName;
                    userPersonalDetails.Email = userCreationObject.EmailId;
                    userPersonalDetails.ContactNo1 = userCreationObject.Telephone;
                    userPersonalDetails.AddressLine1 = userCreationObject.Address1;
                    userPersonalDetails.AddressLine2 = userCreationObject.Address2;
                    userPersonalDetails.City = userCreationObject.City;
                    userPersonalDetails.Country = userCreationObject.CountryValue;
                    userPersonalDetails.Zip = userCreationObject.PostalCode;
                    userPersonalDetails.Company = userCreationObject.CompanyName;
                    userPersonalDetails.AllowIPAddress = userCreationObject.AllowIpAddress;
                    userPersonalDetails.BlockIPAddress = userCreationObject.BlockIpAddress;
                    userPersonalDetails.SecurityQuestNo = Convert.ToInt16(userCreationObject.SecurityValue);
                    userPersonalDetails.SecurityAnswer = userCreationObject.Answer;
                                                                         
                    dbContext.SaveChanges();
                };
                for (int i = 0; i < userCreationObject.listCurrency.Count; i++)
                {
                    AddUserCurrency obj = userCreationObject.listCurrency[i];
                    obj.Cur = currencyUser.GetAllCurrencyWithInActive(userDetails);
                    obj.SelectItem(obj.CurrencyId.ToString());
                    obj.ListFeeCal = GetFeeCalculation();
                    obj.SelectItemFee(obj.FeeValue);
                    user_fee_details userFeeDetails = dbContext.user_fee_details.Where(f => f.C_id == obj.Id).FirstOrDefault();
                    {   
                        if(userPersonalDetails.Role.Equals(Constant.ROLE_SUPER_ADMIN))
                        { 
                            userFeeDetails.UserId = userPersonalDetails.C_id;
                            userFeeDetails.PercentFee = obj.PercentAmount;
                            userFeeDetails.FlatFee = obj.FlatAmount;
                            userFeeDetails.ComissionAmount = 0;
                            userFeeDetails.CurrencyId = obj.CurrencyId;                          
                            userFeeDetails.FeeType = obj.FeeValue;
                        }               
                        userFeeDetails.IsActive = obj.IsActive;                                           
                        dbContext.SaveChanges();
                    };
                    user_currency_details userCurrencyDetails = dbContext.user_currency_details.FirstOrDefault(j => j.UserId == userPersonalDetails.C_id && j.CurrencyId == userFeeDetails.CurrencyId);
                    {
                        if (userPersonalDetails.Role.Equals(Constant.ROLE_SUPER_ADMIN))
                        {                            
                            userCurrencyDetails.UserId = userPersonalDetails.C_id;
                            userCurrencyDetails.CurrencyId = obj.CurrencyId;
                            userCurrencyDetails.Currency = obj.CurrencyName;
                        }
                        userCurrencyDetails.IsActive = obj.IsActive;                   
                        dbContext.SaveChanges();
                    };
                    if (!obj.IsActive && userId > 0)
                    {
                        if (userPersonalDetails.Role.Equals(Constant.ROLE_SUPER_ADMIN))
                        {
                            string query = "UPDATE user_fee_details set IsActive = 0 where CurrencyId = " + userFeeDetails.CurrencyId + " AND UserID  > 3";
                            var updateResult = dbContext.ExecuteStoreCommand(query);

                            query = "UPDATE user_currency_details set IsActive = 0 where CurrencyId = " + userFeeDetails.CurrencyId + " AND UserID > 3";
                            var updateResult1 = dbContext.ExecuteStoreCommand(query);
                        }
                        else if (userPersonalDetails.Role.Equals(Constant.ROLE_DISTRIBUTOR))                            
                        {
                            string query = "UPDATE user_fee_details set IsActive = 0 where CurrencyId = " + userFeeDetails.CurrencyId + " AND UserID in (Select _id from user_personal_details where DistributorId = " + userId + ")";
                            var updateResult = dbContext.ExecuteStoreCommand(query);

                            query = "UPDATE user_currency_details set IsActive = 0 where CurrencyId = " + userFeeDetails.CurrencyId + " AND UserID in (Select _id from user_personal_details where DistributorId = " + userId + ")";
                            var updateResult1 = dbContext.ExecuteStoreCommand(query);
                        }
                        else if (userPersonalDetails.Role.Equals(Constant.ROLE_ADMIN))
                        {
                            string query = "UPDATE user_fee_details set IsActive = 0 where CurrencyId = " + userFeeDetails.CurrencyId + " AND UserID in (Select _id from user_personal_details where AdminId = " + userId + ")";
                            var updateResult = dbContext.ExecuteStoreCommand(query);

                            query = "UPDATE user_currency_details set IsActive = 0 where CurrencyId = " + userFeeDetails.CurrencyId + " AND UserID in (Select _id from user_personal_details where AdminId = " + userId + ")";
                            var updateResult1 = dbContext.ExecuteStoreCommand(query);
                        }
                    }
                }               
            }
            catch (Exception e)
            {
                return View("Error");
            }
            return RedirectToAction("Dashboard", "Dashboard");
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
        private List<SelectListItem> GetFeeCalculation()
        {
            SelectListItem si = new SelectListItem();
            List<SelectListItem> FeeCalc = new List<SelectListItem>();
            FeeCalc.Add(new SelectListItem { Text = "AND", Value = "AND" });
            FeeCalc.Add(new SelectListItem { Text = "OR", Value = "OR" });
            return FeeCalc;
        }
    }
}