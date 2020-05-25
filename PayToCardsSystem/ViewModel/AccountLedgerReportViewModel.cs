using PayToCardsSystem.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayToCardsSystem.ViewModel
{
    public class AccountLedgerReportViewModel
    {
        public IEnumerable<AccountLedgerDTO> listAccountLedger{ get; set; }
  //      public string UserValue { get; set; }
        public List<SelectListItem> UserNameList { get; set; }
        public string CurrencyValue { get; set; }
        public List<SelectListItem> CurrencyList { get; set; }
        public int UserId { get; set; }

    }
}