using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayToCardsSystem.Models
{
    public class UserCreationModel
    {
        public UserCreationModel()
        {
            RoleNameList = new List<SelectListItem>();
            listCurrency = new List<AddUserCurrency>();
            SelectedCurrency = new AddUserCurrency();
        }
        public int PkId { get; set; } = 0;
        [Required]
        public string RoleValue { get; set; }

        public List<SelectListItem> RoleNameList { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z ']+$", ErrorMessage = "only alphabets required")]
        public string FirstName { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z ']+$", ErrorMessage = "only alphabets required")]
        public string LastName { get; set; }

        [Required]
        public string UserName { get; set; }


        [Required]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*(_|[^\\w])).+$",ErrorMessage = "Password should contain uppercase,lowercase character, digit and special character")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
        [DataType(DataType.Password)]
        public string RetypePassword { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string Address1 { get; set; }


        public string Address2 { get; set; }

        public string City { get; set; }

        [Required]
        public string CountryValue { get; set; } = "226";

        public List<SelectListItem> CountryNameList { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Not a valid Postal code")]
        public string PostalCode { get; set; }

        [EmailAddress]
        [Required]
        public string EmailId { get; set; }

       
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^[0-9]{7,16}$", ErrorMessage = "Not a valid Phone number")]
        public string Telephone { get; set; }             
        public string CurrencyValue { get; set; }

        public List<SelectListItem> CurrencyList { get; set; }

        public double AgentCommInPer { get; set; } = 0;

        public double AgentCommInAmount { get; set; } = 0;

         
        public string SecurityValue { get; set; }

        public List<SelectListItem> SecurityList { get; set; }

        public string Answer { get; set; }

        public AddUserCurrency SelectedCurrency { get; set; }
        public List<AddUserCurrency> listCurrency { get; set; }
        public string FeeCalculationValue { get; set; }
        public List<SelectListItem> ListFeeCalculation { get; set; }
        //  public List<AddUserCurrency> SelectedFeeCalculation { get; set; }
        public string AllowIpAddress { get; set; }
        public string BlockIpAddress { get; set; }
        public bool IsActive { get; set; }
        public string ApiPassword { get; set; }
    }
    public class ManageUserModel
    {
        public int SrNo { get; set; }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string AdminName { get; set; }
        public string DistributorName { get; set; }
        public string CompanyName { get; set; }
        public bool IsActive { get; set; }
    }
    public class AddUserCurrency
    {
        public int SrNo { get; set; }
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        [Required]
        public double PercentAmount { get; set; }
        [Required]
        public double FlatAmount { get; set; }
        public bool IsActive { get; set; }
        public List<SelectListItem> Cur { get; set; }
        [Required]
        public string FeeValue { get; set; }
        public string FeeName { get; set; }
        public List<SelectListItem> ListFeeCal { get; set; }

        public AddUserCurrency()
        {
            Cur = new List<SelectListItem>();
            ListFeeCal = new List<SelectListItem>();
        }

        public void SelectItem(string value)
        {
            var itemList = Cur.Where(c => c.Value == value);          
            if (itemList != null && itemList.Count() > 0)
            {
                itemList.FirstOrDefault().Selected = true;
             
                CurrencyName = itemList.FirstOrDefault().Text;
            }
        }
        public void SelectItemFee(string value)
        {
          
            var itemList = ListFeeCal.Where(c => c.Value == value);
            if (itemList != null && itemList.Count() > 0)
            {
                itemList.FirstOrDefault().Selected = true;

                FeeName = itemList.FirstOrDefault().Text;
            }
        }
    }
}