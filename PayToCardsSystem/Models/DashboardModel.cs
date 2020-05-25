using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
 

namespace PayToCardsSystem.Models
{
    public class DashboardModel
    {
        public int UserId { get; set; }
        public int Total { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
        public List<BalanceHistoryViewModel> ListBalanceHistoryViewModel { get; set; }
        public List<TransactionViewModel> ListTransactionViewModel { get; set; }        
    }
}