using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayToCardsSystem.Service
{
    public class CurrencyService
    {
        vmpaytocardsEntities dbContext = new vmpaytocardsEntities();

        public List<SelectListItem> GetAllCurrency(user_personal_details userDetails)
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            var innerQry = from x in dbContext.user_currency_details
                           where x.UserId == userDetails.C_id && x.IsActive == true
                           select x.CurrencyId;
            var currencies = from c in dbContext.currencies
                             where innerQry.Contains(c.C_id)
                             select c;
            List<currency> userRole = currencies.ToList();
            List<SelectListItem> temproleNames = new List<SelectListItem>();
            userRole.ForEach(x =>
            {
                temproleNames.Add(new SelectListItem { Text = x.ShortCode, Value = x.C_id.ToString() });
            });
            return temproleNames;
        }
        public List<SelectListItem> GetAllCurrencyWithInActive(user_personal_details userDetails)
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            var innerQry = from x in dbContext.user_currency_details
                           where x.UserId == userDetails.C_id
                           select x.CurrencyId;
            var currencies = from c in dbContext.currencies
                             where innerQry.Contains(c.C_id)
                             select c;
            List<currency> userRole = currencies.ToList();
            List<SelectListItem> temproleNames = new List<SelectListItem>();
            userRole.ForEach(x =>
            {
                temproleNames.Add(new SelectListItem { Text = x.ShortCode, Value = x.C_id.ToString() });
            });
            return temproleNames;
        }
        public List<SelectListItem> GetAllCurrencyValueShortcode(user_personal_details userDetails)
        {
            vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
            var innerQry = from x in dbContext.user_currency_details
                           where x.UserId == userDetails.C_id
                           select x.CurrencyId;
            var currencies = from c in dbContext.currencies
                             where innerQry.Contains(c.C_id)
                             select c;
            List<currency> userRole = currencies.ToList();
            List<SelectListItem> temproleNames = new List<SelectListItem>();
            userRole.ForEach(x =>
            {
                temproleNames.Add(new SelectListItem { Text = x.ShortCode, Value = x.ShortCode.ToString() });
            });
            return temproleNames;
        }
    }
}