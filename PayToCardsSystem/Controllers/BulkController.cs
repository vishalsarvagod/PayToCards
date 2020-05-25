using PayToCardsSystem;
using PayToCardsSystem.AppCode.CommonCode;
using PayToCardsSystem.AppCode.DAO;
using PayToCardsSystem.Controllers;
using PayToCardsSystem.DTO;
using PayToCardsSystem.DTO.Request;
using PayToCardsSystem.Models;
using PayToCardsSystem.Services;
using PayToCardsSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Controllers
{
    public class BulkController : Controller
    {
        //  List<BulkTransactionDTO> dataList = new List<BulkTransactionDTO>();
        BulkTransactionDTOViewModel BulkViewModel = new BulkTransactionDTOViewModel();
        // GET: Bulk
        public ActionResult Index()
        {
            if ((user_personal_details)(Session["UserPersonalDetails"]) != null)
            {

                return View(BulkViewModel);
                //  return View(dataList);
            }
            else
                return RedirectToAction("Index", "Login");
        }
        [HttpGet]
        public ActionResult UploadFile()
        {
            if ((user_personal_details)(Session["UserPersonalDetails"]) != null)
                return View();
            else
                return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        [SubmitButtonSelector(Name = "Submit")]
        public ActionResult Index(HttpPostedFileBase file)
        {
            DataTable dtErr;
            ViewBag.Error = null;
            //List<BulkTransactionDTO> dataList = new List<BulkTransactionDTO>();
            BulkTransactionDTOViewModel bulkViewModel = new BulkTransactionDTOViewModel();
            bulkViewModel.IsComplete = true;
            try
            {
                if ((user_personal_details)(Session["UserPersonalDetails"]) != null)
                {
                    if (file.ContentLength > 0)
                    {
                        string strFileName = Path.GetFileName(file.FileName);
                        string extension = System.IO.Path.GetExtension(strFileName).ToLower();
                        string query = null;
                        string connString = "";
                        string[] validFileTypes = { ".xls", ".xlsx", ".csv" };

                        string path1 = Path.Combine(Server.MapPath("~/UploadedFiles"), strFileName);
                        if (validFileTypes.Contains(extension))
                        {
                            if (System.IO.File.Exists(path1))
                            {
                                System.IO.File.Delete(path1);
                            }

                            file.SaveAs(path1);
                            if (extension.Trim() == ".xls")
                            {
                                connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                                DataTable dt = Utility.ConvertXSLXtoDataTable(path1, connString);

                                if (dt.Rows.Count > 0)
                                {

                                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[i].ItemArray[0].ToString()))
                                        {
                                            {
                                                TransationData bulkTransaction = new TransationData
                                                {

                                                    CardNumber = dt.Rows[i].ItemArray[0].ToString(),
                                                    ExpiryDate = dt.Rows[i].ItemArray[1].ToString(),
                                                    Amount = Convert.ToDouble(dt.Rows[i].ItemArray[2].ToString()),
                                                    Currency = dt.Rows[i].ItemArray[3].ToString(),
                                                    Descriptor = dt.Rows[i].ItemArray[4].ToString(),
                                                    Comments = dt.Rows[i].ItemArray[5].ToString(),
                                                    Name = dt.Rows[i].ItemArray[6].ToString(),
                                                    ContactNo = dt.Rows[i].ItemArray[7].ToString()

                                                };
                                                //  dataList.Add(bulkTransaction);
                                                bulkViewModel.ListBulkTransactionDTO.Add(bulkTransaction);
                                            }
                                        }
                                    }
                                    //dtErr = readDT(dt);
                                    //ViewBag.Data = dtErr;
                                    //if (dtErr.Rows.Count > 0)
                                    //    ViewBag.Error = "Following entries have errors. Please review them";
                                }
                                else
                                {
                                    ViewBag.Error = "Excel Row Count zero";
                                }
                            }
                            else if (extension.Trim() == ".xlsx")
                            {
                                //connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                                //DataTable dt = Utility.ConvertXSLXtoDataTable(path1, connString);

                                //if (dt.Rows.Count > 0)
                                //{
                                //    dtErr = readDT(dt);
                                //    ViewBag.Data = dtErr;
                                //    if (dtErr.Rows.Count > 0)
                                //        ViewBag.Error = "Following entries have errors. Please review them";
                                //}
                                //else
                                //{
                                //    ViewBag.Error = "Excel Row Count zero";
                                //}
                            }
                        }
                        else
                        {
                            ViewBag.Error = "Please Upload Files in .xls, .xlsx or .csv format";
                        }
                    }
                    ViewBag.Message = "File Uploaded Successfully!!";
                    //   return View(dataList);
                    return View(bulkViewModel);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "File upload failed!! " + ex.Message;
                bulkViewModel.IsComplete = false;
                //   return View(dataList);
                return View(bulkViewModel);
            }
        }
        private DataTable getErrorDT()
        {
            DataTable dtErr = new DataTable();
            dtErr.Columns.Add("CardNumber");
            dtErr.Columns.Add("ExpiryDate");
            dtErr.Columns.Add("Amount");
            dtErr.Columns.Add("Currency");
            dtErr.Columns.Add("Message1");
            dtErr.Columns.Add("Message2");
            dtErr.Columns.Add("HolderName");
            dtErr.Columns.Add("ContactNo");
            dtErr.Columns.Add("Error");
            dtErr.AcceptChanges();
            return dtErr;
        }
        //public DataTable readDT(DataTable dt)
        //{
        //    DataTable dtErr = getErrorDT();
        //    TransactionModels transModel = new TransactionModels();
        //    vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
        //    user_personal_details userDetails = (user_personal_details)(Session["UserPersonalDetails"]);
        //    int userId = userDetails.C_id;
        //    List<userFeeModel> listFee = new List<userFeeModel>();
        //    var list = (from fee in dbContext.user_fee_details
        //                from cur in dbContext.currencies
        //                where fee.CurrencyId == cur.C_id && fee.UserId == userId
        //                orderby cur.ShortCode
        //                select new { cur.ShortCode, fee.FlatFee, fee.PercentFee });
        //    foreach (var element in list)
        //    {
        //        listFee.Add(new userFeeModel
        //        {
        //            Currency = element.ShortCode,
        //            FlatFee = element.FlatFee,
        //            PercentFee = element.PercentFee
        //        });
        //    }
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        if ((listFee.Where(z => z.Currency == dt.Rows[i].ItemArray[(int)ColIndex.Currency].ToString())).Count() > 0)
        //        {
        //            transModel.CardNumber = dt.Rows[i].ItemArray[(int)ColIndex.Cardnumber].ToString();
        //            if (validateDate(dt.Rows[i].ItemArray[(int)ColIndex.ExpireDate].ToString()))
        //            {
        //                transModel.ExpireDate = Convert.ToDateTime(dt.Rows[i].ItemArray[(int)ColIndex.ExpireDate].ToString().Substring(0, 3) + "01/" + dt.Rows[i].ItemArray[(int)ColIndex.ExpireDate].ToString().Substring(3));
        //                if (validateAmount(dt.Rows[i].ItemArray[(int)ColIndex.TransactionAmount].ToString()))
        //                {
        //                    transModel.TransactionAmount = Convert.ToDouble(dt.Rows[i].ItemArray[(int)ColIndex.TransactionAmount].ToString());
        //                    transModel.Currency = dt.Rows[i].ItemArray[(int)ColIndex.Currency].ToString();
        //                    transModel.Message1 = dt.Rows[i].ItemArray[(int)ColIndex.Message1].ToString();
        //                    transModel.Message2 = dt.Rows[i].ItemArray[(int)ColIndex.Message2].ToString();
        //                    transModel.CardHolderName = dt.Rows[i].ItemArray[(int)ColIndex.CardHolderName].ToString();
        //                    transModel.ContactNo = dt.Rows[i].ItemArray[(int)ColIndex.ContactNo].ToString();
        //                    transModel.UserId = userId;
        //                    string strRet = validate(transModel);
        //                    if (strRet.Equals(""))
        //                    {
        //                        double transactionFee = 0;
        //                        user_currency_details userCur = dbContext.user_currency_details.FirstOrDefault(cur => cur.UserId == userId && cur.Currency == transModel.Currency);
        //                        user_fee_details userFeeDetail = dbContext.user_fee_details.FirstOrDefault(fee => fee.UserId == userId && fee.CurrencyId == userCur.CurrencyId);
        //                        double percentFee = 0;
        //                        percentFee = transModel.TransactionAmount * userFeeDetail.PercentFee / 100;
        //                        if (userFeeDetail.FeeType.Equals("AND"))
        //                        {
        //                            transactionFee = percentFee + userFeeDetail.FlatFee;
        //                        }
        //                        else
        //                        {
        //                            if (percentFee > userFeeDetail.FlatFee)
        //                                transactionFee = percentFee;
        //                            else
        //                                transactionFee = userFeeDetail.FlatFee;
        //                        }
        //                        if (userCur.Balance >= (transModel.TransactionAmount + transactionFee))
        //                        {
        //                            TransactionService transService = new TransactionService();
        //                            TransactionDTO transDTO = transService.saveTranWithApi(transModel, transactionFee, userDetails, userCur.CurrencyId);
        //                            if (transDTO != null)
        //                            {
        //                                CardPaymentResponse cardResponse = transDTO.cardPaymentReposne;
        //                                if (!transService.isSwishSuccessCode(cardResponse.result.code.ToString()))
        //                                {
        //                                    DataRow dr = dtErr.NewRow();
        //                                    dr[0] = dt.Rows[i].ItemArray[(int)ColIndex.Cardnumber].ToString();
        //                                    dr[1] = dt.Rows[i].ItemArray[(int)ColIndex.ExpireDate].ToString();
        //                                    dr[2] = dt.Rows[i].ItemArray[(int)ColIndex.TransactionAmount].ToString();
        //                                    dr[3] = dt.Rows[i].ItemArray[(int)ColIndex.Currency].ToString();
        //                                    dr[4] = dt.Rows[i].ItemArray[(int)ColIndex.Message1].ToString();
        //                                    dr[5] = dt.Rows[i].ItemArray[(int)ColIndex.Message2].ToString();
        //                                    dr[6] = dt.Rows[i].ItemArray[(int)ColIndex.CardHolderName].ToString();
        //                                    dr[7] = dt.Rows[i].ItemArray[(int)ColIndex.ContactNo].ToString();
        //                                    dr[8] = cardResponse.result.description.ToString();
        //                                    dtErr.Rows.Add(dr);
        //                                    dtErr.AcceptChanges();
        //                                }
        //                            }
        //                            else
        //                            {
        //                                DataRow dr = dtErr.NewRow();
        //                                dr[0] = dt.Rows[i].ItemArray[(int)ColIndex.Cardnumber].ToString();
        //                                dr[1] = dt.Rows[i].ItemArray[(int)ColIndex.ExpireDate].ToString();
        //                                dr[2] = dt.Rows[i].ItemArray[(int)ColIndex.TransactionAmount].ToString();
        //                                dr[3] = dt.Rows[i].ItemArray[(int)ColIndex.Currency].ToString();
        //                                dr[4] = dt.Rows[i].ItemArray[(int)ColIndex.Message1].ToString();
        //                                dr[5] = dt.Rows[i].ItemArray[(int)ColIndex.Message2].ToString();
        //                                dr[6] = dt.Rows[i].ItemArray[(int)ColIndex.CardHolderName].ToString();
        //                                dr[7] = dt.Rows[i].ItemArray[(int)ColIndex.ContactNo].ToString();
        //                                dr[8] = "Unable to Connect Server";
        //                                dtErr.Rows.Add(dr);
        //                                dtErr.AcceptChanges();
        //                            }
        //                        }
        //                        else
        //                        {
        //                            DataRow dr = dtErr.NewRow();
        //                            dr[0] = dt.Rows[i].ItemArray[(int)ColIndex.Cardnumber].ToString();
        //                            dr[1] = dt.Rows[i].ItemArray[(int)ColIndex.ExpireDate].ToString();
        //                            dr[2] = dt.Rows[i].ItemArray[(int)ColIndex.TransactionAmount].ToString();
        //                            dr[3] = dt.Rows[i].ItemArray[(int)ColIndex.Currency].ToString();
        //                            dr[4] = dt.Rows[i].ItemArray[(int)ColIndex.Message1].ToString();
        //                            dr[5] = dt.Rows[i].ItemArray[(int)ColIndex.Message2].ToString();
        //                            dr[6] = dt.Rows[i].ItemArray[(int)ColIndex.CardHolderName].ToString();
        //                            dr[7] = dt.Rows[i].ItemArray[(int)ColIndex.ContactNo].ToString();
        //                            dr[8] = "Out of Balance";
        //                            dtErr.Rows.Add(dr);
        //                            dtErr.AcceptChanges();
        //                        }
        //                    }
        //                    else
        //                    {
        //                        DataRow dr = dtErr.NewRow();
        //                        dr[0] = dt.Rows[i].ItemArray[(int)ColIndex.Cardnumber].ToString();
        //                        dr[1] = dt.Rows[i].ItemArray[(int)ColIndex.ExpireDate].ToString();
        //                        dr[2] = dt.Rows[i].ItemArray[(int)ColIndex.TransactionAmount].ToString();
        //                        dr[3] = dt.Rows[i].ItemArray[(int)ColIndex.Currency].ToString();
        //                        dr[4] = dt.Rows[i].ItemArray[(int)ColIndex.Message1].ToString();
        //                        dr[5] = dt.Rows[i].ItemArray[(int)ColIndex.Message2].ToString();
        //                        dr[6] = dt.Rows[i].ItemArray[(int)ColIndex.CardHolderName].ToString();
        //                        dr[7] = dt.Rows[i].ItemArray[(int)ColIndex.ContactNo].ToString();
        //                        dr[8] = strRet;
        //                        dtErr.Rows.Add(dr);
        //                        dtErr.AcceptChanges();
        //                    }
        //                }
        //                else
        //                {
        //                    DataRow dr = dtErr.NewRow();
        //                    dr[0] = dt.Rows[i].ItemArray[(int)ColIndex.Cardnumber].ToString();
        //                    dr[1] = dt.Rows[i].ItemArray[(int)ColIndex.ExpireDate].ToString();
        //                    dr[2] = dt.Rows[i].ItemArray[(int)ColIndex.TransactionAmount].ToString();
        //                    dr[3] = dt.Rows[i].ItemArray[(int)ColIndex.Currency].ToString();
        //                    dr[4] = dt.Rows[i].ItemArray[(int)ColIndex.Message1].ToString();
        //                    dr[5] = dt.Rows[i].ItemArray[(int)ColIndex.Message2].ToString();
        //                    dr[6] = dt.Rows[i].ItemArray[(int)ColIndex.CardHolderName].ToString();
        //                    dr[7] = dt.Rows[i].ItemArray[(int)ColIndex.ContactNo].ToString();
        //                    dr[8] = "IncorrectAmount";
        //                    dtErr.Rows.Add(dr);
        //                    dtErr.AcceptChanges();
        //                }
        //            }
        //            else
        //            {
        //                DataRow dr = dtErr.NewRow();
        //                dr[0] = dt.Rows[i].ItemArray[(int)ColIndex.Cardnumber].ToString();
        //                dr[1] = dt.Rows[i].ItemArray[(int)ColIndex.ExpireDate].ToString();
        //                dr[2] = dt.Rows[i].ItemArray[(int)ColIndex.TransactionAmount].ToString();
        //                dr[3] = dt.Rows[i].ItemArray[(int)ColIndex.Currency].ToString();
        //                dr[4] = dt.Rows[i].ItemArray[(int)ColIndex.Message1].ToString();
        //                dr[5] = dt.Rows[i].ItemArray[(int)ColIndex.Message2].ToString();
        //                dr[6] = dt.Rows[i].ItemArray[(int)ColIndex.CardHolderName].ToString();
        //                dr[7] = dt.Rows[i].ItemArray[(int)ColIndex.ContactNo].ToString();
        //                dr[8] = "Incorrect Expiry Date";
        //                dtErr.Rows.Add(dr);
        //                dtErr.AcceptChanges();
        //            }
        //        }
        //        else
        //        {
        //            DataRow dr = dtErr.NewRow();
        //            dr[0] = dt.Rows[i].ItemArray[(int)ColIndex.Cardnumber].ToString();
        //            dr[1] = dt.Rows[i].ItemArray[(int)ColIndex.ExpireDate].ToString();
        //            dr[2] = dt.Rows[i].ItemArray[(int)ColIndex.TransactionAmount].ToString();
        //            dr[3] = dt.Rows[i].ItemArray[(int)ColIndex.Currency].ToString();
        //            dr[4] = dt.Rows[i].ItemArray[(int)ColIndex.Message1].ToString();
        //            dr[5] = dt.Rows[i].ItemArray[(int)ColIndex.Message2].ToString();
        //            dr[6] = dt.Rows[i].ItemArray[(int)ColIndex.CardHolderName].ToString();
        //            dr[7] = dt.Rows[i].ItemArray[(int)ColIndex.ContactNo].ToString();
        //            dr[8] = "Unsupported Currency";
        //            dtErr.Rows.Add(dr);
        //            dtErr.AcceptChanges();
        //        }
        //    }
        //    return dtErr;
        //}


        private enum ColIndex
        {
            Cardnumber = 0,
            ExpireDate = 1,
            TransactionAmount = 2,
            Currency = 3,
            Message1 = 4,
            Message2 = 5,
            CardHolderName = 6,
            ContactNo = 7
        }

        [HttpPost]
        [SubmitButtonSelector(Name = "Save")]
        public ActionResult Save(BulkTransactionDTOViewModel bulkModel)
        {
            try
            {
                TransactionService transService = new TransactionService();
                bulkModel.IsSubmit = true;
                bulkModel.IsComplete = true;
                ModelState.Clear();

                vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
                user_personal_details userDetails = (user_personal_details)(Session["UserPersonalDetails"]);
               


                bulkModel.ListBulkTransactionDTO = transService.SaveTransaction(bulkModel.ListBulkTransactionDTO, userDetails);
                ViewBag.Message = "Data Saved Successfully!!";
                //return PartialView("balances", bulkModel.ListBulkTransactionDTO);
                return View(bulkModel);
            }catch(Exception ex)
            {
                ViewBag.Message = "Unable to connect Server";
                return View(bulkModel);
            }
        }
    }
}