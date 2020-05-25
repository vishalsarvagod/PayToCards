using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayToCardsSystem.DTO
{
    public class AccountLedgerDTO
    {
        public int TransId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy HH:mm}")]
        public DateTime TransactionDate { get; set; }
        public string Name { get; set; }
        public string  TransactionType { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public double RunningBalance { get; set; }

    }
}