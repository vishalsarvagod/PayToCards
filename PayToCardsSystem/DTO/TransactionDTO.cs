using PayToCardsSystem.AppCode.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayToCardsSystem.DTO
{
    public class TransactionDTO
    {
        public int TransactionNo { get; set; }
        public double RunningBalance { get; set; }
        public CardPaymentResponse cardPaymentReposne { get; set; }
        public bool IsSuccess { get; set; }
        public string CardNumber { get; set; }

    }
}