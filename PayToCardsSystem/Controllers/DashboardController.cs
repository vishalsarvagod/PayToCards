using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayToCardsSystem.Models;
using PayToCardsSystem.AppCode.CommonCode;

namespace PayToCardsSystem.Controllers
{
    public class DashboardController : Controller
    {
       
        
        public ActionResult Dashboard()
        {
            try
            {
                if (Session["UserPersonalDetails"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                DashboardModel dm = new DashboardModel();
                GetTransactionCount(dm);
                dm.ListBalanceHistoryViewModel = GetAllBalanceHistory();
                dm.ListTransactionViewModel = GetAllTransaction();
                return View(dm);
            }catch(Exception e)
            {
                return View("Error");
            }
            
        }
        
        #region Transaction Count
        private void GetTransactionCount(DashboardModel dm)
        {
            user_personal_details userDetail = (user_personal_details)(Session["UserPersonalDetails"]);
            if (userDetail.Role.Equals(Constant.ROLE_SUPER_ADMIN))
                GetTransCountSuper(dm);
            else if (userDetail.Role.Equals(Constant.ROLE_ADMIN))
                GetTransCountAdmin(dm);
            else if (userDetail.Role.Equals(Constant.ROLE_DISTRIBUTOR))
                GetTransCountDistributor(dm);
            else if (userDetail.Role.Equals(Constant.ROLE_MERCHANT))
                GetTransCountMerchant(dm);
            dm.UserId = userDetail.C_id;
        }
        private void GetTransCountSuper(DashboardModel dm)
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();

            DateTime now = DateTime.Now;
            DateTime sDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            DateTime eDate = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            string success = "Success";
            string fail = "Fail";

            int totalTrans = dbContext.transaction_card.Where(v => v.TransactionDate > sDate
            && v.TransactionDate < eDate).Count();
            int successTrans = dbContext.transaction_card.Where(v => v.Status.ToLower().Contains(success) && v.TransactionDate > sDate
            && v.TransactionDate < eDate).Count();
            int failedTrans = dbContext.transaction_card.Where(v => v.Status.ToLower().Contains(fail) && v.TransactionDate > sDate
            && v.TransactionDate < eDate).Count();
            dm.Total = totalTrans;
            dm.Success = successTrans;
            dm.Failed = failedTrans;
        }
        private void GetTransCountAdmin(DashboardModel dm)
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            user_personal_details userDetail = (user_personal_details)(Session["UserPersonalDetails"]);
            DateTime now = DateTime.Now;
            DateTime sDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            DateTime eDate = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            string success = "Success";
            string fail = "Fail";

            int totalTrans = (from user in dbContext.user_personal_details
                              from transaction in dbContext.transaction_card
                              where user.C_id == transaction.UserId && user.AdminId == userDetail.C_id &&
                              transaction.TransactionDate > sDate && transaction.TransactionDate < eDate
                              select new { transaction.C_id }).Count();

            int successTrans = (from user in dbContext.user_personal_details
                                from transaction in dbContext.transaction_card
                                where user.C_id == transaction.UserId && user.AdminId == userDetail.C_id &&
                                transaction.TransactionDate > sDate && transaction.TransactionDate < eDate &&
                                transaction.Status.ToLower().Contains(success)
                                select new { transaction.C_id }).Count();

            int failedTrans = (from user in dbContext.user_personal_details
                               from transaction in dbContext.transaction_card
                               where user.C_id == transaction.UserId && user.AdminId == userDetail.C_id &&
                               transaction.TransactionDate > sDate && transaction.TransactionDate < eDate &&
                               transaction.Status.ToLower().Contains(fail)
                               select new { transaction.C_id }).Count();

            dm.Total = totalTrans;
            dm.Success = successTrans;
            dm.Failed = failedTrans;
        }
        private void GetTransCountDistributor(DashboardModel dm)
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            user_personal_details userDetail = (user_personal_details)(Session["UserPersonalDetails"]);
            DateTime now = DateTime.Now;
            DateTime sDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            DateTime eDate = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            string success = "Success";
            string fail = "Fail";

            int totalTrans = (from user in dbContext.user_personal_details
                              join transaction in dbContext.transaction_card
                              on user.C_id equals transaction.UserId
                              where (userDetail.C_id == transaction.UserId || user.DistributorID == userDetail.C_id) &&
                              transaction.TransactionDate > sDate && transaction.TransactionDate < eDate
                              select new { transaction.C_id }).Count();

            int successTrans = (from user in dbContext.user_personal_details
                                join transaction in dbContext.transaction_card
                                on user.C_id equals transaction.UserId
                                where (userDetail.C_id == transaction.UserId || user.DistributorID == userDetail.C_id) &&
                                transaction.TransactionDate > sDate && transaction.TransactionDate < eDate &&
                                transaction.Status.ToLower().Contains(success)
                                select new { transaction.C_id }).Count();

            int failedTrans = (from user in dbContext.user_personal_details
                               join transaction in dbContext.transaction_card
                               on user.C_id equals transaction.UserId
                               where (userDetail.C_id == transaction.UserId || user.DistributorID == userDetail.C_id) &&
                               transaction.TransactionDate > sDate && transaction.TransactionDate < eDate &&
                               transaction.Status.ToLower().Contains(fail)
                               select new { transaction.C_id }).Count();

            dm.Total = totalTrans;
            dm.Success = successTrans;
            dm.Failed = failedTrans;
        }
        private void GetTransCountMerchant(DashboardModel dm)
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            user_personal_details userDetail = (user_personal_details)(Session["UserPersonalDetails"]);
            DateTime now = DateTime.Now;
            DateTime sDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            DateTime eDate = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            string success = "Success";
            string fail = "Fail";

            int totalTrans = (from user in dbContext.user_personal_details
                              from transaction in dbContext.transaction_card
                              where user.C_id == transaction.UserId 
                              && user.C_id == userDetail.C_id &&
                              transaction.TransactionDate > sDate && transaction.TransactionDate < eDate
                              select new { transaction.C_id }).Count();

            int successTrans = (from user in dbContext.user_personal_details
                                from transaction in dbContext.transaction_card
                                where user.C_id == transaction.UserId && user.C_id == userDetail.C_id &&
                                transaction.TransactionDate > sDate && transaction.TransactionDate < eDate &&
                                transaction.Status.ToLower().Contains(success)
                                select new { transaction.C_id }).Count();

            int failedTrans = (from user in dbContext.user_personal_details
                               from transaction in dbContext.transaction_card
                               where user.C_id == transaction.UserId && user.C_id == userDetail.C_id &&
                               transaction.TransactionDate > sDate && transaction.TransactionDate < eDate &&
                               transaction.Status.ToLower().Contains(fail)
                               select new { transaction.C_id }).Count();

            dm.Total = totalTrans;
            dm.Success = successTrans;
            dm.Failed = failedTrans;
        }
        #endregion

        #region Show All Balance Histroy
        
        private List<BalanceHistoryViewModel> GetAllBalanceHistory()
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            List<BalanceHistoryViewModel> ListBalanceHistoryViewModel = new List<BalanceHistoryViewModel>();

            user_personal_details userDetails = (user_personal_details)(Session["UserPersonalDetails"]);

            var transDetails = (from currency in dbContext.user_currency_details
                                join
                                personal in dbContext.user_personal_details on currency.UserId equals personal.C_id
                                where personal.C_id == userDetails.C_id
                                orderby currency.UserId
                                select new { currency.C_id, currency.UserId, currency.Balance, currency.Currency, personal.FirstName, personal.LastName ,currency.IsActive});
            int i = 0;
            foreach (var user in transDetails)
            {
                i++;
                ListBalanceHistoryViewModel.Add(
                new BalanceHistoryViewModel
                {
                    UserId = user.UserId,
                    Id = user.C_id,
                    SrNo = i,
                    Name = user.FirstName + " " + user.LastName,
                    Balance = Convert.ToDouble(user.Balance),
                    Currency = user.Currency,
                    IsActive =(bool)user.IsActive
                }
                );
            }

            return ListBalanceHistoryViewModel;

        }

        #endregion

        #region All Todays Transaction
        private List<TransactionViewModel> GetAllTransaction()
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            user_personal_details userDetail = (user_personal_details)(Session["UserPersonalDetails"]);
            int userid = userDetail.C_id;
            List<TransactionViewModel> ListTransactionViewModel = new List<TransactionViewModel>();
            DateTime now = DateTime.Now;
            DateTime sDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            DateTime eDate = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            if (userDetail.Role.Equals(Constant.ROLE_SUPER_ADMIN))
            {
                var transDetails = (from user in dbContext.user_personal_details
                                    from transaction in dbContext.transaction_card
                                    where user.C_id == transaction.UserId && transaction.TransactionDate > sDate && transaction.TransactionDate < eDate
                                    orderby transaction.TransactionDate descending
                                    select new
                                    {
                                        transaction.C_id,
                                        user.FirstName,
                                        user.LastName,
                                        transaction.CardNumber,
                                        transaction.TransactionAmount,
                                        transaction.TransactionFee,
                                        transaction.TransactionDate,
                                        transaction.Currency,
                                        transaction.Status,
                                        transaction.Message1,
                                        transaction.Message2
                                    });
                foreach (var user in transDetails)
                {
                    ListTransactionViewModel.Add(
                    new TransactionViewModel
                    {
                        Id = user.C_id,
                        CardNumber = user.CardNumber,
                        TransactionAmount = user.TransactionAmount,
                        TransactionFee = user.TransactionFee,
                        TransactionDate = user.TransactionDate,
                        Status = user.Status,
                        UserName = user.FirstName + " " + user.LastName,
                        Currency = user.Currency,
                        Remarks = user.Message1 + " " + user.Message2
                    }
                    );
                }
            }
            else if (userDetail.Role.Equals(Constant.ROLE_ADMIN))
            {
                var transDetails = (from user in dbContext.user_personal_details
                                    from transaction in dbContext.transaction_card
                                    where user.C_id == transaction.UserId && user.AdminId == userDetail.C_id && transaction.TransactionDate > sDate && transaction.TransactionDate < eDate
                                    orderby transaction.TransactionDate descending
                                    select new
                                    {
                                        transaction.C_id,
                                        user.FirstName,
                                        user.LastName,
                                        transaction.CardNumber,
                                        transaction.TransactionAmount,
                                        transaction.TransactionFee,
                                        transaction.TransactionDate,
                                        transaction.Currency,
                                        transaction.Status,
                                        transaction.Message1,
                                        transaction.Message2
                                    });

                foreach (var user in transDetails)
                {

                    ListTransactionViewModel.Add(
                    new TransactionViewModel
                    {
                        Id = user.C_id,
                        CardNumber = user.CardNumber,
                        TransactionAmount = user.TransactionAmount,
                        TransactionFee = user.TransactionFee,
                        TransactionDate = user.TransactionDate,
                        Status = user.Status,
                        UserName = user.FirstName + " " + user.LastName,
                        Currency = user.Currency,
                        Remarks = user.Message1 + " " + user.Message2
                    }
                    );
                }
            }
            else if (userDetail.Role.Equals(Constant.ROLE_DISTRIBUTOR))
            {
                var transDetails = (from user in dbContext.user_personal_details
                                    join transaction in dbContext.transaction_card
                                    on user.C_id equals transaction.UserId
                                    where
                                    (userDetail.C_id == transaction.UserId || user.DistributorID == userDetail.C_id)
                                    && transaction.TransactionDate > sDate && transaction.TransactionDate < eDate
                                    orderby transaction.TransactionDate descending
                                    select new
                                    {
                                        transaction.C_id,
                                        user.FirstName,
                                        user.LastName,
                                        transaction.CardNumber,
                                        transaction.TransactionAmount,
                                        transaction.TransactionFee,
                                        transaction.TransactionDate,
                                        transaction.Currency,
                                        transaction.Status,
                                        transaction.Message1,
                                        transaction.Message2
                                    });

                foreach (var user in transDetails)
                {

                    ListTransactionViewModel.Add(
                    new TransactionViewModel
                    {
                        Id = user.C_id,

                        CardNumber = user.CardNumber,
                        TransactionAmount = user.TransactionAmount,
                        TransactionFee = user.TransactionFee,
                        TransactionDate = user.TransactionDate,
                        Status = user.Status,
                        UserName = user.FirstName + " " + user.LastName,
                        Currency = user.Currency,
                        Remarks = user.Message1 + " " + user.Message2
                    }
                    );
                }
            }
            else if (userDetail.Role.Equals(Constant.ROLE_MERCHANT))
            {
                var transDetails = (from user in dbContext.user_personal_details
                                    from transaction in dbContext.transaction_card
                                    where user.C_id == transaction.UserId
                                    && user.C_id == userDetail.C_id && transaction.TransactionDate > sDate && transaction.TransactionDate < eDate
                                    orderby transaction.TransactionDate descending
                                    select new
                                    {
                                        transaction.C_id,
                                        user.FirstName,
                                        user.LastName,
                                        transaction.CardNumber,
                                        transaction.TransactionAmount,
                                        transaction.TransactionFee,
                                        transaction.TransactionDate,
                                        transaction.Currency,
                                        transaction.Status,
                                        transaction.Message1,
                                        transaction.Message2
                                    });

                foreach (var user in transDetails)
                {

                    ListTransactionViewModel.Add(
                    new TransactionViewModel
                    {
                        Id = user.C_id,

                        CardNumber = user.CardNumber,
                        TransactionAmount = user.TransactionAmount,
                        TransactionFee = user.TransactionFee,
                        TransactionDate = user.TransactionDate,
                        Status = user.Status,
                        UserName = user.FirstName + " " + user.LastName,
                        Currency = user.Currency,
                        Remarks = user.Message1 + " " + user.Message2
                    }
                    );
                }
            }
            
            return ListTransactionViewModel;

        }
        #endregion
    }
}