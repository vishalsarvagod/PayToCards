using PayToCardsSystem.AppCode.CommonCode;
using PayToCardsSystem.AppCode.DAO;
using PayToCardsSystem.DTO;
using PayToCardsSystem.DTO.Request;
using PayToCardsSystem.DTO.Response;
using PayToCardsSystem.Models;
using PayToCardsSystem.Service;
using PayToCardsSystem.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace PayToCardsSystem.Controllers.Api
{
    public class TransactionController : ApiController
    {
        private vmpaytocardsEntities _context;
        public TransactionController()
        {
            _context = new vmpaytocardsEntities();

        }
        // GET /api/transaction/1
        public IHttpActionResult GetHourlyTransaction(int UserId)
        {


            List<HourlyTransDTO.Dataset> listDataSet = new List<HourlyTransDTO.Dataset>();

            HourlyTransDTO.Data hData = new HourlyTransDTO.Data();
            hData.labels = new List<string>();

            string query = "SELECT DATE_FORMAT(TransactionDate, ' %h %p') as 'TimeOfTrans',transaction_card.Status,Count(*) as 'NoOfTrans' FROM transaction_card " +
                " inner join user_personal_details on user_personal_details._id = transaction_card.UserId " +
               " where TransactionDate between concat(curdate(),' 00:00:00') and concat(curdate(),' 23:59:59') ";
            user_personal_details userPersonalDetails = _context.user_personal_details.FirstOrDefault(i => i.C_id == UserId);
            if (userPersonalDetails != null)
            {
                if (userPersonalDetails.Role.Equals(Constant.ROLE_ADMIN))
                    query += "and user_personal_details.AdminId = " + userPersonalDetails.C_id + "";
                else if (userPersonalDetails.Role.Equals(Constant.ROLE_DISTRIBUTOR))
                    query += "and (transaction_card.UserId = " + userPersonalDetails.C_id + " || user_personal_details.DistributorID = " + userPersonalDetails.C_id + ")";
                else if (userPersonalDetails.Role.Equals(Constant.ROLE_MERCHANT))
                    query += "and transaction_card.UserId = " + userPersonalDetails.C_id + "";
            }

            query +=
                " group by TimeOfTrans,transaction_card.Status " +
                " order by TimeOfTrans,transaction_card.Status ";
            var hourlyTransList = _context.ExecuteStoreQuery<HourlyTransHistoryViewModel>(query).ToList();
            HourlyTransDTO.Dataset hDataSetSuccess = new HourlyTransDTO.Dataset();
            hDataSetSuccess.label = "Success";
            hDataSetSuccess.borderColor = "rgb(52, 152, 219)";
            hDataSetSuccess.data = new List<int>();

            HourlyTransDTO.Dataset hDataSetFail = new HourlyTransDTO.Dataset();
            hDataSetFail.label = "Failed";
            hDataSetFail.borderColor = "rgb(231, 76, 60)";
            hDataSetFail.data = new List<int>();
            string label = "";
            bool isSuccess = false, isFail = false;
            int count = 0;
            foreach (var hourlyList in hourlyTransList)
            {
                if (!label.Equals(hourlyList.TimeOfTrans))
                {
                    hData.labels.Add(hourlyList.TimeOfTrans);
                    label = hourlyList.TimeOfTrans;
                    //isSuccess = false;
                    //isFail = false;
                    if (count > 0)
                    {
                        if (!isSuccess) { hDataSetSuccess.data.Add(0); }
                        if (!isFail) { hDataSetFail.data.Add(0); }
                        isSuccess = false; isFail = false;
                    }
                    count++;
                }
                if (hourlyList.Status.Equals(Constant.ResponseMsg.Fail.ToString()))
                {
                    hDataSetFail.data.Add(hourlyList.NoOfTrans);
                    isFail = true;
                }
                if (hourlyList.Status.Equals(Constant.ResponseMsg.Success.ToString()))
                {
                    hDataSetSuccess.data.Add(hourlyList.NoOfTrans);
                    isSuccess = true;
                }
            }
            if (!isSuccess) { hDataSetSuccess.data.Add(0); }
            if (!isFail) { hDataSetFail.data.Add(0); }

            listDataSet.Add(hDataSetSuccess);
            listDataSet.Add(hDataSetFail);

            hData.datasets = listDataSet;

            HourlyTransDTO.Title hTitle = new HourlyTransDTO.Title();
            hTitle.display = true;
            hTitle.text = "Hourly Report";

            HourlyTransDTO.Options hOption = new HourlyTransDTO.Options();
            hOption.title = hTitle;


            HourlyTransDTO hourlyDTO = new HourlyTransDTO();
            hourlyDTO.type = "line";
            hourlyDTO.data = hData;
            hourlyDTO.options = hOption;

            return Ok(hourlyDTO);
        }

        // POST /api/transaction
        [HttpPost]
        public IHttpActionResult SaveTransaction(SaveTransactionRequestDTO savrTransactionDTO)
        {
            if (savrTransactionDTO != null)
            {
                             
                TransactionModels transModel = new TransactionModels();
                CardPaymentResponse cardResponse = new CardPaymentResponse();
                TransactionService transService = new TransactionService();
                cardResponse.id = "0";
                //cardResponse.paymentType = "";
                //cardResponse.paymentBrand = "";
                if (savrTransactionDTO.transactionData != null)
                {
                    cardResponse.amount = savrTransactionDTO.transactionData.Amount.ToString();
                    cardResponse.currency = savrTransactionDTO.transactionData.Currency;
                    cardResponse.descriptor = savrTransactionDTO.transactionData.Descriptor;
                }
                //Card card = new Card();
                //card.bin = "";
                //card.last4Digits = "";
                //card.holder = "";
                //card.expiryMonth = "";
                //card.expiryYear = "";

                //cardResponse.card = card;

                //Risk risk = new Risk();
                //risk.score = "";

                //cardResponse.risk = risk;

                //cardResponse.buildNumber = "";
                cardResponse.timestamp = DateTime.Now.ToString();
                //    cardResponse.ndc = "";

                if (!ModelState.IsValid)
                {
                    Result result = new Result();
                    result.code = "200.300.404";
                    result.description = "invalid or missing parameter";

                    List<ParameterError> listError = new List<ParameterError>();

                    foreach (var key in ModelState.Keys)
                    {
                        ModelState modelState = ModelState[key];
                        ParameterError errorDTO = new ParameterError();
                        string[] words = key.Split('.');
                        try
                        {
                            errorDTO.name = words[words.Length - 1];
                            foreach (ModelError error in modelState.Errors)
                            {
                                errorDTO.message = error.ErrorMessage;

                            }
                            listError.Add(errorDTO);
                        }
                        catch (Exception ex)
                        {
                        }

                    }
                    result.parameterErrors = listError;
                    cardResponse.result = result;





                    return Ok(cardResponse);
                }

                //If User Is Deactive?

                var userDetails = _context.user_personal_details.FirstOrDefault(cur => cur.UserName == savrTransactionDTO.UserId && cur.ApiPassword == savrTransactionDTO.ApiPassword && (cur.Role.Equals(Constant.ROLE_DISTRIBUTOR) || cur.Role.Equals(Constant.ROLE_MERCHANT)) && cur.IsActive==true);
                if (userDetails == null)
                {
                    Result result = new Result();
                    result.code = "200.300.404";
                    result.description = "invalid or missing parameter";
                    List<ParameterError> listError = new List<ParameterError>();
                    ParameterError errorDto = new ParameterError();
                    errorDto.name = "UserName";
                    errorDto.value = "";
                    errorDto.message = "Bad User";
                    listError.Add(errorDto);
                    result.parameterErrors = listError;
                    cardResponse.result = result;

                }
                else
                {
                    string ip = Request.GetIP();
                    UserService userSerivce = new UserService();
                    if (!string.IsNullOrEmpty(ip) && userSerivce.IsAllowedIPAddress(userDetails, ip))
                    {
                        List<TransationData> transDataList = new List<TransationData>();
                        transDataList.Add(savrTransactionDTO.transactionData);
                        List<TransationData> listBulkTrans = transService.SaveTransaction(transDataList, userDetails);
                        cardResponse.result = listBulkTrans[0].cardPaymentReponse.result;
                        cardResponse.id = listBulkTrans[0].TransactionId.ToString();

                    }
                    else
                    {
                        Result result = new Result();
                        result.code = "200.300.404";
                        result.description = "invalid or missing parameter";
                        List<ParameterError> listError = new List<ParameterError>();
                        ParameterError errorDto = new ParameterError();
                        errorDto.name = "Sorry you can't access this service. Please contact to administrator";
                        errorDto.value = "";
                        errorDto.message = "Sorry you can't access this service. Please contact to administrator";
                        listError.Add(errorDto);
                        result.parameterErrors = listError;
                        cardResponse.result = result;
                    }
               }
                return Ok(cardResponse);
            }
            else
            {
                SaveTransactionRequestDTO request = new SaveTransactionRequestDTO();
                CardPaymentResponse cardResponse = new CardPaymentResponse();
                cardResponse.id = "0";
                cardResponse.timestamp = DateTime.Now.ToString();
                Result result = new Result();
                result.code = "200.300.404";
                result.description = "invalid or missing parameter";

                List<ParameterError> listError = new List<ParameterError>();

                foreach (var key in request.GetType().GetProperties())
                {
                    var attribute = key.GetCustomAttributes(true);
                    ParameterError errorDTO = new ParameterError();
                    foreach (Attribute error in attribute)
                    {
                        if (error is RequiredAttribute)
                        {
                            listError.Add(new ParameterError { name = key.Name, value = "", message = "Value Cannot be null" });
                        }
                    }
                }
                result.parameterErrors = listError;
                cardResponse.result = result;
                return Ok(cardResponse);
            }
        }
    }
    public static class HttpRequestMessageExtensions
    {
        public static string GetIP(this HttpRequestMessage requestMessage)
        {
            // Owin Hosting
            //if (requestMessage.Properties.ContainsKey("MS_OwinContext"))
            //{
            //    return HttpContext.Current != null
            //        ? HttpContext.Current.Request.GetOwinContext().Request.RemoteIpAddress
            //        : null;
            //}
            // Web Hosting
            if (requestMessage.Properties.ContainsKey("MS_HttpContext"))
            {
                return HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : null;
            }
            // Self Hosting
            //if (requestMessage.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            //{
            //    RemoteEndpointMessageProperty property =
            //        (RemoteEndpointMessageProperty)requestMessage
            //            .Properties[RemoteEndpointMessageProperty.Name];
            //    return property != null ? property.Address : null;
            //}
            return null;
        }
    }
}
