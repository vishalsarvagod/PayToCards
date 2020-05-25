using Newtonsoft.Json;
using PayToCardsSystem.AppCode.CommonCode;
using PayToCardsSystem.AppCode.DAO;
using PayToCardsSystem.DTO;
using PayToCardsSystem.Models;
using PayToCardsSystem.Service;
using PayToCardsSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace PayToCardsSystem.Controllers
{
    public class TransactionController : Controller
    {
        vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
        CurrencyService currencyUser = new CurrencyService();
        UserService userService = new UserService();
        // GET: Transaction
        public ActionResult ManualTransaction()
        {
            if (Session["UserPersonalDetails"] != null)
            {
                ViewBag.Message = "";
                var userDetails = (user_personal_details)Session["UserPersonalDetails"];
                return View(InitializeForm(userDetails));
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        private TransactionModels InitializeForm(user_personal_details userDetails)
        {
            TransactionModels transModel = new TransactionModels();
            int userid = ((user_personal_details)(Session["UserPersonalDetails"])).C_id;
            user_currency_details userCurrency = dbContext.user_currency_details.FirstOrDefault(i => i.UserId == userid);
            // transModel.Currency = userCurrency.Currency;
            transModel.ExpireDate = DateTime.Now;
            transModel.CurrencyList = currencyUser.GetAllCurrency(userDetails);
            return transModel;
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ManualTransaction(TransactionModels transModel)
        {
            TransactionService transService = new TransactionService();
            var userDetails = (user_personal_details)Session["UserPersonalDetails"];

            try
            {
                int currencyId = Convert.ToInt32(transModel.Currency);
                user_currency_details userCur = dbContext.user_currency_details.FirstOrDefault(i => i.UserId == userDetails.C_id && i.CurrencyId == currencyId);

                if (userCur != null)
                {
                    transModel.Currency = userCur.Currency;
                    double transactionFee = 0;
                    user_fee_details userFeeDetail = dbContext.user_fee_details.FirstOrDefault(i => i.UserId == userDetails.C_id && i.CurrencyId == userCur.CurrencyId);
                    double percentFee = 0;
                    percentFee = transModel.TransactionAmount * userFeeDetail.PercentFee / 100;
                    if (userFeeDetail.FeeType.Equals("AND"))
                    {
                        transactionFee = percentFee + userFeeDetail.FlatFee;
                    }
                    else
                    {
                        if (percentFee > userFeeDetail.FlatFee)
                            transactionFee = percentFee;
                        else
                            transactionFee = userFeeDetail.FlatFee;
                    }
                    if (userCur.Balance >= (transModel.TransactionAmount + transactionFee))
                    {
                        int userid = userDetails.C_id;
                        transModel.UserId = userid;
                        //var selectedItem = dbContext.currencies.Where(item => item.C_id.Equals(CurId)).First();
                        //transModel.Currency = selectedItem.ShortCode;
                        TransactionDTO transDTO = transService.saveTranWithApi(transModel, transactionFee, userDetails, userCur.CurrencyId);
                        if (transDTO != null)
                        {
                            CardPaymentResponse cardResponse = transDTO.cardPaymentReposne;
                            if (cardResponse == null)
                            {
                                ViewBag.Message = "Error Occured";

                            }
                            else
                            {
                                if (transDTO.IsSuccess)
                                    ViewBag.Message = "Transaction ID : " + transDTO.TransactionNo + " Balance : " + transDTO.RunningBalance;
                                else
                                    ViewBag.Message = "Transaction ID:"+ transDTO.TransactionNo +" "+ cardResponse.result.description;
                            }
                            ModelState.Clear();
                            transModel = InitializeForm(userDetails);
                            return View(transModel);
                        }
                        else
                        {
                            transModel = InitializeForm(userDetails);
                            ViewBag.Message = "Unable to Connect Server";
                            return View(transModel);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "You are out of balance to do this transaction.";
                        transModel = InitializeForm(userDetails);
                        return View(transModel);
                    }
                }else
                {
                    ViewBag.Message = "Currency Not Found.";
                    transModel = InitializeForm(userDetails);
                    return View(transModel);
                }

            }
            catch (Exception e)
            {
                ViewBag.Message = "Error Occured During Transaction.";
                transModel = InitializeForm(userDetails);
                return View(transModel);
            }
        }

        public ActionResult GetBalance(string CurrencyId)
        {
            if (!string.IsNullOrEmpty(CurrencyId))
            {
                int curId = Convert.ToInt32(CurrencyId);
                try
                {
                    var userDetails = (user_personal_details)Session["UserPersonalDetails"];
                    //  int CurId = Convert.ToInt32(CurrencyId);
                    vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
                    var getBalance = from x in dbContext.user_currency_details
                                     where x.UserId == userDetails.C_id && x.CurrencyId == curId
                                     select new { x.Balance };
                    double balance = Convert.ToDouble(getBalance.FirstOrDefault().Balance);
                    balance = System.Math.Round(balance, 2);
                    return Json(balance);
                }
                catch (Exception ex)
                {
                    return Json(0);
                }
            }
            return Json(0);
        }
        public ActionResult ManageTransaction()
        {
            if (Session["UserPersonalDetails"] != null)
            {
                var userDetails = (user_personal_details)Session["UserPersonalDetails"];
                TransactionSearchModel transSearchModel = new TransactionSearchModel();
                DateTime fromDate = new DateTime(transSearchModel.FromDate.Year, transSearchModel.FromDate.Month, transSearchModel.FromDate.Day, 0, 0, 0);
                DateTime toDate = new DateTime(transSearchModel.ToDate.Year, transSearchModel.ToDate.Month, transSearchModel.ToDate.Day, 23, 59, 59);
                string userList = null;
                string currencyList = null;
                string cardNo = null;

                transSearchModel.ListTransactionViewModel = GetAllTransaction(fromDate, toDate, userList, currencyList, cardNo, transSearchModel.TransactionAmountFrom, transSearchModel.TransactionAmountTo, transSearchModel.Success, transSearchModel.Fail);
                // transSearchModel.FromDate = DateTime.Now;
                //transSearchModel.UserList = new List<SelectListItem>(){
                //    new SelectListItem {Text="Admin",Value="1"}};
                transSearchModel.UserList = userService.GetAllUserRoleWiseIncludeLogin(userDetails);
                transSearchModel.CurrencyList = currencyUser.GetAllCurrencyValueShortcode(userDetails);
                return View(transSearchModel);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        [SubmitButtonSelector(Name = "Search")]
        public ActionResult Search(TransactionSearchModel transactionSearch)
        {
            var userDetails = (user_personal_details)Session["UserPersonalDetails"];
            TransactionSearchModel transactionModel = new TransactionSearchModel();
            transactionModel.CurrencyList = currencyUser.GetAllCurrencyValueShortcode(userDetails);
            transactionModel.UserList = userService.GetAllUserRoleWiseIncludeLogin(userDetails);
            DateTime fromDate = new DateTime(transactionSearch.FromDate.Year, transactionSearch.FromDate.Month, transactionSearch.FromDate.Day, 0, 0, 0);
            DateTime toDate = new DateTime(transactionSearch.ToDate.Year, transactionSearch.ToDate.Month, transactionSearch.ToDate.Day, 23, 59, 59);
            transactionModel.ListTransactionViewModel = GetAllTransaction(fromDate, toDate, transactionSearch.UserValue, transactionSearch.CurrencyValue, transactionSearch.CardNumber, transactionSearch.TransactionAmountFrom, transactionSearch.TransactionAmountTo, transactionSearch.Success, transactionSearch.Fail);
            return View(transactionModel);
        }

        //To view employee details with generic list



        public List<TransactionViewModel> GetAllTransaction(DateTime fromDate, DateTime toDate,
            string userValue, string currencyValue,
            string cardNo, double transAmountFrom,
            double transAmountTo, bool success, bool fail)
        {
            user_personal_details userDetail = (user_personal_details)(Session["UserPersonalDetails"]);
            int userid = userDetail.C_id;
            List<TransactionViewModel> manageUserList = new List<TransactionViewModel>();

            var transDetails1 = (from user in dbContext.user_personal_details
                                 join transaction in dbContext.transaction_card
                                 on user.C_id equals transaction.UserId
                                 join response in dbContext.transaction_card_response
                                 on transaction.C_id equals response.TransactionId
                                 where transaction.TransactionDate >= fromDate
                                 && transaction.TransactionDate <= toDate
                                 orderby transaction.TransactionDate descending

                                 select new
                                 {
                                     transaction.C_id,
                                     transaction.TransactionNo,
                                     UserId = transaction.UserId,
                                     user.AdminId,
                                     user.DistributorID,
                                     user.FirstName,
                                     user.LastName,
                                     transaction.CardNumber,
                                     transaction.TransactionAmount,
                                     transaction.TransactionFee,
                                     transaction.TransactionDate,
                                     transaction.Currency,
                                     transaction.Status,
                                     transaction.Message1,
                                     transaction.Message2,
                                     response.ResponseMessage,
                                 });
            if (userDetail.Role.Equals(Constant.ROLE_ADMIN))
            {

                transDetails1 = transDetails1.Where(c => c.AdminId == userDetail.C_id);

            }
            else if (userDetail.Role.Equals(Constant.ROLE_DISTRIBUTOR))
            {

                transDetails1 = transDetails1.Where(c => c.UserId == userDetail.C_id || c.DistributorID == userDetail.C_id);

            }
            else if (userDetail.Role.Equals(Constant.ROLE_MERCHANT))
            {
                transDetails1 = transDetails1.Where(c => c.UserId == userDetail.C_id);

            }
            if (!string.IsNullOrEmpty(userValue))
            {
                int userId = Convert.ToInt32(userValue);
                transDetails1 = transDetails1.Where(c => c.UserId == userId);
            }

            if (!string.IsNullOrEmpty(currencyValue))
            {

                transDetails1 = transDetails1.Where(c => c.Currency.Equals(currencyValue));
            }
            if (!string.IsNullOrEmpty(cardNo))
            {

                transDetails1 = transDetails1.Where(c => c.CardNumber.Contains(cardNo));
            }
            if (transAmountFrom > 0)
            {

                transDetails1 = transDetails1.Where(c => c.TransactionAmount >= transAmountFrom);
            }


            if (transAmountTo > 0)
            {

                transDetails1 = transDetails1.Where(c => c.TransactionAmount <= transAmountTo);
            }

            if (success)
            {

                transDetails1 = transDetails1.Where(c => c.Status.Equals("Success"));
            }

            if (fail)
            {

                transDetails1 = transDetails1.Where(c => c.Status.Equals("Fail"));
            }

            foreach (var user in transDetails1)
            {

                manageUserList.Add(
                new TransactionViewModel
                {
                    Id = user.C_id,
                    TransactionNo = user.TransactionNo,
                    CardNumber = user.CardNumber,
                    TransactionAmount = user.TransactionAmount,
                    TransactionFee = user.TransactionFee,
                    TransactionDate = user.TransactionDate,
                    Status = (user.Status == "Success") ? "S - " + user.ResponseMessage : "F - " + user.ResponseMessage,
                    UserName = user.FirstName + " " + user.LastName,
                    Currency = user.Currency,
                    Remarks = user.Message1 + " " + user.Message2,
                    Total = user.TransactionAmount + user.TransactionFee,
                }
                );
            }

            // }

            return manageUserList;

        }


    }
}