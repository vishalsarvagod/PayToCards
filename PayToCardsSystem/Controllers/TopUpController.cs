using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayToCardsSystem.Models;
using PayToCardsSystem.AppCode.CommonCode;
using PayToCardsSystem.Service;

namespace PayToCardsSystem.Controllers
{
    public class TopUpController : Controller
    {
        static int userId = 0;
        CurrencyService currencyUser = new CurrencyService();
        UserService userService = new UserService();
        // GET: TopUp
        public ActionResult Index()
        {
            if (Session["UserPersonalDetails"] != null)
            {
                userId = 0;
                var userDetails = (user_personal_details)Session["UserPersonalDetails"];
                TopUpModels topup = InitializeTopUp(userDetails);
                return View(topup);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        private TopUpModels InitializeTopUp(user_personal_details userDetails)
        {
            TopUpModels topUp = new TopUpModels();
            topUp.CurrencyList = currencyUser.GetAllCurrency(userDetails);
            topUp.UserNameList = userService.GetAllUserRoleWise(userDetails);
            return topUp;
        }
        public ActionResult GetCurrencyByUser(string UserId)
        {
            List<SelectListItem> CurrencyList = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(UserId))
            {
                int Id = Convert.ToInt32(UserId);
                userId = Id;
                vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
                var getCurrency = from x in dbContext.user_currency_details
                                  where x.UserId == Id && x.IsActive == true
                                  select new { x.CurrencyId, x.Currency };
                int i = 0;
                
                foreach (var item in getCurrency)
                {
                    i++;
                    CurrencyList.Add(new SelectListItem { Text = item.Currency, Value = item.CurrencyId.ToString() });
                }
            }
            else
            {
                var userDetails = (user_personal_details)Session["UserPersonalDetails"];
                ModelState.Clear();
                TopUpModels topup = new TopUpModels();
                topup = InitializeTopUp(userDetails);
                return View(topup);
            }           
            return Json(CurrencyList);
        }
        public ActionResult GetBalance(string CurrencyId)
        {
            try
            {
                int CurId = Convert.ToInt32(CurrencyId);
                vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
                var getBalance = from x in dbContext.user_currency_details
                                 where x.UserId == userId && x.CurrencyId == CurId
                                 select new { x.Balance };
                double balance = Convert.ToDouble(getBalance.FirstOrDefault().Balance);
                return Json(balance);
            }
            catch (Exception ex)
            {
                return Json(0);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]        
        public ActionResult Index(TopUpModels topUpModel)
        {                    
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            int selectedUserId = Convert.ToInt32(topUpModel.UserValue);
            int currencyID = Convert.ToInt32(topUpModel.CurrencyValue);
            var userDetails = (user_personal_details)Session["UserPersonalDetails"];
            int loginUserId = Convert.ToInt32(userDetails.C_id);
            user_currency_details userCurrencyDetail_SelectedUser = dbContext.user_currency_details.FirstOrDefault(i => i.UserId == selectedUserId && i.CurrencyId == currencyID);
            user_currency_details userCurrencyDetails_LoginUser = dbContext.user_currency_details.FirstOrDefault(i => i.UserId == loginUserId && i.CurrencyId == currencyID);
            string loginUserPerviousBalance = userCurrencyDetails_LoginUser.Balance.ToString();
          
            if (!userDetails.Role.ToString().Equals(Constant.ROLE_SUPER_ADMIN))
            {
                if (userCurrencyDetails_LoginUser.Balance >= topUpModel.TopupAmount)
                {
                    transaction_topup topUp = new transaction_topup();
                    {
                        topUp.TopUpDoneBy = loginUserId;
                        topUp.UserId = Convert.ToInt32(topUpModel.UserValue);
                        topUp.Amount = topUpModel.TopupAmount;
                        topUp.BalanceBefore = (double)userCurrencyDetail_SelectedUser.Balance;
                        topUp.BalanceAfter = topUp.BalanceBefore + topUpModel.TopupAmount;
                        topUp.TopupDate = DateTime.Now;
                        topUp.Remarks = topUpModel.Remarks;
                        topUp.Currency = topUpModel.CurrencyValue;
                        dbContext.transaction_topup.AddObject(topUp);
                        dbContext.SaveChanges();
                    }
                    transaction_card_account trans_card_Credit = new transaction_card_account();
                    {
                        userCurrencyDetail_SelectedUser.Balance = userCurrencyDetail_SelectedUser.Balance + topUpModel.TopupAmount;
                        trans_card_Credit.TransactionId = topUp.C_id;
                        trans_card_Credit.Credit = topUpModel.TopupAmount;
                        trans_card_Credit.Debit = 0;
                        trans_card_Credit.UserId = Convert.ToInt32(topUpModel.UserValue);
                        trans_card_Credit.RunningBalance = (double)userCurrencyDetail_SelectedUser.Balance;
                        trans_card_Credit.TransactionType = Constant.TransactionType.TopUp.ToString();
                        dbContext.transaction_card_account.AddObject(trans_card_Credit);
                        dbContext.SaveChanges();
                    }
                    transaction_card_account trans_card_Debit = new transaction_card_account();
                    {
                        userCurrencyDetails_LoginUser.Balance = userCurrencyDetails_LoginUser.Balance - topUpModel.TopupAmount;
                        trans_card_Debit.TransactionId = topUp.C_id;
                        trans_card_Debit.Credit = 0;
                        trans_card_Debit.Debit = topUpModel.TopupAmount;
                        trans_card_Debit.UserId = loginUserId;
                        trans_card_Debit.RunningBalance = (double)userCurrencyDetails_LoginUser.Balance;
                        trans_card_Debit.TransactionType = Constant.TransactionType.TopUp.ToString();
                        dbContext.transaction_card_account.AddObject(trans_card_Debit);
                        dbContext.SaveChanges();
                    }
                    ViewBag.Message = "Topup Success";
                    ModelState.Clear();
                    topUpModel = InitializeTopUp(userDetails);
                    topUpModel.UserBalance = topUp.BalanceAfter.ToString();
                    topUpModel.UserPerviousBalance = topUp.BalanceBefore.ToString();
                    topUpModel.LoginUserPerviousBalance = loginUserPerviousBalance;
                    topUpModel.LoginUserBalance = userCurrencyDetails_LoginUser.Balance.ToString();
                    topUpModel.Currency = userCurrencyDetails_LoginUser.Currency;
                    user_personal_details userPersonalDetails = dbContext.user_personal_details.FirstOrDefault(i => i.C_id == selectedUserId);
                    topUpModel.SelectUserName = userPersonalDetails.FirstName + " " + userPersonalDetails.LastName;
                    topUpModel.IsReport = true;
                    
                    return View(topUpModel);
                }
                else
                {
                    ViewBag.Message = "Balance Is Not Enough";
                    ModelState.Clear();
                    topUpModel = InitializeTopUp(userDetails);
                    return View(topUpModel);
                }
            }else
            {
                transaction_topup topUp = new transaction_topup();
                {
                    topUp.TopUpDoneBy = loginUserId;
                    topUp.UserId = Convert.ToInt32(topUpModel.UserValue);
                    topUp.Amount = topUpModel.TopupAmount;
                    topUp.BalanceBefore = (double)userCurrencyDetail_SelectedUser.Balance;
                    topUp.BalanceAfter = topUp.BalanceBefore + topUpModel.TopupAmount;
                    topUp.TopupDate = DateTime.Now;
                    topUp.Remarks = topUpModel.Remarks;
                    topUp.Currency = topUpModel.CurrencyValue;
                    dbContext.transaction_topup.AddObject(topUp);
                    dbContext.SaveChanges();
                }
                transaction_card_account trans_card_Credit = new transaction_card_account();
                {
                    userCurrencyDetail_SelectedUser.Balance = userCurrencyDetail_SelectedUser.Balance + topUpModel.TopupAmount;
                    trans_card_Credit.TransactionId = topUp.C_id;
                    trans_card_Credit.Credit = topUpModel.TopupAmount;
                    trans_card_Credit.Debit = 0;
                    trans_card_Credit.UserId = Convert.ToInt32(topUpModel.UserValue);
                    trans_card_Credit.RunningBalance = (double)userCurrencyDetail_SelectedUser.Balance;
                    trans_card_Credit.TransactionType = Constant.TransactionType.TopUp.ToString();
                    dbContext.transaction_card_account.AddObject(trans_card_Credit);
                    dbContext.SaveChanges();
                }
                ViewBag.Message = "Topup Success";
                ModelState.Clear();
                topUpModel = InitializeTopUp(userDetails);
                topUpModel.UserBalance = topUp.BalanceAfter.ToString();
                topUpModel.UserPerviousBalance = topUp.BalanceBefore.ToString();
                topUpModel.LoginUserPerviousBalance = loginUserPerviousBalance;
                topUpModel.LoginUserBalance = userCurrencyDetails_LoginUser.Balance.ToString();
                topUpModel.Currency = userCurrencyDetails_LoginUser.Currency;
                user_personal_details userPersonalDetails = dbContext.user_personal_details.FirstOrDefault(i => i.C_id == selectedUserId);
                topUpModel.SelectUserName = userPersonalDetails.FirstName + " " + userPersonalDetails.LastName;
                topUpModel.IsReport = true;
                return View(topUpModel);
            }
        }        
    }
        
}