using PayToCardsSystem.AppCode.DAO;
using PayToCardsSystem.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayToCardsSystem.DTO.Response
{
    public class SaveTransactionResponseDTO
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public List<TransationData> cardPaymentResponse { get; set; }
        public List<ErrorDTO> errorList { get; set; }
    }
    public class ErrorDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string StatusCode { get; set; }

    }
}