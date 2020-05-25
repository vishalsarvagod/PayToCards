using Newtonsoft.Json;
using PayToCardsSystem.AppCode.CommonCode;
using PayToCardsSystem.AppCode.DAO;
using PayToCardsSystem.DTO;
using PayToCardsSystem.DTO.Request;
using PayToCardsSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace PayToCardsSystem.Services
{
    public class TransactionService
    {
        vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
        public TransactionDTO saveTranWithApi(TransactionModels transModel, double transactionFee, user_personal_details userDetails, int CurId)
        {


            TransactionDTO tranDTO = new TransactionDTO();
            CardPaymentResponse cardPayment = null;
            string mPaymentBrand = "";
            try
            {
                string firstChar = transModel.CardNumber.Substring(0, 1);
                if (firstChar.Equals("4"))
                {
                    mPaymentBrand = "VISA";
                }
                else if (firstChar.Equals("5"))
                {
                    mPaymentBrand = "MASTER";
                }
                else if (firstChar.Equals("6"))
                {
                    mPaymentBrand = "DISCOVER";
                }
                else if (firstChar.Equals("3"))
                {
                    mPaymentBrand = "AMEX";
                }
                var swishObject = dbContext.swishme_credential.First();                           
                Dictionary<string, string> postParameter = new Dictionary<string, string>();
                
                postParameter.Add("authentication.userId", Utility.DecryptionAES(swishObject.userId, "Pay2cards@VM#"));
                postParameter.Add("authentication.password", Utility.DecryptionAES(swishObject.password, "Pay2cards@VM#"));
                postParameter.Add("authentication.entityId", Utility.DecryptionAES(swishObject.entityId, "Pay2cards@VM#"));
                postParameter.Add("currency", transModel.Currency);
                postParameter.Add("paymentBrand", mPaymentBrand);
                postParameter.Add("paymentType", "CD");
                postParameter.Add("amount", transModel.TransactionAmount.ToString());
                postParameter.Add("card.number", transModel.CardNumber);
                postParameter.Add("card.holder", transModel.CardHolderName);
                if (!string.IsNullOrEmpty(transModel.Message1))
                {
                    postParameter.Add("descriptor", transModel.Message1);
                }
                if (transModel.ExpireDate.Month < 10)
                    postParameter.Add("card.expiryMonth", "0" + transModel.ExpireDate.Month.ToString());
                else
                    postParameter.Add("card.expiryMonth", transModel.ExpireDate.Month.ToString()); //Need to check
                postParameter.Add("card.expiryYear", transModel.ExpireDate.Year.ToString()); //Need to check
                                                                                             //if (txtCardCvv.Text.Length > 0)
                                                                                             //    postParameter.Add("card.cvv", txtCardCvv.Text);
                                                                                             //else
                                                                                             //    postParameter.Add("card.cvv", "123");
                ResponseModel response;
                response = HttpRequest.sendRequest(Utility.DecryptionAES(swishObject.url, "Pay2cards@VM#"), "POST", postParameter);
                cardPayment = JsonConvert.DeserializeObject<CardPaymentResponse>(response.ResponseText);

                bool isSuccess = isSwishSuccessCode(cardPayment.result.code.ToString());
                tranDTO.IsSuccess = isSuccess;

                transaction_card objTransCard = new transaction_card();
                {
                    objTransCard.CardNumber = maskCard(transModel.CardNumber);
                    objTransCard.ExpireDate = transModel.ExpireDate;
                    objTransCard.TransactionAmount = transModel.TransactionAmount;
                    objTransCard.TransactionFee = transactionFee;
                    objTransCard.Currency = transModel.Currency;
                    objTransCard.Message1 = transModel.Message1 == null ? "" : transModel.Message1.Trim();
                    objTransCard.Message2 = transModel.Message2 == null ? "" : (transModel.Message2.Length>100 ? transModel.Message2.Substring(0,100).Trim():transModel.Message2.Trim());
                    objTransCard.Remarks = objTransCard.Message1 + " - " + objTransCard.Message2;
                    objTransCard.CardHolderName = transModel.CardHolderName;
                    objTransCard.ContactNo = transModel.ContactNo;
                    if (isSuccess)
                    {
                        objTransCard.Status = Constant.ResponseMsg.Success.ToString();
                        objTransCard.TransactionNo = getUniqueTransctionNo();
                    }
                    else
                    {
                        objTransCard.Status = Constant.ResponseMsg.Fail.ToString();
                        objTransCard.TransactionNo = getUniqueTransctionNo();
                    }

                    objTransCard.TransactionDate = DateTime.Now;
                    objTransCard.UserId = transModel.UserId;

                    dbContext.transaction_card.AddObject(objTransCard);
                    dbContext.SaveChanges();
                }

                transaction_card_response objCardResponse = new transaction_card_response();
                {
                    objCardResponse.TransactionId = objTransCard.C_id;
                    objCardResponse.ResponseCode = cardPayment.result.code;
                    objCardResponse.ResponseMessage = cardPayment.result.description;
                    dbContext.transaction_card_response.AddObject(objCardResponse);
                    dbContext.SaveChanges();
                }
                if (isSuccess)
                {
                    //-------------Distributor Fee Calculation---------------//
                    int distributorId = Convert.ToInt32(userDetails.DistributorID);
                    double distributorFee = 0;
                    if (distributorId > 0)
                    {
                        user_fee_details distributorFeeStructure = dbContext.user_fee_details.FirstOrDefault(i => i.UserId == distributorId && i.CurrencyId == CurId);
                        distributorFee = transModel.TransactionAmount * distributorFeeStructure.PercentFee / 100;
                        if (distributorFeeStructure.FeeType.Equals("AND"))
                        {
                            distributorFee = distributorFee + distributorFeeStructure.FlatFee;
                        }
                        else
                        {
                            if (distributorFee < distributorFeeStructure.FlatFee)
                                distributorFee = distributorFeeStructure.FlatFee;
                        }
                    }
                    //-------------------------------------------------------//
                    //---------------Admin Fee Calculation-------------------//
                    int AdminId = Convert.ToInt32(userDetails.AdminId);
                    double AdminFee = 0;
                    int SuperAdminId = 0;
                    user_personal_details AdminUserDetails = new user_personal_details();
                    if (AdminId > 0)
                    {
                        AdminUserDetails = dbContext.user_personal_details.FirstOrDefault(i => i.C_id == AdminId);
                        if (AdminUserDetails != null)
                            SuperAdminId = Convert.ToInt32(AdminUserDetails.AdminId);
                        user_fee_details AdminFeeStructure = dbContext.user_fee_details.FirstOrDefault(i => i.UserId == AdminId && i.CurrencyId == CurId);
                        AdminFee = transModel.TransactionAmount * AdminFeeStructure.PercentFee / 100;
                        if (AdminFeeStructure.FeeType.Equals("AND"))
                        {
                            AdminFee = AdminFee + AdminFeeStructure.FlatFee;
                        }
                        else
                        {
                            if (AdminFee < AdminFeeStructure.FlatFee)
                                AdminFee = AdminFeeStructure.FlatFee;
                        }
                    }
                    //-------------------------------------------------------//
                    //----------------Super Admin Fee Calculation------------//
                    double SuperAdminFee = 0;
                    if (SuperAdminId > 0)
                    {
                        user_fee_details SuperAdminFeeStructure = dbContext.user_fee_details.FirstOrDefault(i => i.UserId == SuperAdminId && i.CurrencyId == CurId);
                        SuperAdminFee = transModel.TransactionAmount * SuperAdminFeeStructure.PercentFee / 100;
                        if (SuperAdminFeeStructure.FeeType.Equals("AND"))
                        {
                            SuperAdminFee = SuperAdminFee + SuperAdminFeeStructure.FlatFee;
                        }
                        else
                        {
                            if (SuperAdminFee < SuperAdminFeeStructure.FlatFee)
                                SuperAdminFee = SuperAdminFeeStructure.FlatFee;
                        }
                    }
                    //-------------------------------------------------------//
                    double fee = 0;
                    transaction_card_account objCardAccount = new transaction_card_account();
                    {
                        user_currency_details userCurrencyDetail = dbContext.user_currency_details.FirstOrDefault(i => i.UserId == transModel.UserId && i.Currency == transModel.Currency);
                        {
                            userCurrencyDetail.Balance = userCurrencyDetail.Balance - (transModel.TransactionAmount + transactionFee);
                        }
                        objCardAccount.Debit = Math.Round((transModel.TransactionAmount + transactionFee), 2);
                        objCardAccount.Credit = 0;
                        objCardAccount.UserId = transModel.UserId;
                        objCardAccount.TransactionId = objTransCard.C_id;
                        objCardAccount.RunningBalance = Math.Round(Convert.ToDouble(userCurrencyDetail.Balance), 2);
                        objCardAccount.TransactionType = Constant.TransactionType.Transaction.ToString();
                        dbContext.transaction_card_account.AddObject(objCardAccount);
                        dbContext.SaveChanges();
                        tranDTO.RunningBalance = objCardAccount.RunningBalance;
                    }
                    objCardAccount = new transaction_card_account();
                    {
                        user_currency_details userCurrencyDetail = dbContext.user_currency_details.FirstOrDefault(i => i.UserId == 2 && i.Currency == transModel.Currency);
                        {
                            userCurrencyDetail.Balance = userCurrencyDetail.Balance + transModel.TransactionAmount;
                        }
                        objCardAccount.Debit = 0;
                        objCardAccount.Credit = Math.Round(transModel.TransactionAmount, 2);
                        objCardAccount.UserId = 2;
                        objCardAccount.TransactionId = objTransCard.C_id;
                        objCardAccount.RunningBalance = Math.Round(Convert.ToDouble(userCurrencyDetail.Balance), 2);
                        objCardAccount.TransactionType = Constant.TransactionType.Transaction.ToString();
                        dbContext.transaction_card_account.AddObject(objCardAccount);
                        dbContext.SaveChanges();
                    }
                    if (distributorId > 0)
                    {
                        objCardAccount = new transaction_card_account();
                        {
                            user_currency_details userCurrencyDetail = dbContext.user_currency_details.FirstOrDefault(i => i.UserId == distributorId && i.Currency == transModel.Currency);
                            {
                                userCurrencyDetail.Balance = userCurrencyDetail.Balance + (transactionFee - distributorFee);
                            }
                            objCardAccount.Debit = 0;
                            objCardAccount.Credit = Math.Round((transactionFee - distributorFee), 2);
                            objCardAccount.UserId = distributorId;
                            objCardAccount.TransactionId = objTransCard.C_id;
                            objCardAccount.RunningBalance = Math.Round(Convert.ToDouble(userCurrencyDetail.Balance), 2);
                            objCardAccount.TransactionType = Constant.TransactionType.Transaction.ToString();
                            dbContext.transaction_card_account.AddObject(objCardAccount);
                            dbContext.SaveChanges();

                        }
                    }
                    else
                        distributorFee = transactionFee;
                    if (AdminId > 0)
                    {
                        fee = AdminFee;
                        objCardAccount = new transaction_card_account();
                        {
                            user_currency_details userCurrencyDetail = dbContext.user_currency_details.FirstOrDefault(i => i.UserId == AdminId && i.Currency == transModel.Currency);
                            {
                                userCurrencyDetail.Balance = userCurrencyDetail.Balance + (distributorFee - AdminFee);
                            }
                            objCardAccount.Debit = 0;
                            objCardAccount.Credit = Math.Round((distributorFee - AdminFee), 2);
                            objCardAccount.UserId = AdminId;
                            objCardAccount.TransactionId = objTransCard.C_id;
                            objCardAccount.RunningBalance = Math.Round(Convert.ToDouble(userCurrencyDetail.Balance), 2);
                            objCardAccount.TransactionType = Constant.TransactionType.Transaction.ToString();
                            dbContext.transaction_card_account.AddObject(objCardAccount);
                            dbContext.SaveChanges();
                        }
                    }
                    if (SuperAdminId > 0)
                    {
                        objCardAccount = new transaction_card_account();
                        {
                            fee = SuperAdminFee;
                            user_currency_details userCurrencyDetail = dbContext.user_currency_details.FirstOrDefault(i => i.UserId == SuperAdminId && i.Currency == transModel.Currency);
                            {
                                userCurrencyDetail.Balance = userCurrencyDetail.Balance + (AdminFee - SuperAdminFee);
                            }
                            objCardAccount.Debit = 0;
                            objCardAccount.Credit = Math.Round((AdminFee - SuperAdminFee), 2);
                            //  fee = transModel.DistributionFee - transModel.SuperAdminFee;
                            objCardAccount.UserId = SuperAdminId;
                            objCardAccount.TransactionId = objTransCard.C_id;
                            objCardAccount.RunningBalance = System.Math.Round(Convert.ToDouble(userCurrencyDetail.Balance), 2);
                            objCardAccount.TransactionType = Constant.TransactionType.Transaction.ToString();
                            dbContext.transaction_card_account.AddObject(objCardAccount);
                            dbContext.SaveChanges();
                        }
                    }
                    objCardAccount = new transaction_card_account();
                    {
                        user_currency_details userCurrencyDetail = dbContext.user_currency_details.FirstOrDefault(i => i.UserId == 3 && i.Currency == transModel.Currency);
                        {
                            userCurrencyDetail.Balance = userCurrencyDetail.Balance + (fee);
                        }
                        objCardAccount.Debit = 0;
                        objCardAccount.Credit = Math.Round(fee, 2);
                        objCardAccount.UserId = 3;
                        objCardAccount.TransactionId = objTransCard.C_id;
                        objCardAccount.RunningBalance = System.Math.Round(Convert.ToDouble(userCurrencyDetail.Balance), 2);
                        objCardAccount.TransactionType = Constant.TransactionType.Transaction.ToString();
                        dbContext.transaction_card_account.AddObject(objCardAccount);
                        dbContext.SaveChanges();
                    }
                }
                tranDTO.TransactionNo = objTransCard.TransactionNo;
                tranDTO.CardNumber = objTransCard.CardNumber;
                tranDTO.cardPaymentReposne = cardPayment;
                return tranDTO;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private string maskCard(string strCardNumber)
        {
            int len = strCardNumber.Length - 8;
            string strMaskCard = strCardNumber.Substring(0, 4);
            for (int i = 0; i < len; i++)
                strMaskCard += "*";
            strMaskCard += strCardNumber.Substring(strCardNumber.Length - 4, 4);
            return strMaskCard;
        }
        public bool isSwishSuccessCode(string strResultCode)
        {
            //Result codes for successfully processed transactions
            if (Regex.IsMatch(strResultCode, @"^(000\.000\.|000\.100\.1|000\.[36])"))
                return true;
            //Result codes for successfully processed transactions that should be manually reviewed
            if (Regex.IsMatch(strResultCode, @"^(000\.400\.0|000\.400\.100)"))
                return true;
            //Result codes for pending transactions
            if (Regex.IsMatch(strResultCode, @"^(000\.200)"))
                return true;
            //Result codes for pending transactions (status of a transaction can change even after several days)
            if (Regex.IsMatch(strResultCode, @"^(800\.400\.5|100\.400\.500)"))
                return true;
            return false;
        }

        private int getUniqueTransctionNo()
        {
            try
            {
                Random ran = new Random(DateTime.Now.Millisecond);
                int key = 0;
                key = ran.Next(1000000000, int.MaxValue);
                transaction_card transCard = dbContext.transaction_card.FirstOrDefault(i => i.TransactionNo == key);
                if (transCard != null)
                    key = getUniqueTransctionNo();
                return key;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        

        public List<TransationData> SaveTransaction(List<TransationData> saveTranData, user_personal_details userDetails)
        {
            try
            {
                List<userFeeModel> listFee = new List<userFeeModel>();
                List<TransationData> listBulkTrans = new List<TransationData>();

                int userId = userDetails.C_id;
                var list = (from fee in dbContext.user_fee_details
                            from cur in dbContext.currencies
                            where fee.CurrencyId == cur.C_id && fee.UserId == userId && fee.IsActive == true
                            orderby cur.ShortCode
                            select new { cur.ShortCode, fee.FlatFee, fee.PercentFee });
                foreach (var element in list)
                {
                    listFee.Add(new userFeeModel
                    {
                        Currency = element.ShortCode,
                        FlatFee = element.FlatFee,
                        PercentFee = element.PercentFee
                    });
                }
                for (int i = 0; i < saveTranData.Count; i++)
                {
                    TransactionModels transModel = new TransactionModels();
                    var tempBulkModel = saveTranData[i];
                    if ((listFee.Where(z => z.Currency == tempBulkModel.Currency)).Count() > 0)
                    {
                        transModel.CardNumber = tempBulkModel.CardNumber;
                        if (validateDate(tempBulkModel.ExpiryDate.ToString()))
                        {
                            transModel.ExpireDate = Convert.ToDateTime(tempBulkModel.ExpiryDate);
                            if (validateAmount(tempBulkModel.Amount.ToString()))
                            {
                                transModel.TransactionAmount = Math.Round(tempBulkModel.Amount,2);
                                transModel.Currency = tempBulkModel.Currency;
                                transModel.Message1 = tempBulkModel.Descriptor;
                                if (tempBulkModel.Comments.Length > 100)
                                {
                                    transModel.Message2 = tempBulkModel.Comments.Substring(0, 100);
                                }
                                else
                                {
                                    transModel.Message2 = tempBulkModel.Comments;
                                }
                              
                                transModel.CardHolderName = tempBulkModel.Name;
                                transModel.ContactNo = tempBulkModel.ContactNo;
                                transModel.UserId = userId;
                                string strRet = validate(transModel);
                                if (strRet.Equals(""))
                                {
                                    double transactionFee = 0;
                                    user_currency_details userCur = dbContext.user_currency_details.FirstOrDefault(cur => cur.UserId == userId && cur.Currency == transModel.Currency);
                                    user_fee_details userFeeDetail = dbContext.user_fee_details.FirstOrDefault(fee => fee.UserId == userId && fee.CurrencyId == userCur.CurrencyId);
                                    double percentFee = 0;
                                    percentFee = transModel.TransactionAmount * userFeeDetail.PercentFee / 100;
                                    if (userFeeDetail.FeeType.Equals("AND"))
                                    {
                                        transactionFee = percentFee + userFeeDetail.FlatFee;
                                    }
                                    else
                                    {
                                        if (percentFee > userFeeDetail.FlatFee)
                                            transactionFee = percentFee;
                                        else
                                            transactionFee = userFeeDetail.FlatFee;
                                    }
                                    if (userCur.Balance >= (transModel.TransactionAmount + transactionFee))
                                    {
                                        TransactionDTO transDTO = saveTranWithApi(transModel, transactionFee, userDetails, userCur.CurrencyId);
                                        if (transDTO != null)
                                        {


                                            tempBulkModel.CardNumber = transDTO.CardNumber;
                                            CardPaymentResponse cardResponse = transDTO.cardPaymentReposne;
                                            tempBulkModel.code = cardResponse.result.code;
                                            tempBulkModel.message = cardResponse.result.description.ToString();
                                            tempBulkModel.cardPaymentReponse = cardResponse;
                                            tempBulkModel.message = cardResponse.result.description.ToString();
                                            //if (isSwishSuccessCode(cardResponse.result.code.ToString()))
                                            //{
                                                tempBulkModel.TransactionId = transDTO.TransactionNo;
                                            //}
                                        }
                                        else
                                        {
                                            tempBulkModel.code = "100.000.000";
                                            tempBulkModel.parameterName = "Unable to Connect Server";
                                            tempBulkModel.message = "Unable to Connect Server";
                                        }
                                    }
                                    else
                                    {
                                        tempBulkModel.code = "200.300.404";
                                        tempBulkModel.parameterName = "Balance";
                                        tempBulkModel.message = "Out of Balance";
                                    }
                                }
                                else
                                {
                                    tempBulkModel.code = "200.300.404";
                                    tempBulkModel.parameterName = strRet;
                                    tempBulkModel.message = strRet;
                                }
                            }
                            else
                            {
                                tempBulkModel.code = "200.300.404";
                                tempBulkModel.parameterName = "Amount";
                                tempBulkModel.message = "IncorrectAmount";
                            }
                        }
                        else
                        {
                            tempBulkModel.code = "200.300.404";
                            tempBulkModel.parameterName = "Expiry Date";
                            tempBulkModel.message = "Incorrect Expiry Date";
                        }
                    }
                    else
                    {
                        tempBulkModel.code = "200.300.404";
                        tempBulkModel.parameterName = "Currency";
                        tempBulkModel.message = "Unsupported Currency";
                    }
                    if (tempBulkModel.cardPaymentReponse == null)
                    {
                        tempBulkModel.cardPaymentReponse = new CardPaymentResponse();
                        Result result = new Result();
                        result.code = tempBulkModel.code;
                        result.description = "invalid or missing parameter";
                        List<ParameterError> listError = new List<ParameterError>();
                        ParameterError errorDto = new ParameterError();
                        errorDto.name = tempBulkModel.parameterName;
                        errorDto.value = "";
                        errorDto.message = tempBulkModel.message;
                        listError.Add(errorDto);
                        result.parameterErrors = listError;
                        tempBulkModel.cardPaymentReponse.result = result;
                    }
                    listBulkTrans.Add(tempBulkModel);
                }
                return listBulkTrans;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region Validation
        private bool validateAmount(string strAmount)
        {
            double amnt;
            if (double.TryParse(strAmount, out amnt))
            {
                if (amnt > 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        private bool validateDate(string strDate)
        {
            int temp = 0;
            DateTime dt = DateTime.Now;
            if (strDate.Length != 7)
                return false;
            if (!int.TryParse(strDate.Substring(0, 2), out temp))
                return false;
            if (!(temp > 0 && temp < 13))
                return false;
            if (!strDate.Substring(2, 1).Equals("/"))
                return false;
            if (!int.TryParse(strDate.Substring(3), out temp))
                return false;
            if (temp < dt.Year)
                return false;
            return true;
        }
        private string validate(TransactionModels transModel)
        {
            string strErr = "";
            string strFirstChar = transModel.CardNumber.Substring(0, 1);
            if (!(strFirstChar.Equals("3") || strFirstChar.Equals("4") || strFirstChar.Equals("5") || strFirstChar.Equals("6")))
            {
                strErr += "Card number should begin with either 3 or 4 or 5 or 6";
            }
            else
            {
                if (strFirstChar.Equals("3") && transModel.CardNumber.Length != 15)
                    strErr += "Incorrect card number. Amex 15 digits";
                else if (transModel.CardNumber.Length != 16)
                    strErr += "Incorrect card number it should be 16 digits";
            }
            if (!string.IsNullOrEmpty(transModel.Message1))
            {
                if (transModel.Message1.Length > 0 && !System.Text.RegularExpressions.Regex.IsMatch(transModel.Message1, @"^[\s\S]{1,10}$"))
                {
                    strErr += "Descriptor should be maximum 10 characters";
                }
            }
            if (transModel.CardHolderName.Length < 3 || transModel.CardHolderName.Length > 50)
            {
                strErr += "Holder name should be between 3 to 50 characters";
            }
            if (transModel.Currency.Length != 3)
            {
                strErr += "Incorrect Currency";
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(transModel.ContactNo, "^[0-9]{7,16}$"))
            {
                strErr += "Not a valid Phone number";
            }
            return strErr;
        }
        private bool validateRegex(string strData, string strRegex)
        {
            System.Configuration.RegexStringValidator regex = new System.Configuration.RegexStringValidator(strRegex);
            try
            {
                regex.Validate(strData);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}