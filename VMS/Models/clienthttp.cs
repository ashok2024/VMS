using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace VMS.Models
{
    public enum HttpStatus
    {
        Http200 = 0,
        HttpOther,
        HttpTimeOut
    }

    public class clienthttp
    {
        private CredentialCache _credentialCache = null;
        private string strURL = string.Empty;
        private int m_iHttpTimeOut = 5000;
        //type:0-data,1-data over,2-content length
        public delegate void HttpCallback(int type, byte[] buf, int len);
        private HttpCallback m_cb;

        private CredentialCache GetCredentialCache(string sUrl, string strUserName, string strPassword)
        {
            if (_credentialCache == null)
            {
                _credentialCache = new CredentialCache();
                _credentialCache.Add(new Uri(sUrl), "Digest", new NetworkCredential(strUserName, strPassword));
                strURL = sUrl;
            }
            if (strURL != sUrl)
            {
                _credentialCache.Add(new Uri(sUrl), "Digest", new NetworkCredential(strUserName, strPassword));
                strURL = sUrl;
            }

            return _credentialCache;
        }

        public int HttpRequest(string strUserName, string strPassword, string strUrl, string strHttpMethod, string strReq, ref string strRsp)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(strUrl);
            request.Credentials = GetCredentialCache(strUrl, strUserName, strPassword);
            request.Method = strHttpMethod;
            request.Timeout = m_iHttpTimeOut;

            if (strReq.Length > 0)
            {
                byte[] bs = Encoding.ASCII.GetBytes(strReq);

                request.ContentType = "text/json";
                request.ContentLength = bs.Length;

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                }
            }

            try
            {
                WebResponse wr = request.GetResponse();
                strRsp = new StreamReader(wr.GetResponseStream()).ReadToEnd();

                wr.Close();

                return (int)HttpStatus.Http200;
            }
            catch (WebException ex)
            {
                WebResponse wenReq = (HttpWebResponse)ex.Response;
                if (wenReq != null)
                {
                    strRsp = new StreamReader(wenReq.GetResponseStream()).ReadToEnd();
                    wenReq.Close();
                }

                return (int)HttpStatus.HttpOther;
            }
        }

        public int HttpPostData(string strUserName, string strPassword, string strUrl, string fileKeyName, string filePath, NameValueCollection stringDict, ref string strRsp)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(strUrl);
            webRequest.Credentials = GetCredentialCache(strUrl, strUserName, strPassword);
            webRequest.Method = "POST";
            webRequest.Timeout = 50000;

            // boundary
            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
            var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var endBoundary = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

            var memStream = new MemoryStream();

            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            // request
            var stringKeyHeader = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"" + "\r\n\r\n{1}\r\n";
            foreach (byte[] formitembytes in from string key in stringDict.Keys
                                             select string.Format(stringKeyHeader, key, stringDict[key])
                                                into formitem
                                             select Encoding.ASCII.GetBytes(formitem))
            {
                memStream.Write(formitembytes, 0, formitembytes.Length);
            }

            // picture
            const string filePartHeader = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" + "Content-Type: image/jpeg\r\n\r\n";
            var header = string.Format(filePartHeader, fileKeyName, "1.jpg");
            //var headerbytes = Encoding.UTF8.GetBytes(header);
            var headerbytes = Encoding.ASCII.GetBytes(header);

            memStream.Write(beginBoundary, 0, beginBoundary.Length);
            memStream.Write(headerbytes, 0, headerbytes.Length);

            var buffer = new byte[1024];
            int bytesRead; // =0

            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                memStream.Write(buffer, 0, bytesRead);
            }

            //end Boundary
            memStream.Write(endBoundary, 0, endBoundary.Length);
            webRequest.ContentLength = memStream.Length;
            var requestStream = webRequest.GetRequestStream();
            memStream.Position = 0;
            var tempBuffer = new byte[memStream.Length];
            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();

            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            requestStream.Close();

            try
            {
                WebResponse wr = webRequest.GetResponse();
                strRsp = new StreamReader(wr.GetResponseStream()).ReadToEnd();

                wr.Close();
                fileStream.Close();
                return (int)HttpStatus.Http200;
            }
            catch (WebException ex)
            {
                fileStream.Close();
                return (int)HttpStatus.HttpOther;
            }
        }
    }
}