using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace PayToCardsSystem.Models
{
    public class TransactionModels
    {
        
        [Required]
        [Display(Name = "CardNumber")]
        [RegularExpression(@"^[3456][0-9]*$", ErrorMessage ="Card number should begin with either 3 or 4 or 5 or 6")]
        [StringLength(16,MinimumLength =15,ErrorMessage ="Card number must be 15 or 16 digit")]
        public string CardNumber { get; set; }

        [Required]
        [Display(Name = "ExpireDate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMM/yyyy}")]
        public DateTime ExpireDate { get; set; } = DateTime.Now;

        [Required]
        //[RegularExpression(@"^[0-9]{1,8}(\.[0-9]{2})?", ErrorMessage = "Kindly Enter Two Decimal")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Kindly Enter Two Decimal And Greater Than Zero")]
        [Display(Name = "TransactionAmount")]
        public double TransactionAmount { get; set; }

        [Required]
        //[RegularExpression(@"^[A-Z]{3}",ErrorMessage ="Currency Code Should be in (ISO-4217)")]
        [Display(Name = "Currency")]
        public string Currency { get; set; }

        [Display(Name = "Descriptor")]
        [RegularExpression(@"^[\s\S]{1,10}", ErrorMessage = "Maximum 10 Characters Allowed")]
        //[StringLength(10,ErrorMessage ="Maximum 10 Characters Allowed")]
        public string Message1 { get; set; }
        [Display(Name = "Comments")]
        public string Message2 { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Card holder name must be more than 3 characters")]
        [Display(Name = "CardHolderName")]
        [RegularExpression("^[a-zA-Z ']+$", ErrorMessage = "only alphabets required")]
        public string CardHolderName { get; set; }

        [Required]
        [Display(Name = "ContactNo")]
        [RegularExpression(@"^[0-9]{7,16}$", ErrorMessage = "Not a valid Phone number")]
        public string ContactNo { get; set; }

        public int UserId { get; set; }
        public Double Balance { get; set; }

        public List<SelectListItem> CurrencyList { get; set; }
    }
    public class TransactionViewModel
    {
        public int Id { get; set; }
        public int TransactionNo { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpireDate { get; set; }
        public double TransactionAmount { get; set; }
        public double TransactionFee { get; set; }
        public string Currency { get; set; }
        public string Message1 { get; set; }
        public string Message2 { get; set; }
        public string CardHolderName { get; set; }
        public string ContactNo { get; set; }
        public string Status { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy HH:mm}")]
        public DateTime TransactionDate { get; set; }
        public string Remarks { get; set; }
        public string UserName { get; set; }
        public double Total { get; set; }
        public string Response { get; set; }
        
    }
    public class userFeeModel
    {
        public string Currency { get; set; }
        public double FlatFee { get; set; }
        public double PercentFee { get; set; }
    }
    public class BalanceListModel
    {
        public List<SelectListItem> UserList { get; set; }
        public string UserValue { get; set; }
        public string CurrencyValue { get; set; }
        public List<SelectListItem> CurrencyList { get; set; }
        public List<BalanceHistoryViewModel> ListBalanceHistory { get; set; }
    }
    public class BalanceHistoryViewModel
    {

        public int SrNo { get; set; }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public Double Balance { get; set; }
        public string Currency { get; set; }
        public bool IsActive { get; set; }
        
    }

    public class TransactionSearchModel
    {
        [Display(Name = "FromDate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMM-dd-yyyy}")]
        public DateTime FromDate { get; set; } = DateTime.Now.AddDays(-1);

        [Display(Name = "ToDate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMM-dd-yyyy}")]    //yyyy-MMM-dd
        public DateTime ToDate { get; set; } = DateTime.Now;
        public string UserValue { get; set; }
        public List<SelectListItem> UserList { get; set; }
        public string CurrencyValue { get; set; }
        public List<SelectListItem> CurrencyList { get; set; }

        [Display(Name = "CardNumber")]
        [RegularExpression(@"^[3456][0-9]*$", ErrorMessage = "Card number should begin with either 3 or 4 or 5 or 6")]
        public string CardNumber { get; set; }

        
        [RegularExpression(@"^[0-9]{1,8}(\.[0-9]{2})?", ErrorMessage = "Kindly Enter Amount in 2 Decimal")]
        [Display(Name = "TransactionAmount")]
        public double TransactionAmountFrom { get; set; }

        
        [RegularExpression(@"^[0-9]{1,8}(\.[0-9]{2})?", ErrorMessage = "Kindly Enter Amount in 2 Decimal")]
        [Display(Name = "TransactionAmountTo")]
        public double TransactionAmountTo{ get; set; }

        public bool Success { get; set; }
        public bool Fail { get; set; }

        public List<TransactionViewModel> ListTransactionViewModel { get; set; }
    }
    public class HourlyTransHistoryViewModel
    {

        public string TimeOfTrans { get; set; }
        public int NoOfTrans { get; set; }
        public string Status { get; set; }

    }
    public class LadgerDetailsModel
    {
        public string TransactionNo { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpireDate { get; set; }
        public double TransactionAmount { get; set; }
        public string Currency { get; set; }      
        public string CardHolderName { get; set; }
        public string ContactNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string TransactionType { get; set; }
        public string User { get; set; }
        public string Role { get; set; }
        public string Credit { get; set; }
        public string Debit { get; set; }
        public string RunningBalance { get; set; }
        public bool CheckType { get; set; } // to Check Topup or Transaction
        public string TopupDoneBy { get; set; }
        public string TopupDoneTo { get; set; }
        public string TransactionDoneFor { get; set; }

    }
}