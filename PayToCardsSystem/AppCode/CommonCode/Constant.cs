using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace PayToCardsSystem.AppCode.CommonCode
{
    public static class Constant
    {
        public static string ROLE_SUPER_ADMIN = "Super Admin";
        public static string ROLE_ADMIN = "Admin";
        public static string ROLE_DISTRIBUTOR = "Distributor";
        public static string ROLE_MERCHANT = "Merchant";

        public enum Role
        {
            [Description("Super Admin")]
            SuperAdmin,
            [Description("Admin")]
            Admin,
            [Description("Distributor")]
            Distributor,
            [Description("Merchant")]
            Merchant

        };
        public enum TransactionType
        {
            Transaction,
            TopUp

        };
        public enum ResponseMsg
        {
            Success,
            Fail

        };
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}