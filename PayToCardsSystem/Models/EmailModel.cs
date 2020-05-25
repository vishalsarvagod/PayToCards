using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayToCardsSystem.Models
{
    public class EmailModel
    {
        public string To { get; set; }
        public string Subject { get; set; }

        public string  Body{ get; set; }
        public string CC { get; set; }
    }
}