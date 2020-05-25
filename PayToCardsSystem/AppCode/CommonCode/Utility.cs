using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Data.OleDb;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Security.Cryptography;
using System.Text;
using System.Web.Helpers;
using System.Net.Mail;
using System.Net;

namespace PayToCardsSystem.AppCode.CommonCode
{
    public static class Utility
    {
        #region Convert CSV file To Datatabel
        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }

                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    if (rows.Length > 1)
                    {
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i].Trim();
                        }
                        dt.Rows.Add(dr);
                    }
                }

            }


            return dt;
        }
        #endregion

        public static DataTable ConvertXSLXtoDataTable(string strFilePath, string connString)
        {
            OleDbConnection oledbConn = new OleDbConnection(connString);
            DataTable dt = new DataTable();
            try
            {
                oledbConn.Open();
                using (OleDbCommand cmd = new OleDbCommand("SELECT * FROM [TransactionDetails$]", oledbConn))
                {
                    OleDbDataAdapter oleda = new OleDbDataAdapter();
                    oleda.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    oleda.Fill(ds);

                    dt = ds.Tables[0];
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {

                oledbConn.Close();
            }

            return dt;

        }

        public static void ExportToExcel(GridView obj, string fileName)
        {
            GridView GridView1 = obj;
            GridView1.AllowPaging = false;
            GridView1.DataBind();

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xls");
            HttpContext.Current.Response.Charset = "";
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel ";
            StringWriter sw = new StringWriter();
            sw.WriteLine("File generated at: " + System.DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"));
            sw.WriteLine(sw.NewLine);
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            GridView1.RenderControl(hw);

            HttpContext.Current.Response.Output.Write(sw.ToString());
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        public static string MD5Hash(string input)
        {
            try
            {
                StringBuilder hash = new StringBuilder();
                MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
                byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

                for (int i = 0; i < bytes.Length; i++)
                {
                    hash.Append(bytes[i].ToString("x2"));
                }
                return hash.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SendEmail(string to,string subject,string emailBody,string cc)
        {
            try
            {
                using (MailMessage mm = new MailMessage("vmpaytocards@gmail.com", to))
                {
                    mm.Subject = subject;
                    mm.Body = emailBody;
                    //if (model.Attachment.ContentLength > 0)
                    //{
                    //    string fileName = Path.GetFileName(model.Attachment.FileName);
                    //    mm.Attachments.Add(new Attachment(model.Attachment.InputStream, fileName));
                    //}
                    mm.IsBodyHtml = true;
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        NetworkCredential NetworkCred = new NetworkCredential("vmpaytocards@gmail.com", "paytocard@9876");
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }
                }

                //WebMail.SmtpServer = "smtp.gmail.com";
                //WebMail.SmtpUseDefaultCredentials = true;
                ////sending emails with secure protocol  
                //WebMail.EnableSsl = true;
                ////EmailId used to send emails from application  
                
                //WebMail.UserName = "vmpaytocards@gmail.com";
                //WebMail.Password = "paytocard@9876";
                //smtp.Port = 587;
                ////Sender email address.  
                //WebMail.From = "vmpaytocards@gmail.com";


                //WebMail.Send(to: to, subject: subject, body: emailBody, cc: cc, isBodyHtml: true);
                ////Send email  
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string EncryptionAES(string textData, string encryptionKey)
        {
            using (RijndaelManaged obj = new RijndaelManaged())
            {
                obj.Mode = CipherMode.CBC;
                obj.Padding = PaddingMode.PKCS7;
                obj.KeySize = 0x80;
                obj.BlockSize = 0x80;
                byte[] passBytes = Encoding.UTF8.GetBytes(encryptionKey);

                byte[] encryptionByte = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                int len = passBytes.Length;
                if (len > encryptionByte.Length)
                {
                    len = encryptionByte.Length;
                }
                Array.Copy(passBytes, encryptionByte, len);
                obj.Key = encryptionByte;
                obj.IV = encryptionByte;
                ICryptoTransform objTransform = obj.CreateEncryptor();
                byte[] txtDataByte = Encoding.UTF8.GetBytes(textData);
                return Convert.ToBase64String(objTransform.TransformFinalBlock(txtDataByte, 0, txtDataByte.Length));
            }
        }
        public static string DecryptionAES(string encryptedText, string encryptionKey)
        {
            using (RijndaelManaged obj = new RijndaelManaged())
            {
                obj.Mode = CipherMode.CBC;
                obj.Padding = PaddingMode.PKCS7;

                obj.KeySize = 0x80;
                obj.BlockSize = 0x80;
                byte[] encryptedTextByte = Convert.FromBase64String(encryptedText);
                byte[] passBytes = Encoding.UTF8.GetBytes(encryptionKey);
                byte[] EncryptionkeyBytes = new byte[0x10];
                int len = passBytes.Length;
                if (len > EncryptionkeyBytes.Length)
                {
                    len = EncryptionkeyBytes.Length;
                }
                Array.Copy(passBytes, EncryptionkeyBytes, len);
                obj.Key = EncryptionkeyBytes;
                obj.IV = EncryptionkeyBytes;
                byte[] TextByte = obj.CreateDecryptor().TransformFinalBlock(encryptedTextByte, 0, encryptedTextByte.Length);
                return Encoding.UTF8.GetString(TextByte);  //it will return readable string  
            }
        }

    }
}