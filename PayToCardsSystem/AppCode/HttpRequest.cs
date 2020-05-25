using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

public class HttpRequest
{
    public static ResponseModel sendRequest(string url, string method, Dictionary<string, string> postParameters)
    {
        string postData = "";
        ResponseModel responseModel = new ResponseModel();
        try
        {
            if (postParameters != null)
            {
                foreach (string key in postParameters.Keys)
                {
                    postData += HttpUtility.UrlEncode(key) + "="
                          + HttpUtility.UrlEncode(postParameters[key]) + "&";
                }
            }
            postData = postData.Remove(postData.Length - 1);
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);

            byte[] data = Encoding.ASCII.GetBytes(postData);
            myHttpWebRequest.Method = method;
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.KeepAlive = true;
            myHttpWebRequest.AllowWriteStreamBuffering = false;
            myHttpWebRequest.SendChunked = false;
            myHttpWebRequest.ContentLength = data.Length;



            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            // allows for validation of SSL conversations
            //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            //SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            Stream requestStream = myHttpWebRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

            Stream responseStream = myHttpWebResponse.GetResponseStream();

            StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

            string response = myStreamReader.ReadToEnd();

            myStreamReader.Close();
            responseStream.Close();

            myHttpWebResponse.Close();
            responseModel.ResponseText = response;
            responseModel.StatusCode = (int)myHttpWebResponse.StatusCode;
            return responseModel;
        }
        catch (WebException ex)
        {
            if (ex.Response == null)
            {
                throw ex;
            }
            else
            {
                Stream responseStream = ex.Response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);
                string response = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                responseStream.Close();
                responseModel.ResponseText = response;
                responseModel.StatusCode = (int)ex.Status;
                return responseModel;
            }
        }
        catch (Exception ec)
        {
            throw ec;
        }
    }
}