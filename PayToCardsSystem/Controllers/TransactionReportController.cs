using PayToCardsSystem.AppCode.CommonCode;
using PayToCardsSystem.DTO;
using PayToCardsSystem.Models;
using PayToCardsSystem.Service;
using PayToCardsSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayToCardsSystem.Controllers
{
    public class TransactionReportController : Controller
    {
        UserService userService = new UserService();

        public ActionResult Index()
        {
            if (Session["UserPersonalDetails"] != null)
            {
                var userDetails = (user_personal_details)Session["UserPersonalDetails"];
                AccountLedgerReportViewModel accountModel = InitializeAccountLedgerModel(userDetails);

                return View(accountModel);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        private AccountLedgerReportViewModel InitializeAccountLedgerModel(user_personal_details userDetails)
        {
            AccountLedgerReportViewModel accountLedgerReport = new AccountLedgerReportViewModel();
            accountLedgerReport.CurrencyList = new List<SelectListItem>();
            accountLedgerReport.UserNameList = userService.GetAllUserRoleWiseIncludeLogin(userDetails);
            accountLedgerReport.listAccountLedger = new List<AccountLedgerDTO>();
            return accountLedgerReport;
        }
        public ActionResult GetCurrencyByUser(string UserId)
        {

            List<SelectListItem> CurrencyList = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(UserId))
            {
                int Id = Convert.ToInt32(UserId);
                //userId = Id;
                vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
                var getCurrency = from x in dbContext.user_currency_details
                                  where x.UserId == Id
                                  select new { x.Currency };
                int i = 0;

                foreach (var item in getCurrency)
                {
                    i++;
                    CurrencyList.Add(new SelectListItem { Text = item.Currency, Value = item.Currency});
                }
            }
            return Json(CurrencyList);
        }
        [HttpPost]
        public ActionResult Index(AccountLedgerReportViewModel accountModel)
        {
            vmpaytocardsEntities _context = new vmpaytocardsEntities();
            IEnumerable<AccountLedgerDTO> listAccountLedger = new List<AccountLedgerDTO>();
            string query = "Select * from (SELECT transaction_card_account._id as 'TransId', transaction_card.TransactionDate, " +
                " user_personal_details.FirstName as 'Name', transaction_card_account.TransactionType, transaction_card.TransactionAmount as 'Amount', " +
                " transaction_card.currency as 'Currency', transaction_card_account.Debit, transaction_card_account.Credit, " +
                " transaction_card_account.RunningBalance FROM transaction_card_account " +
                " INNER JOIN transaction_card ON transaction_card_account.TransactionId = transaction_card._id  " +
                " AND transaction_card_account.TransactionType = 'Transaction' " +
                " INNER JOIN user_personal_details ON user_personal_details._id = transaction_card.UserId " +
                " WHERE transaction_card_account.UserId = " + accountModel.UserId +
                " UNION " +
                " SELECT transaction_card_account._id as 'TransId', transaction_topup.TopupDate as 'TransactionDate', user_personal_details.FirstName as 'Name', " +
                " transaction_card_account.TransactionType, transaction_topup.Amount as 'Amount', currency.ShortCode as 'Currency', " +
                " transaction_card_account.Debit, transaction_card_account.Credit, transaction_card_account.RunningBalance " +
                " FROM transaction_card_account " +
                " INNER JOIN transaction_topup ON transaction_card_account.TransactionId = transaction_topup._id AND transaction_card_account.TransactionType = 'Topup' " +
                " INNER JOIN user_personal_details ON user_personal_details._id = transaction_topup.TopUpDoneBy " +
                " INNER JOIN currency ON currency._id = transaction_topup.Currency " +
                " WHERE transaction_card_account.UserId =  " + accountModel.UserId + ") as Tbl " +
                " order by Tbl.TransactionDate desc; ";

            var ledgerAccountTransList = _context.ExecuteStoreQuery<AccountLedgerDTO>(query).ToList();

            if (Session["UserPersonalDetails"] != null)
            {
                var userDetails = (user_personal_details)Session["UserPersonalDetails"];
                //int CurrVal = Int32.Parse(accountModel.CurrencyValue);
                if (accountModel.CurrencyValue!=null && !accountModel.CurrencyValue.Equals("0"))
                {
                    //string cur = _context.currencies.FirstOrDefault(x => x.C_id == CurrVal).ShortCode;
                    accountModel.listAccountLedger = ledgerAccountTransList.Where(x => x.Currency.Equals(accountModel.CurrencyValue));
                }
                else
                    accountModel.listAccountLedger = ledgerAccountTransList;
                accountModel.UserNameList = userService.GetAllUserRoleWiseIncludeLogin(userDetails);
                accountModel.CurrencyList = new List<SelectListItem>();
                return View(accountModel);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }



        }

        public ActionResult ViewDetails(int? id)
        {
            try
            {
                if (Session["UserPersonalDetails"] != null)
                {
                    var userDetails = (user_personal_details)Session["UserPersonalDetails"];
                    vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
                    transaction_card_account transactionCardAccount = dbContext.transaction_card_account.FirstOrDefault(i => i.C_Id == id);

                    LadgerDetailsModel ledgerDetails = new LadgerDetailsModel();
                    if (transactionCardAccount.TransactionType == "Transaction")
                    {
                        transaction_card transactionCard = dbContext.transaction_card.FirstOrDefault(i => i.C_id == transactionCardAccount.TransactionId);
                        user_personal_details userPersonalDetails = dbContext.user_personal_details.FirstOrDefault(i => i.C_id == transactionCard.UserId);
                        ledgerDetails.TransactionNo = transactionCard.TransactionNo.ToString();
                        ledgerDetails.TransactionDate = transactionCard.TransactionDate;
                        ledgerDetails.TransactionAmount = Math.Round(transactionCard.TransactionAmount, 2);
                        ledgerDetails.Currency = transactionCard.Currency;
                        ledgerDetails.TransactionType = transactionCardAccount.TransactionType;
                        ledgerDetails.CardNumber = transactionCard.CardNumber;
                        ledgerDetails.ExpireDate = transactionCard.ExpireDate;
                        ledgerDetails.CardHolderName = transactionCard.CardHolderName;
                        ledgerDetails.ContactNo = transactionCard.ContactNo;
                        ledgerDetails.Remarks = transactionCard.Remarks;
                        ledgerDetails.Status = transactionCard.Status;
                        ledgerDetails.User = userPersonalDetails.FirstName + " " + userPersonalDetails.LastName;
                        ledgerDetails.Role = userPersonalDetails.Role;
                        ledgerDetails.Credit = Math.Round(transactionCardAccount.Credit, 2).ToString();
                        ledgerDetails.Debit = Math.Round(transactionCardAccount.Debit, 2).ToString();
                        ledgerDetails.RunningBalance = Math.Round(transactionCardAccount.RunningBalance, 2).ToString();
                        ledgerDetails.CheckType = true;
                        user_personal_details userTransDoneFor = dbContext.user_personal_details.FirstOrDefault(i => i.C_id == transactionCardAccount.UserId);
                        ledgerDetails.TransactionDoneFor = userTransDoneFor.FirstName + " " + userTransDoneFor.LastName;

                    }
                    else
                    {
                        transaction_topup transactionTopup = dbContext.transaction_topup.FirstOrDefault(i => i.C_id == transactionCardAccount.TransactionId);
                        user_personal_details userPersonalDetails = dbContext.user_personal_details.FirstOrDefault(i => i.C_id == transactionTopup.TopUpDoneBy);
                        user_personal_details userPersonal = dbContext.user_personal_details.FirstOrDefault(i => i.C_id == transactionTopup.UserId);
                        ledgerDetails.TopupDoneBy = userPersonalDetails.FirstName + " " + userPersonalDetails.LastName;
                        ledgerDetails.TopupDoneTo = userPersonal.FirstName + " " + userPersonal.LastName;
                        ledgerDetails.TransactionAmount = Math.Round(transactionTopup.Amount, 2);
                        ledgerDetails.TransactionDate = transactionTopup.TopupDate;
                        ledgerDetails.Remarks = transactionTopup.Remarks;
                        int currencyId = Convert.ToInt32(transactionTopup.Currency);
                        currency currencyShortCode = dbContext.currencies.FirstOrDefault(i => i.C_id == currencyId);
                        ledgerDetails.Currency = currencyShortCode.ShortCode;
                        ledgerDetails.Credit = Math.Round(transactionCardAccount.Credit, 2).ToString();
                        ledgerDetails.Debit = Math.Round(transactionCardAccount.Debit, 2).ToString();
                        ledgerDetails.RunningBalance = Math.Round(transactionCardAccount.RunningBalance, 2).ToString();
                        ledgerDetails.TransactionType = transactionCardAccount.TransactionType;
                        ledgerDetails.CheckType = false;
                        user_personal_details userTransDoneFor = dbContext.user_personal_details.FirstOrDefault(i => i.C_id == transactionCardAccount.UserId);
                        ledgerDetails.TransactionDoneFor = userTransDoneFor.FirstName + " " + userTransDoneFor.LastName;
                    }

                    return View(ledgerDetails);

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

    }
}