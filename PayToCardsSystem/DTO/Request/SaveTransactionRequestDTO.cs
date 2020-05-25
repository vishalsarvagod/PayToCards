using PayToCardsSystem.AppCode.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayToCardsSystem.DTO.Request
{
    public class SaveTransactionRequestDTO
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string ApiPassword { get; set; }

        [Required]
        public TransationData transactionData { get; set; }
    }
    public class TransationData
    {
        [Required]
        [RegularExpression(@"^[3456][0-9]*$", ErrorMessage = "Card number should begin with either 3 or 4 or 5 or 6")]
        [Display(Name = "Card Number")]
        [StringLength(16, MinimumLength = 15, ErrorMessage = "Card number must be 15 or 16 digit")]
        public string CardNumber { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMM/yyyy}")]
        public string ExpiryDate { get; set; }

        [Required]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Kind`ly Enter Two Decimal And Greater Than Zero")]
        public double Amount { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]{3}", ErrorMessage = "Currency Code Should be in (ISO-4217)")]
        public string Currency { get; set; }

        //[Required]
        [RegularExpression(@"^[\s\S]{1,10}", ErrorMessage = "Maximum 10 Characters Allowed")]
        public string Descriptor { get; set; }
        public string Comments { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Card holder name must be more than 3 characters")]
        [RegularExpression("^[a-zA-Z ']+$", ErrorMessage = "only alphabets required")]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{7,16}$", ErrorMessage = "Not a valid Phone number")]
        public string ContactNo { get; set; }

        public string message { get; set; }
        public string code { get; set; }
        public string parameterName { get; set; }
        public int TransactionId { get; set; }

        public CardPaymentResponse cardPaymentReponse { get; set; }
    }
}