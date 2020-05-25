using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ResponseModel
/// </summary>
public class ResponseModel
{
    private String mResponseText;
    private int mStatusCode;

    public string ResponseText
    {
        get { return mResponseText; }
        set { mResponseText = value; }
    }
    public int StatusCode
    {
        get { return mStatusCode; }
        set { mStatusCode = value; }
    }
}