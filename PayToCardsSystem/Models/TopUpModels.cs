using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayToCardsSystem.Models
{
    public class TopUpModels
    {
        public int PkId { get; set; } = 0;

        [Required]
        public string UserValue { get; set; }

        public List<SelectListItem> UserNameList { get; set; }

        [Required]
        [RegularExpression(@"^[1-9][0-9]*$", ErrorMessage = "Select Curreny")]
        public string CurrencyValue { get; set; }
        public List<SelectListItem> CurrencyList { get; set; }

        public double Balance { get; set; }

        [Required]
        [RegularExpression(@"^-?[0-9]\d*(\.\d+)?$", ErrorMessage = "Enter only Number")]
        public double TopupAmount { get; set; }

        [Required]
        public string Remarks { get; set; }
        public string UserBalance { get; set; }
        public string UserPerviousBalance { get; set; }
        public string Currency { get; set; }
        public string LoginUserBalance { get; set; }
        public string LoginUserPerviousBalance { get; set; }
        public bool IsReport { get; set; }
        public string SelectUserName { get; set; }
    }
}