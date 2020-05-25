using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayToCardsSystem.AppCode.DAO
{
    public class CardPaymentResponse
    {
        public string id { get; set; }
  //      public string paymentType { get; set; }
  //      public string paymentBrand { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string descriptor { get; set; }
        public Result result { get; set; }
    //    public Card card { get; set; }
     //   public Risk risk { get; set; }
  //      public string buildNumber { get; set; }
        public string timestamp { get; set; }
  //      public string ndc { get; set; }

        public override string ToString()
        {
            return "Id : " + id + "<br /> Amount : " + amount + "<br /> Result : Code - " + result.code + " Description - " + result.description;
        }
    }
    public class Result
    {
        public string code { get; set; }
        public string description { get; set; }
        public List<ParameterError> parameterErrors { get; set; }
    }
    public class ParameterError
    {
        public string name { get; set; }
        public object value { get; set; }
        public string message { get; set; }
    }

    //public class Card
    //{
    //    public string bin { get; set; }
    //    public string last4Digits { get; set; }
    //    public string holder { get; set; }
    //    public string expiryMonth { get; set; }
    //    public string expiryYear { get; set; }
    //}

    //public class Risk
    //{
    //    public string score { get; set; }
    //}
}