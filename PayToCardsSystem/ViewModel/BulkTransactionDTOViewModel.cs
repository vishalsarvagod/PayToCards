using PayToCardsSystem.DTO;
using PayToCardsSystem.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayToCardsSystem.ViewModel
{
    public class BulkTransactionDTOViewModel
    {    
        public BulkTransactionDTOViewModel()
        {
            ListBulkTransactionDTO = new List<TransationData>();
        }
        public List<TransationData> ListBulkTransactionDTO { get; set; }
        public bool IsComplete { get; set; }
        public bool IsSubmit { get; set; }
    }
}