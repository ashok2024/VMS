using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VMS.Middleware;
using VMS.Models;
using VMS.Models.Account;

namespace VMS.Controllers.Account
{
    public class AccountController : BaseController
    {
        VMSDBEntities db = new VMSDBEntities();

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        // GET: Account
        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.Ip = "";
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            ViewBag.Message = "";
            return View();
        }

        [HttpPost, ValidateInput(false)]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel log, string returnUrl)
        {
            try
            {
                string userName = log.UserName;
                string CacheKeyNameUser = userName;
                ApplicationCache applicationCache = new ApplicationCache();
                applicationCache.RemoveMyCachedItem(CacheKeyNameUser);
                AppUser = null;

                UserModel user = new UserModel();               


                if (ModelState.IsValid)
                {
                    var UserData = db.UserTBs.Where(c => c.UserName == log.UserName && c.Password == log.Password).FirstOrDefault();

                    if (UserData != null)
                    {
                        user.UserId = UserData.UserId;
                        user.Name = UserData.FirstName + " " + UserData.LastName;
                        user.FirstName = UserData.FirstName;
                        user.LastName = UserData.LastName;
                        user.Phone = UserData.Phone;
                        user.Email = UserData.Email;
                        user.BirthDate = Convert.ToDateTime(UserData.BirthDate);
                        user.UserName = UserData.UserName;
                        user.Password = UserData.Password;
                        user.Address = UserData.Address;
                        user.UserType = UserData.UserType;

                        Session.Add("LoginUser", user.Name);
                        Session.Add("LoggedUserType", user.UserType);
                        System.Web.HttpContext.Current.Session["SessionId"] = System.Web.HttpContext.Current.Session.SessionID;
                        // Keep User and User Roles in Cache
                        applicationCache = new ApplicationCache();
                        CacheKeyNameUser = user.UserName;
                        applicationCache.AddtoCache(CacheKeyNameUser, user, AppCachePriority.Default, 3600.00);
                        ViewBag.UserId = user.UserId;
                        ViewBag.UserName = user.UserName;

                        if (user.UserType == "Reception")
                        {
                            return RedirectToAction("VisitorDashboard", "VisitorDashboard", new { userId = ViewBag.UserId, userName = ViewBag.UserName });
                        }
                        else if (user.UserType == "Employee")
                        {
                            return RedirectToAction("EmployeeDashboard", "Employee", new { userId = ViewBag.UserId, userName = ViewBag.UserName });
                        }
                        else if (user.UserType == "Admin")
                        {
                            return RedirectToAction("AdminDashboard", "Admin", new { userId = ViewBag.UserId, userName = ViewBag.UserName });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Please enter the valid Credentials");
                    }
                }
                else
                {
                    HttpResponseMessage response = new HttpResponseMessage();
                    // var userdt = response.Content.ReadAsAsync<UserModel>().Result;

                    string responseContent = await response.Content.ReadAsStringAsync();
                    ExceptionResponse exceptionResponse = JsonConvert.DeserializeObject<ExceptionResponse>(responseContent);

                    ModelState.AddModelError("", exceptionResponse.ExceptionMessage);
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
            }

            return View(log);
        }


        public ActionResult LogOut()
        {
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            FormsAuthentication.SignOut();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddHours(-1));
            Response.Cache.SetNoServerCaching();
            Response.Cache.SetNoStore();
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            System.Web.HttpContext.Current.Session.Clear();
            // string userName = System.Web.HttpContext.Current.Request["userName"];
            string CacheKeyNameUser = userName;
            ApplicationCache applicationCache = new ApplicationCache();
            applicationCache.RemoveMyCachedItem(CacheKeyNameUser);
            AppUser = null;
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult DummyLogin()
        {
            ViewBag.Ip = "";
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            ViewBag.Message = "";
            return View();
        }

        [HttpPost, ValidateInput(false)]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DummyLogin(LoginModel log, string returnUrl)
        {
            try
            {
                //api for authorisation - Verify the user
                string url = "http://103.240.91.206:9099/ISAPI/Security/userCheck?format=json";
                string res = string.Empty;
                clienthttp http = new clienthttp();

                int iRet = http.HttpRequest("admin", "Admin@1234", url, "GET", "", ref res);
                if (iRet == (int)HttpStatus.Http200)
                {

                }
                //else
                //{
                //    ModelState.AddModelError("", "Authentication failed");
                //}

                // api for Get added device Information
                //url = "http://103.240.91.206:9099/ISAPI/ContentMgmt/DeviceMgmt/deviceList?format=json";
                //int position = 0; int maxresult = 5;
                //string req = "{\"SearchDescription\":{\"position\":0,\"maxResult\":";
                //req += maxresult;
                //req += ",\"Filter\":{\"key\":\"\",\"protocolType\":[],\"devStatus\":[]}}}";

                //http = new clienthttp();
                //iRet = http.HttpRequest("admin", "Admin@1234", url, "POST", req, ref res);
                //if (iRet == (int)HttpStatus.Http200)
                //{

                //}

                // api for Add device to device Gateway
                //url = "http://103.240.91.206:9099/ISAPI/ContentMgmt/DeviceMgmt/addDevice?format=json";

                //req = "{\"DeviceInList\" : [{\"Device\": {\"protocolType\": \"ehomeVS\",\"EhomeParams\": {\"EhomeID\":\"1234ABCD\",\"EhomeKey\":\"1234567a\"},\"devName\": \"TestingAPI\",\"devType\": \"AccessControl\"}}]}";

                //http = new clienthttp();
                //iRet = http.HttpRequest("admin", "Admin@1234", url, "POST", req, ref res);
                //if (iRet == (int)HttpStatus.Http200)
                //{

                //}

                //api Get a device information
                //string index = "7D657AA1-C6DE-4572-B7EB-2BF544465B0D";
                //url = "http://103.240.91.206:9099/ISAPI/System/deviceInfo?format=json&devIndex=" + index;
                //iRet = http.HttpRequest("admin", "Admin@1234", url, "GET", "", ref res);
                //if (iRet == (int)HttpStatus.Http200)
                //{

                //}

                //api Employees Puch Downloading API                
                //url = "http://103.240.91.206:9099/ISAPI/AccessControl/AcsEvent?format=json&devIndex=" + index;

                //req = "{\"AcsEventSearchDescription\" : {\"searchID\":\"123\",\"searchResultPosition\":0,\"maxResults\":50,\"AcsEventFilter\": {\"major\":0,\"minor\":0,\"startTime\": \"2021-09-04T00:00:00+05:30\",\"endTime\": \"2021-09-04T23:00:00+05:30\"}}}";

                ////req = string.Empty;
                //string strMatchNum = string.Empty;
                //http = new clienthttp();
                //iRet = http.HttpRequest("admin", "Admin@1234", url, "POST", req, ref res);
                //if (iRet == (int)HttpStatus.Http200)
                //{
                //    DataTable dt = new DataTable();
                //    dt.Columns.AddRange(new DataColumn[13] { new DataColumn("major", typeof(string)),
                //    new DataColumn("minor", typeof(string)),
                //    new DataColumn("time",typeof(DateTime)),
                //    new DataColumn("employeeNoString",typeof(string)),
                //    new DataColumn("cardNo",typeof(string)),
                //    new DataColumn("cardReaderNo",typeof(string)),
                //    new DataColumn("doorNo",typeof(string)),
                //    new DataColumn("currentVerifyMode",typeof(string)),
                //    new DataColumn("serialNo",typeof(string)),
                //    new DataColumn("type",typeof(string)),
                //    new DataColumn("mask",typeof(string)),
                //    new DataColumn("name",typeof(string)),
                //    new DataColumn("userType",typeof(string)),});                    

                //    EventSearchRoot dr = JsonConvert.DeserializeObject<EventSearchRoot>(res);
                //    strMatchNum = dr.AcsEventSearchResult.numOfMatches;
                //    if ("0" != strMatchNum)
                //    {
                //        List<EmployeePunchTB> savelist = new List<EmployeePunchTB>();

                //        foreach (var item in dr.AcsEventSearchResult.MatchList)
                //        {
                //            EmployeePunchTB info = new EmployeePunchTB();
                //            info.major = item.major;
                //            info.minor = item.minor;
                //            info.time = Convert.ToDateTime(item.time);
                //            info.employeeNoString = item.employeeNoString;
                //            info.cardNo = item.cardNo;
                //            info.cardReaderNo = item.cardReaderNo;
                //            info.doorNo = item.doorNo;
                //            info.currentVerifyMode = item.currentVerifyMode;
                //            info.serialNo = item.serialNo ;
                //            info.type = item.type;
                //            info.mask = item.mask;
                //            info.name = item.name;
                //            info.userType = item.userType;
                //            savelist.Add(info);
                //          //  dt.Rows.Add(major,minor,Convert.ToDateTime(time),employeeNoString,cardNo,cardReaderNo,doorNo,currentVerifyMode,serialNo,type,mask,name,userType);
                //        }

                //        if(savelist.Count() > 0)
                //        {
                //            using (VMSDBEntities entities = new VMSDBEntities())
                //            {
                //                //Loop and insert records.
                //                foreach (EmployeePunchTB events in savelist)
                //                {
                //                    entities.EmployeePunchTBs.Add(events);
                //                }
                //                int insertedRecords = entities.SaveChanges();
                //                // return Json(insertedRecords);
                //            }
                //        }
                //    }
                //}

                //api for face upload
                //string filePath = System.IO.Path.Combine(Server.MapPath("/Uploads/"), "M+ANIL_1002788.jpg");
                //string filePath = "C:\\Prasad Kotalwar Data\\LMS\\VMS\\VMS_BK\\M+ANIL_1002788.jpg";
                //string strEmployeeID = "705";
                //string strReq = "{ \"FaceInfo\": {\"employeeNo\": \"" + strEmployeeID + "\",\"faceLibType\": \"blackFD\" }}";
                //string strUrl = "http://103.240.91.206:9099/ISAPI/Intelligent/FDLib/FaceDataRecord?format=json&devIndex=7D657AA1-C6DE-4572-B7EB-2BF544465B0D";
                //string strRsp = string.Empty;

                //string fileKeyName = "FaceImage";
                //NameValueCollection stringDict = new NameValueCollection();
                //stringDict.Add("FaceDataRecord", strReq);

                //http = new clienthttp();
                //iRet = http.HttpPostData("admin", "Admin@1234", strUrl, fileKeyName, filePath, stringDict, ref strRsp);
                //if (iRet != (int)HttpStatus.Http200)
                //{
                //    //alert("Upload Face fail");
                //}
                //else
                //{
                //    //MessageBox.Show("Upload Face success");
                //}


                //url = "http://103.240.91.206:9099/ISAPI/Intelligent/FDLib/Count?format=json&devIndex=7D657AA1-C6DE-4572-B7EB-2BF544465B0D";
                //iRet = http.HttpRequest("admin", "Admin@1234", url, "GET", "", ref res);
                //if (iRet == (int)HttpStatus.Http200)
                //{

                //}

                url = "http://103.240.91.206:9099/ISAPI/Intelligent/FDLib/FDSearch?format=json&devIndex=7D657AA1-C6DE-4572-B7EB-2BF544465B0D";
                string rsp = string.Empty;
                string req = "{\"FaceInfoSearchCond\": {\"searchID\": \"abc\",\"searchResultPosition\": 0,\"maxResults\": 30,\"employeeNo\": \"1001\", \"faceLibType\": \"blackFD\"}}";

                http = new clienthttp();

                iRet = http.HttpRequest("admin", "Admin@1234", url, "POST", req, ref rsp);
                if (iRet == (int)HttpStatus.Http200)
                {
                    FaceSearchRoot dr = JsonConvert.DeserializeObject<FaceSearchRoot>(rsp);
                    if ("0" != dr.FaceInfoSearch.totalMatches)
                    {
                        var data = dr.FaceInfoSearch.FaceInfo.FirstOrDefault();

                        var imgurl = data.faceURL;

                        string pic = data.faceURL.Split('?')[1];

                        string Imageurl = @"http://103.240.91.206:9099" + imgurl;

                        string _response = null;
                        string _auth = "Basic";
                        Uri _uri = new Uri("http://localhost:52434/Account/DummyLogin");
                        HttpWebRequest _req = (System.Net.HttpWebRequest)WebRequest.Create(Imageurl);
                        CredentialCache _cc = new CredentialCache();
                        HttpWebResponse _res = default(HttpWebResponse);
                        StreamReader _sr = default(StreamReader);

                        System.Net.WebProxy proxy = new WebProxy("http://103.240.91.206:9099", true);
                        proxy.Credentials = new NetworkCredential("admin", "Admin@1234", Imageurl);
                        _req.Proxy = proxy;
                        _cc.Add(_uri, _auth, new NetworkCredential("admin", "Admin@1234", Imageurl));
                        _req.PreAuthenticate = true;
                        _req.Credentials = _cc.GetCredential(_uri, _auth);
                        var response = _req.GetResponse();

                        var red = response.ResponseUri;


                        //System.IO.StreamReader sr =
                        //                new System.IO.StreamReader(_res.GetResponseStream());

                        ////_sr = new StreamReader(_res.GetResponseStream);
                        //_response = _sr.ReadToEnd();
                        //_sr.Close();

                        using (var client = new WebClient())
                        {
                            try
                            {
                                client.DownloadFile(red, @"E:\a.html");

                            }
                            catch (Exception ex)
                            {
                                while (ex != null)
                                {
                                    Console.WriteLine(ex.Message);
                                    ex = ex.InnerException;
                                }
                            }
                        }



                        string file = System.IO.Path.GetFileName(Imageurl);
                        WebClient cln = new WebClient();
                        cln.DownloadFile(url, file);

                        var _comPath = Server.MapPath("~/faceupload/");

                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(Imageurl, _comPath);


                        using (WebClient _wc = new WebClient())
                        {
                            _wc.DownloadFileAsync(new Uri(Imageurl), Path.Combine(_comPath, Path.GetFileName("pic?" + pic)));
                        }

                        string[] fileEntries = Directory.GetFiles(Server.MapPath("~/faceupload"));
                        foreach (string fileName in fileEntries)
                        {
                            Response.ContentType = "image/jpeg";
                            Response.AppendHeader("Content-Disposition", "attachment; filename=test.gif");
                            Response.TransmitFile(fileName);
                            Response.End();
                        }
                    }
                }

                string userName = log.UserName;
                string CacheKeyNameUser = userName;
                ApplicationCache applicationCache = new ApplicationCache();
                applicationCache.RemoveMyCachedItem(CacheKeyNameUser);
                AppUser = null;

                UserModel user = new UserModel();

                if (ModelState.IsValid)
                {
                    var UserData = db.UserTBs.Where(c => c.UserName == log.UserName && c.Password == log.Password).FirstOrDefault();

                    if (UserData != null)
                    {
                        user.UserId = UserData.UserId;
                        user.Name = UserData.FirstName + " " + UserData.LastName;
                        user.FirstName = UserData.FirstName;
                        user.LastName = UserData.LastName;
                        user.Phone = UserData.Phone;
                        user.Email = UserData.Email;
                        user.BirthDate = Convert.ToDateTime(UserData.BirthDate);
                        user.UserName = UserData.UserName;
                        user.Password = UserData.Password;
                        user.Address = UserData.Address;
                        user.UserType = UserData.UserType;

                        Session.Add("LoginUser", user.Name);
                        Session.Add("LoggedUserType", user.UserType);
                        System.Web.HttpContext.Current.Session["SessionId"] = System.Web.HttpContext.Current.Session.SessionID;
                        // Keep User and User Roles in Cache
                        applicationCache = new ApplicationCache();
                        CacheKeyNameUser = user.UserName;
                        applicationCache.AddtoCache(CacheKeyNameUser, user, AppCachePriority.Default, 3600.00);
                        ViewBag.UserId = user.UserId;
                        ViewBag.UserName = user.UserName;

                        if (user.UserType == "Reception")
                        {
                            return RedirectToAction("VisitorDashboard", "VisitorDashboard", new { userId = ViewBag.UserId, userName = ViewBag.UserName });
                        }
                        else if (user.UserType == "Employee")
                        {
                            return RedirectToAction("EmployeeDashboard", "Employee", new { userId = ViewBag.UserId, userName = ViewBag.UserName });
                        }
                        else if (user.UserType == "Admin")
                        {
                            return RedirectToAction("AdminDashboard", "Admin", new { userId = ViewBag.UserId, userName = ViewBag.UserName });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Please enter the valid Credentials");
                    }
                }
                else
                {
                    HttpResponseMessage response = new HttpResponseMessage();
                    // var userdt = response.Content.ReadAsAsync<UserModel>().Result;

                    string responseContent = await response.Content.ReadAsStringAsync();
                    ExceptionResponse exceptionResponse = JsonConvert.DeserializeObject<ExceptionResponse>(responseContent);

                    ModelState.AddModelError("", exceptionResponse.ExceptionMessage);
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
            }

            return View(log);
        }

    }
}