using PayToCardsSystem.AppCode.CommonCode;
using PayToCardsSystem.Models;
using PayToCardsSystem.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayToCardsSystem.Controllers
{
    public class BalanceHistoryController : Controller
    {
        UserService userService = new UserService();
        CurrencyService currencyUser = new CurrencyService();
        public ActionResult BalanceView()
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            if (Session["UserPersonalDetails"] != null)
            {
                var userDetails = (user_personal_details)Session["UserPersonalDetails"];
                BalanceListModel balanceHistory = new BalanceListModel();
                balanceHistory.CurrencyList = currencyUser.GetAllCurrencyWithInActive(userDetails);
                balanceHistory.UserList = userService.GetAllUserRoleWiseIncludeLogin(userDetails);
                string strUser = null;
                string strCurrency = null;
                balanceHistory.ListBalanceHistory = GetAllBalanceHistory(strUser, strCurrency, userDetails);
                return View(balanceHistory);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }
        [HttpPost]
        [SubmitButtonSelector(Name = "Search")]
        public ActionResult Search(BalanceListModel balanceHistoryModel)
        {
            var userDetails = (user_personal_details)Session["UserPersonalDetails"];
            BalanceListModel balanceHistory = new BalanceListModel();
            balanceHistory.CurrencyList = currencyUser.GetAllCurrencyWithInActive(userDetails);

            balanceHistory.UserList = userService.GetAllUserRoleWiseIncludeLogin(userDetails);

            string strUser = balanceHistoryModel.UserValue;
            string strCurrency = balanceHistoryModel.CurrencyValue;
            balanceHistory.ListBalanceHistory = GetAllBalanceHistory(strUser, strCurrency, userDetails);
            return View(balanceHistory);
        }


        #region Get Balance List (RoleWise)
        private List<BalanceHistoryViewModel> GetSuperAdminList(string userSearch, string currencySearch, user_personal_details userDetails)
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            List<BalanceHistoryViewModel> manageUserList = new List<BalanceHistoryViewModel>();

            if (string.IsNullOrEmpty(userSearch) && string.IsNullOrEmpty(currencySearch))
            {
                var transDetails = (from currency in dbContext.user_currency_details
                                    join
                                    personal in dbContext.user_personal_details on currency.UserId equals personal.C_id
                                    where personal.C_id > 3 && personal.IsActive == true
                                    orderby currency.UserId
                                    select new { currency.C_id, currency.UserId, currency.Balance, currency.Currency, personal.FirstName, personal.LastName, currency.IsActive });
                int i = 0;
                foreach (var user in transDetails)
                {
                    i++;
                    //user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(s => s.C_id == userid);          
                    bool isAdd = true;
                    bool isActive = (bool)user.IsActive;
                    if (!isActive && Convert.ToDouble(user.Balance) == 0)
                        isAdd = false;

                    if (isAdd)
                    {
                        manageUserList.Add(
                        new BalanceHistoryViewModel
                        {
                            UserId = user.UserId,
                            Id = user.C_id,
                            SrNo = i,
                            Name = user.FirstName + " " + user.LastName,
                            Balance = Convert.ToDouble(user.Balance),
                            Currency = user.Currency,
                            IsActive = (bool)user.IsActive
                        }
                        );
                    }
                }
            }
            else if (!string.IsNullOrEmpty(userSearch) && string.IsNullOrEmpty(currencySearch))
            {
                int userId = Convert.ToInt32(userSearch);
                var transDetails = (from currency in dbContext.user_currency_details
                                    join
                                    personal in dbContext.user_personal_details on currency.UserId equals personal.C_id
                                    where personal.C_id == userId && personal.C_id > 3 && personal.IsActive == true
                                    orderby currency.UserId
                                    select new { currency.C_id, currency.UserId, currency.Balance, currency.Currency, personal.FirstName, personal.LastName, currency.IsActive });
                int i = 0;
                foreach (var user in transDetails)
                {
                    i++;
                    //user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(s => s.C_id == userid);
                    bool isAdd = true;
                    bool isActive = (bool)user.IsActive;
                    if (!isActive && Convert.ToDouble(user.Balance) == 0)
                        isAdd = false;
                    if (isAdd)
                    {
                        manageUserList.Add(
                        new BalanceHistoryViewModel
                        {
                            UserId = user.UserId,
                            Id = user.C_id,
                            SrNo = i,
                            Name = user.FirstName + " " + user.LastName,
                            Balance = Convert.ToDouble(user.Balance),
                            Currency = user.Currency,
                            IsActive = (bool)user.IsActive
                        }
                        );
                    }
                }
            }
            else if (string.IsNullOrEmpty(userSearch) && !string.IsNullOrEmpty(currencySearch))
            {
                int currencyId = Convert.ToInt32(currencySearch);
                var transDetails = (from currency in dbContext.user_currency_details
                                    join
                                    personal in dbContext.user_personal_details on currency.UserId equals personal.C_id
                                    where currency.CurrencyId == currencyId && personal.C_id > 3 && personal.IsActive == true
                                    orderby currency.UserId
                                    select new { currency.C_id, currency.UserId, currency.Balance, currency.Currency, personal.FirstName, personal.LastName, currency.IsActive });
                int i = 0;
                foreach (var user in transDetails)
                {
                    i++;
                    //user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(s => s.C_id == userid);
                    bool isAdd = true;
                    bool isActive = (bool)user.IsActive;
                    if (!isActive && Convert.ToDouble(user.Balance) == 0)
                        isAdd = false;
                    if (isAdd)
                    {
                        manageUserList.Add(
                        new BalanceHistoryViewModel
                        {
                            UserId = user.UserId,
                            Id = user.C_id,
                            SrNo = i,
                            Name = user.FirstName + " " + user.LastName,
                            Balance = Convert.ToDouble(user.Balance),
                            Currency = user.Currency,
                            IsActive = (bool)user.IsActive
                        }
                        );
                    }
                }

            }
            else if (!string.IsNullOrEmpty(userSearch) && !string.IsNullOrEmpty(currencySearch))
            {
                int userId = Convert.ToInt32(userSearch);
                int currencyId = Convert.ToInt32(currencySearch);
                var transDetails = (from currency in dbContext.user_currency_details
                                    join
                                    personal in dbContext.user_personal_details on currency.UserId equals personal.C_id
                                    where currency.CurrencyId == currencyId && personal.C_id == userId && personal.C_id > 3 && personal.IsActive == true
                                    orderby currency.UserId
                                    select new { currency.C_id, currency.UserId, currency.Balance, currency.Currency, personal.FirstName, personal.LastName, currency.IsActive });
                int i = 0;
                foreach (var user in transDetails)
                {
                    i++;
                    //user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(s => s.C_id == userid);
                    bool isAdd = true;
                    bool isActive = (bool)user.IsActive;
                    if (!isActive && Convert.ToDouble(user.Balance) == 0)
                        isAdd = false;
                    if (isAdd)
                    {
                        manageUserList.Add(
                        new BalanceHistoryViewModel
                        {
                            UserId = user.UserId,
                            Id = user.C_id,
                            SrNo = i,
                            Name = user.FirstName + " " + user.LastName,
                            Balance = Convert.ToDouble(user.Balance),
                            Currency = user.Currency,
                            IsActive = (bool)user.IsActive
                        }
                        );
                    }
                }
            }
            return manageUserList;
        }
        private List<BalanceHistoryViewModel> GetAdminList(string userSearch, string currencySearch, user_personal_details userDetails)
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            List<BalanceHistoryViewModel> manageUserList = new List<BalanceHistoryViewModel>();

            if (string.IsNullOrEmpty(userSearch) && string.IsNullOrEmpty(currencySearch))
            {
                var transDetails = (from currency in dbContext.user_currency_details
                                    join
                                    personal in dbContext.user_personal_details on currency.UserId equals personal.C_id
                                    where (personal.AdminId == userDetails.C_id || personal.C_id == userDetails.C_id) && personal.IsActive == true
                                    orderby currency.UserId
                                    select new { currency.C_id, currency.UserId, currency.Balance, currency.Currency, personal.FirstName, personal.LastName, currency.IsActive });
                int i = 0;
                foreach (var user in transDetails)
                {
                    i++;
                    //user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(s => s.C_id == userid);
                    bool isAdd = true;
                    bool isActive = (bool)user.IsActive;
                    if (!isActive && Convert.ToDouble(user.Balance) == 0)
                        isAdd = false;
                    if (isAdd)
                    {
                        manageUserList.Add(
                        new BalanceHistoryViewModel
                        {
                            UserId = user.UserId,
                            Id = user.C_id,
                            SrNo = i,
                            Name = user.FirstName + " " + user.LastName,
                            Balance = Convert.ToDouble(user.Balance),
                            Currency = user.Currency,
                            IsActive = (bool)user.IsActive
                        }
                        );
                    }
                }
            }
            else if (!string.IsNullOrEmpty(userSearch) && string.IsNullOrEmpty(currencySearch))
            {
                int userId = Convert.ToInt32(userSearch);
                var transDetails = (from currency in dbContext.user_currency_details
                                    join
                                    personal in dbContext.user_personal_details on currency.UserId equals personal.C_id
                                    where personal.C_id == userId && personal.IsActive == true
                                    orderby currency.UserId
                                    select new { currency.C_id, currency.UserId, currency.Balance, currency.Currency, personal.FirstName, personal.LastName, currency.IsActive });
                int i = 0;
                foreach (var user in transDetails)
                {
                    i++;
                    //user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(s => s.C_id == userid);
                    bool isAdd = true;
                    bool isActive = (bool)user.IsActive;
                    if (!isActive && Convert.ToDouble(user.Balance) == 0)
                        isAdd = false;
                    if (isAdd)
                    {
                        manageUserList.Add(
                        new BalanceHistoryViewModel
                        {
                            UserId = user.UserId,
                            Id = user.C_id,
                            SrNo = i,
                            Name = user.FirstName + " " + user.LastName,
                            Balance = Convert.ToDouble(user.Balance),
                            Currency = user.Currency,
                            IsActive = (bool)user.IsActive
                        }
                        );
                    }
                }
            }
            else if (string.IsNullOrEmpty(userSearch) && !string.IsNullOrEmpty(currencySearch))
            {
                int currencyId = Convert.ToInt32(currencySearch);
                var transDetails = (from currency in dbContext.user_currency_details
                                    join
                                    personal in dbContext.user_personal_details on currency.UserId equals personal.C_id
                                    where currency.CurrencyId == currencyId && (personal.AdminId == userDetails.C_id || personal.C_id == userDetails.C_id) && personal.IsActive == true
                                    orderby currency.UserId
                                    select new { currency.C_id, currency.UserId, currency.Balance, currency.Currency, personal.FirstName, personal.LastName, currency.IsActive });
                int i = 0;
                foreach (var user in transDetails)
                {
                    i++;
                    //user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(s => s.C_id == userid);
                    bool isAdd = true;
                    bool isActive = (bool)user.IsActive;
                    if (!isActive && Convert.ToDouble(user.Balance) == 0)
                        isAdd = false;
                    if (isAdd)
                    {
                        manageUserList.Add(
                        new BalanceHistoryViewModel
                        {
                            UserId = user.UserId,
                            Id = user.C_id,
                            SrNo = i,
                            Name = user.FirstName + " " + user.LastName,
                            Balance = Convert.ToDouble(user.Balance),
                            Currency = user.Currency,
                            IsActive = (bool)user.IsActive
                        }
                        );
                    }
                }

            }
            else if (!string.IsNullOrEmpty(userSearch) && !string.IsNullOrEmpty(currencySearch))
            {
                int userId = Convert.ToInt32(userSearch);
                int currencyId = Convert.ToInt32(currencySearch);
                var transDetails = (from currency in dbContext.user_currency_details
                                    join
                                    personal in dbContext.user_personal_details on currency.UserId equals personal.C_id
                                    where currency.CurrencyId == currencyId && personal.C_id == userId && personal.IsActive == true
                                    orderby currency.UserId
                                    select new { currency.C_id, currency.UserId, currency.Balance, currency.Currency, personal.FirstName, personal.LastName, currency.IsActive });
                int i = 0;
                foreach (var user in transDetails)
                {
                    i++;
                    //user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(s => s.C_id == userid);
                    bool isAdd = true;
                    bool isActive = (bool)user.IsActive;
                    if (!isActive && Convert.ToDouble(user.Balance) == 0)
                        isAdd = false;
                    if (isAdd)
                    {
                        manageUserList.Add(
                        new BalanceHistoryViewModel
                        {
                            UserId = user.UserId,
                            Id = user.C_id,
                            SrNo = i,
                            Name = user.FirstName + " " + user.LastName,
                            Balance = Convert.ToDouble(user.Balance),
                            Currency = user.Currency,
                            IsActive = (bool)user.IsActive
                        }
                        );
                    }
                }
            }
            return manageUserList;
        }
        private List<BalanceHistoryViewModel> GetDistributorList(string userSearch, string currencySearch, user_personal_details userDetails)
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            List<BalanceHistoryViewModel> manageUserList = new List<BalanceHistoryViewModel>();


            if (string.IsNullOrEmpty(userSearch) && string.IsNullOrEmpty(currencySearch))
            {
                var transDetails = (from currency in dbContext.user_currency_details
                                    join
                                    personal in dbContext.user_personal_details on currency.UserId equals personal.C_id
                                    where (personal.DistributorID == userDetails.C_id || personal.C_id == userDetails.C_id) && personal.IsActive == true
                                    orderby currency.UserId
                                    select new { currency.C_id, currency.UserId, currency.Balance, currency.Currency, personal.FirstName, personal.LastName, currency.IsActive });
                int i = 0;
                foreach (var user in transDetails)
                {
                    i++;
                    //user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(s => s.C_id == userid);
                    bool isAdd = true;
                    bool isActive = (bool)user.IsActive;
                    if (!isActive && Convert.ToDouble(user.Balance) == 0)
                        isAdd = false;
                    if (isAdd)
                    {
                        manageUserList.Add(
                        new BalanceHistoryViewModel
                        {
                            UserId = user.UserId,
                            Id = user.C_id,
                            SrNo = i,
                            Name = user.FirstName + " " + user.LastName,
                            Balance = Convert.ToDouble(user.Balance),
                            Currency = user.Currency,
                            IsActive = (bool)user.IsActive
                        }
                        );
                    }
                }
            }
            else if (!string.IsNullOrEmpty(userSearch) && string.IsNullOrEmpty(currencySearch))
            {
                int userId = Convert.ToInt32(userSearch);
                var transDetails = (from currency in dbContext.user_currency_details
                                    join
                                    personal in dbContext.user_personal_details on currency.UserId equals personal.C_id
                                    where (personal.C_id == userId) && personal.IsActive == true
                                    orderby currency.UserId
                                    select new { currency.C_id, currency.UserId, currency.Balance, currency.Currency, personal.FirstName, personal.LastName, currency.IsActive });
                int i = 0;
                foreach (var user in transDetails)
                {
                    i++;
                    //user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(s => s.C_id == userid);
                    bool isAdd = true;
                    bool isActive = (bool)user.IsActive;
                    if (!isActive && Convert.ToDouble(user.Balance) == 0)
                        isAdd = false;
                    if (isAdd)
                    {
                        manageUserList.Add(
                        new BalanceHistoryViewModel
                        {
                            UserId = user.UserId,
                            Id = user.C_id,
                            SrNo = i,
                            Name = user.FirstName + " " + user.LastName,
                            Balance = Convert.ToDouble(user.Balance),
                            Currency = user.Currency,
                            IsActive = (bool)user.IsActive
                        }
                        );
                    }
                }
            }
            else if (string.IsNullOrEmpty(userSearch) && !string.IsNullOrEmpty(currencySearch))
            {
                int currencyId = Convert.ToInt32(currencySearch);
                var transDetails = (from currency in dbContext.user_currency_details
                                    join
                                    personal in dbContext.user_personal_details on currency.UserId equals personal.C_id
                                    where currency.CurrencyId == currencyId && (personal.DistributorID == userDetails.C_id || personal.C_id == userDetails.C_id) && personal.IsActive == true
                                    orderby currency.UserId
                                    select new { currency.C_id, currency.UserId, currency.Balance, currency.Currency, personal.FirstName, personal.LastName, currency.IsActive });
                int i = 0;
                foreach (var user in transDetails)
                {
                    i++;
                    //user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(s => s.C_id == userid);
                    bool isAdd = true;
                    bool isActive = (bool)user.IsActive;
                    if (!isActive && Convert.ToDouble(user.Balance) == 0)
                        isAdd = false;
                    if (isAdd)
                    {
                        manageUserList.Add(
                        new BalanceHistoryViewModel
                        {
                            UserId = user.UserId,
                            Id = user.C_id,
                            SrNo = i,
                            Name = user.FirstName + " " + user.LastName,
                            Balance = Convert.ToDouble(user.Balance),
                            Currency = user.Currency,
                            IsActive = (bool)user.IsActive
                        }
                        );
                    }
                }

            }
            else if (!string.IsNullOrEmpty(userSearch) && !string.IsNullOrEmpty(currencySearch))
            {
                int userId = Convert.ToInt32(userSearch);
                int currencyId = Convert.ToInt32(currencySearch);
                var transDetails = (from currency in dbContext.user_currency_details
                                    join
                                    personal in dbContext.user_personal_details on currency.UserId equals personal.C_id
                                    where currency.CurrencyId == currencyId && personal.C_id == userId  && personal.IsActive == true
                                    orderby currency.UserId
                                    select new { currency.C_id, currency.UserId, currency.Balance, currency.Currency, personal.FirstName, personal.LastName, currency.IsActive });
                int i = 0;
                foreach (var user in transDetails)
                {
                    i++;
                    //user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(s => s.C_id == userid);
                    bool isAdd = true;
                    bool isActive = (bool)user.IsActive;
                    if (!isActive && Convert.ToDouble(user.Balance) == 0)
                        isAdd = false;
                    if (isAdd)
                    {
                        manageUserList.Add(
                        new BalanceHistoryViewModel
                        {
                            UserId = user.UserId,
                            Id = user.C_id,
                            SrNo = i,
                            Name = user.FirstName + " " + user.LastName,
                            Balance = Convert.ToDouble(user.Balance),
                            Currency = user.Currency,
                            IsActive = (bool)user.IsActive
                        }
                        );
                    }
                }
            }
            return manageUserList;
        }
        private List<BalanceHistoryViewModel> GetMerchantList(string userSearch, string currencySearch, user_personal_details userDetails)
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            List<BalanceHistoryViewModel> manageUserList = new List<BalanceHistoryViewModel>();


            if (string.IsNullOrEmpty(userSearch) && string.IsNullOrEmpty(currencySearch))
            {
                var transDetails = (from currency in dbContext.user_currency_details
                                    join
                                    personal in dbContext.user_personal_details on currency.UserId equals personal.C_id
                                    where personal.C_id == userDetails.C_id && personal.IsActive == true
                                    orderby currency.UserId
                                    select new { currency.C_id, currency.UserId, currency.Balance, currency.Currency, personal.FirstName, personal.LastName, currency.IsActive });
                int i = 0;
                foreach (var user in transDetails)
                {
                    i++;
                    //user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(s => s.C_id == userid);
                    bool isAdd = true;
                    bool isActive = (bool)user.IsActive;
                    if (!isActive && Convert.ToDouble(user.Balance) == 0)
                        isAdd = false;
                    if (isAdd)
                    {
                        manageUserList.Add(
                        new BalanceHistoryViewModel
                        {
                            UserId = user.UserId,
                            Id = user.C_id,
                            SrNo = i,
                            Name = user.FirstName + " " + user.LastName,
                            Balance = Convert.ToDouble(user.Balance),
                            Currency = user.Currency,
                            IsActive = (bool)user.IsActive
                        }
                        );
                    }
                }
            }
            else if (!string.IsNullOrEmpty(userSearch) && string.IsNullOrEmpty(currencySearch))
            {
                int userId = Convert.ToInt32(userSearch);
                var transDetails = (from currency in dbContext.user_currency_details
                                    join
                                    personal in dbContext.user_personal_details on currency.UserId equals personal.C_id
                                    where personal.C_id == userDetails.C_id && personal.C_id == userId && personal.IsActive == true
                                    orderby currency.UserId
                                    select new { currency.C_id, currency.UserId, currency.Balance, currency.Currency, personal.FirstName, personal.LastName, currency.IsActive });
                int i = 0;
                foreach (var user in transDetails)
                {
                    i++;
                    //user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(s => s.C_id == userid);
                    bool isAdd = true;
                    bool isActive = (bool)user.IsActive;
                    if (!isActive && Convert.ToDouble(user.Balance) == 0)
                        isAdd = false;
                    if (isAdd)
                    {
                        manageUserList.Add(
                        new BalanceHistoryViewModel
                        {
                            UserId = user.UserId,
                            Id = user.C_id,
                            SrNo = i,
                            Name = user.FirstName + " " + user.LastName,
                            Balance = Convert.ToDouble(user.Balance),
                            Currency = user.Currency,
                            IsActive = (bool)user.IsActive
                        }
                        );
                    }
                }
            }
            else if (string.IsNullOrEmpty(userSearch) && !string.IsNullOrEmpty(currencySearch))
            {
                int currencyId = Convert.ToInt32(currencySearch);
                var transDetails = (from currency in dbContext.user_currency_details
                                    join
                                    personal in dbContext.user_personal_details on currency.UserId equals personal.C_id
                                    where currency.CurrencyId == currencyId && personal.C_id == userDetails.C_id && personal.IsActive == true
                                    orderby currency.UserId
                                    select new { currency.C_id, currency.UserId, currency.Balance, currency.Currency, personal.FirstName, personal.LastName, currency.IsActive });
                int i = 0;
                foreach (var user in transDetails)
                {
                    i++;
                    //user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(s => s.C_id == userid);
                    bool isAdd = true;
                    bool isActive = (bool)user.IsActive;
                    if (!isActive && Convert.ToDouble(user.Balance) == 0)
                        isAdd = false;
                    if (isAdd)
                    {
                        manageUserList.Add(
                        new BalanceHistoryViewModel
                        {
                            UserId = user.UserId,
                            Id = user.C_id,
                            SrNo = i,
                            Name = user.FirstName + " " + user.LastName,
                            Balance = Convert.ToDouble(user.Balance),
                            Currency = user.Currency,
                            IsActive = (bool)user.IsActive
                        }
                        );
                    }
                }

            }
            else if (!string.IsNullOrEmpty(userSearch) && !string.IsNullOrEmpty(currencySearch))
            {
                int userId = Convert.ToInt32(userSearch);
                int currencyId = Convert.ToInt32(currencySearch);
                var transDetails = (from currency in dbContext.user_currency_details
                                    join
                                    personal in dbContext.user_personal_details on currency.UserId equals personal.C_id
                                    where currency.CurrencyId == currencyId && personal.C_id == userId && personal.C_id == userDetails.C_id && personal.IsActive == true
                                    orderby currency.UserId
                                    select new { currency.C_id, currency.UserId, currency.Balance, currency.Currency, personal.FirstName, personal.LastName, currency.IsActive });
                int i = 0;
                foreach (var user in transDetails)
                {
                    i++;
                    //user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(s => s.C_id == userid);
                    bool isAdd = true;
                    bool isActive = (bool)user.IsActive;
                    if (!isActive && Convert.ToDouble(user.Balance) == 0)
                        isAdd = false;
                    if (isAdd)
                    {
                        manageUserList.Add(
                        new BalanceHistoryViewModel
                        {
                            UserId = user.UserId,
                            Id = user.C_id,
                            SrNo = i,
                            Name = user.FirstName + " " + user.LastName,
                            Balance = Convert.ToDouble(user.Balance),
                            Currency = user.Currency,
                            IsActive = (bool)user.IsActive
                        }
                        );
                    }
                }
            }
            return manageUserList;
        }

        #endregion

        public List<BalanceHistoryViewModel> GetAllBalanceHistory(string userSearch, string currencySearch, user_personal_details userDetails)
        {

            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            List<BalanceHistoryViewModel> manageUserList = new List<BalanceHistoryViewModel>();

            if (userDetails.Role.Equals(Constant.ROLE_SUPER_ADMIN))
            {
                manageUserList = GetSuperAdminList(userSearch, currencySearch, userDetails);
            }

            else if (userDetails.Role.Equals(Constant.ROLE_ADMIN))
            {
                manageUserList = GetAdminList(userSearch, currencySearch, userDetails);
            }
            else if (userDetails.Role.Equals(Constant.ROLE_DISTRIBUTOR))
            {
                manageUserList = GetDistributorList(userSearch, currencySearch, userDetails);
            }
            else if (userDetails.Role.Equals(Constant.ROLE_MERCHANT))
            {
                manageUserList = GetMerchantList(userSearch, currencySearch, userDetails);
            }
            return manageUserList;

        }


    }
}