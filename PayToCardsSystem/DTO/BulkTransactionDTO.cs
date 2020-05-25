using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayToCardsSystem.DTO
{
    public class BulkTransactionDTO
    {
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string Message1 { get; set; }
        public string Message2 { get; set; }
        public string HolderName { get; set; }
        public string ContactNo { get; set; }
        public string Error { get; set; }
        public int TransactionId { get; set; }
      

    }
}