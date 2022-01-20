using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using VMS.Controllers.Account;
using VMS.Middleware;
using VMS.Models;
using VMS.Models.Admin;

namespace VMS.Controllers.Admin
{
    public class SettingController : BaseController
    {
        List<MailSettingModel> mails;
        List<DeviceModel> devices;
        List<EventInfo> Deviceloglist;
        public SettingController()
        {
            mails = new List<MailSettingModel>();
            devices = new List<DeviceModel>();
            Deviceloglist = new List<EventInfo>();
        }

        // GET: Setting
        public ActionResult Index()
        {
            return View();
        }

        #region Mail
        public ActionResult GetMailSetting()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "MailList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewData["GetMailList"] = mails = GetMailSettings();
                return View();
            }
        }

        private static List<MailSettingModel> GetMailSettings()
        {
            List<MailSettingModel> Model = new List<MailSettingModel>();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var data = entities.MailSettingTBs.ToList().OrderByDescending(d => d.Id);

                foreach (var item in data)
                {
                    MailSettingModel courier = new MailSettingModel();
                    courier.Id = item.Id;
                    courier.Host = item.Host;
                    courier.smtpfrom = item.smtpfrom;
                    courier.port = Convert.ToString(item.port);
                    courier.username = item.username;
                    courier.password = item.password;
                    Model.Add(courier);
                }
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        public ActionResult AddMailSetting(int Id)
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "MailList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                MailSettingModel mail = new MailSettingModel();
                if (Id > 0)
                {
                    mail = GetMailDetails(Id);
                }

                return View("AddMailSetting", mail);
            }
        }

        private static MailSettingModel GetMailDetails(int Id)
        {
            MailSettingModel Model = new MailSettingModel();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var data = entities.MailSettingTBs.Where(d => d.Id == Id).FirstOrDefault();

                Model.Id = data.Id;
                Model.Host = data.Host;
                Model.smtpfrom = data.smtpfrom;
                Model.port = Convert.ToString(data.port);
                Model.username = data.username;
                Model.password = data.password;
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        [HttpPost]
        public async Task<ActionResult> Save(MailSettingModel mail)
        {

            VMSDBEntities db = new VMSDBEntities();

            string userid = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            string username = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserId = userid;
            ViewBag.UserName = username;

            ViewBag.ActivePage = "MailList";

            //string sessionid = (System.Web.HttpContext.Current.Session["SessionId"] != null) ? System.Web.HttpContext.Current.Session["SessionId"].ToString() : String.Empty;
            //if (string.IsNullOrEmpty(sessionid))
            //{
            //    return RedirectToAction("LogOut", "Account");
            //}
            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (mail.Id > 0)
                    {
                        var check = db.MailSettingTBs.Where(d => d.Id == mail.Id).FirstOrDefault();

                        check.Host = mail.Host;
                        check.smtpfrom = mail.smtpfrom;
                        check.port = Convert.ToInt32(mail.port);
                        check.username = mail.username;
                        check.password = mail.password;
                        db.Entry(check).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        MailSettingTB tB = new MailSettingTB();
                        tB.Host = mail.Host;
                        tB.smtpfrom = mail.smtpfrom;
                        tB.port = Convert.ToInt32(mail.port);
                        tB.username = mail.username;
                        tB.password = mail.password;
                        db.MailSettingTBs.Add(tB);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return View("AddMailSetting", mail);
            }
            return RedirectToAction("GetMailSetting", "Setting", new { userId = ViewBag.UserId, userName = ViewBag.UserName });
        }

        #endregion

        #region Device 

        public ActionResult GetDeviceList()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "DeviceList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewBag.DeviceList = new SelectList(GetDevices(), "DeviceId", "DeviceName");
                ViewData["GetDeviceList"] = devices = GetDevices();
                return View();
            }
        }
        [HttpPost]
        public JsonResult A_GetDeviceList()
        {
            List<DeviceModel> Model = new List<DeviceModel>();
            Model = GetDevices();
            return Json(Model);
        }

        public List<DeviceModel> GetDevices()
        {
            List<DeviceModel> Model = new List<DeviceModel>();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var data = entities.DevicesTBs.ToList().OrderByDescending(d => d.DeviceId);

                foreach (var item in data)
                {
                    DeviceModel courier = new DeviceModel();
                    courier.DeviceId = item.DeviceId;
                    courier.DeviceName = item.DeviceName;
                    courier.DeviceSerialNo = item.DeviceSerialNo;
                    courier.DeviceAccountId = item.DeviceAccountId;
                    courier.DeviceStatus = item.DeviceStatus;
                    courier.DeviceLocation = item.DeviceLocation;
                    courier.DeviceIPAddress = item.DeviceIPAddress;
                    courier.Port = item.Port;
                    courier.UserName = item.UserName;
                    courier.Password = item.Password;

                    string url = "http://" + item.DeviceIPAddress + ":" + item.Port + "/ISAPI/ContentMgmt/DeviceMgmt/deviceList?format=json";

                    string req = "{\"SearchDescription\" : {\"position\":0,\"maxResult\":5}}";

                    //req = string.Empty;
                    string reps = string.Empty;
                    string strMatchNum = string.Empty;
                    clienthttp clnt = new clienthttp();
                    int iet = clnt.HttpRequest(item.UserName, item.Password, url, "POST", req, ref reps);
                    if (iet == (int)HttpStatus.Http200)
                    {
                        DeviceSearchRoot dr = JsonConvert.DeserializeObject<DeviceSearchRoot>(reps);
                        strMatchNum = Convert.ToString(dr.SearchResult.numOfMatches);

                        if ("0" != strMatchNum)
                        {
                            foreach (var dev in dr.SearchResult.MatchList)
                            {
                                if (dev.Device.EhomeParams.EhomeID == item.DeviceAccountId)
                                {
                                    var devicedt = dev.Device;
                                    courier.DeviceStatus = devicedt.devStatus;
                                }
                            }
                        }
                    }
                    else
                    {
                        courier.DeviceStatus = "Not Available";
                    }


                    Model.Add(courier);
                }
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        public ActionResult AddDevice(int DeviceId)
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "DeviceList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                DeviceModel device = new DeviceModel();
                if (DeviceId > 0)
                {
                    device = GetDeviceModelDetails(DeviceId);
                }

                return View("AddDevice", device);
            }
        }

        private static DeviceModel GetDeviceModelDetails(int Id)
        {
            DeviceModel Model = new DeviceModel();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var item = entities.DevicesTBs.Where(d => d.DeviceId == Id).FirstOrDefault();

                Model.DeviceId = item.DeviceId;
                Model.DeviceName = item.DeviceName;
                Model.DeviceSerialNo = item.DeviceSerialNo;
                Model.DeviceAccountId = item.DeviceAccountId;
                Model.DeviceStatus = item.DeviceStatus;
                Model.DeviceLocation = item.DeviceLocation;
                Model.DeviceIPAddress = item.DeviceIPAddress;
                Model.Port = item.Port;
                Model.UserName = item.UserName;
                Model.Password = item.Password;
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        [HttpPost]
        public async Task<ActionResult> SaveDevice(DeviceModel device)
        {

            VMSDBEntities db = new VMSDBEntities();

            string userid = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            string username = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserId = userid;
            ViewBag.UserName = username;

            ViewBag.ActivePage = "DeviceList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }

            if (device.DeviceId > 0)
            {
                ModelState.Remove("Port");
                ModelState.Remove("Password");
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (device.DeviceId > 0)
                    {
                        var check = db.DevicesTBs.Where(d => d.DeviceId == device.DeviceId).FirstOrDefault();

                        check.DeviceName = device.DeviceName;
                        check.DeviceSerialNo = device.DeviceSerialNo;
                        check.DeviceLocation = device.DeviceLocation;
                        check.DeviceAccountId = device.DeviceAccountId;
                        check.DeviceStatus = device.DeviceStatus;
                        check.DeviceIPAddress = device.DeviceIPAddress;
                        // check.Port = device.Port;
                        check.UserName = device.UserName;
                        // check.Password = device.Password;
                        db.Entry(check).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        DevicesTB tB = new DevicesTB();
                        tB.DeviceName = device.DeviceName;
                        tB.DeviceSerialNo = device.DeviceSerialNo;
                        tB.DeviceLocation = device.DeviceLocation;
                        tB.DeviceAccountId = device.DeviceAccountId;
                        tB.DeviceStatus = device.DeviceStatus;
                        tB.DeviceIPAddress = device.DeviceIPAddress;
                        tB.Port = device.Port;
                        tB.UserName = device.UserName;
                        tB.Password = device.Password;
                        db.DevicesTBs.Add(tB);
                        db.SaveChanges();
                    }

                    #region add device api

                    // api for Add device to device Gateway

                    string url = "http://" + device.DeviceIPAddress + ":" + device.Port + "/ISAPI/ContentMgmt/DeviceMgmt/deviceList?format=json";

                    string req = "{\"deviceinlist\" : [{\"device\": {\"protocoltype\": \"ehomevs\",\"ehomeparams\": {\"ehomeid\":\"1234abcd\",\"ehomekey\":\"1234567a\"},\"devname\": \"testingapi\",\"devtype\": \"accesscontrol\"}}]}";
                    string reps = string.Empty;

                    clienthttp clnt = new clienthttp();                    
                    int iet = clnt.HttpRequest(device.UserName, device.Password, url, "POST", req, ref reps);
                    if (iet == (int)HttpStatus.Http200)
                    {
                        ApiMonitorTB at = new ApiMonitorTB();
                        at.Action = "Add Device";
                        at.Page = "Device";
                        at.time = DateTime.Now;
                        db.ApiMonitorTBs.Add(at);
                        db.SaveChanges();
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return View("AddDevice", device);
            }
            //return RedirectToAction("GetDeviceList", "Setting", new { userId = ViewBag.UserId, userName = ViewBag.UserName });
            ViewBag.Result = "Success";
            return View("AddDevice", device);
        }

        public ActionResult OpenCloseDevice(int DeviceId, string type)
        {
            VMSDBEntities db = new VMSDBEntities();

            bool res = false;

            var device = db.DevicesTBs.Where(d => d.DeviceId == DeviceId).FirstOrDefault();

            if (device != null)
            {
                string url = "http://" + device.DeviceIPAddress + ":" + device.Port + "/ISAPI/ContentMgmt/DeviceMgmt/deviceList?format=json";

                string req = "{\"SearchDescription\" : {\"position\":0,\"maxResult\":5}}";

                //req = string.Empty;
                string reps = string.Empty;
                string strMatchNum = string.Empty;
                clienthttp clnt = new clienthttp();
                int iet = clnt.HttpRequest(device.UserName, device.Password, url, "POST", req, ref reps);
                if (iet == (int)HttpStatus.Http200)
                {
                    DeviceSearchRoot dr = JsonConvert.DeserializeObject<DeviceSearchRoot>(reps);
                    strMatchNum = Convert.ToString(dr.SearchResult.numOfMatches);

                    if ("0" != strMatchNum)
                    {
                        foreach (var item in dr.SearchResult.MatchList)
                        {
                            if (item.Device.EhomeParams.EhomeID == device.DeviceAccountId)
                            {
                                var devicedt = item.Device;

                                string index = devicedt.devIndex;
                                string strReq = "{\"RemoteControlDoor\": {\"cmd\":\"" + type + "\"}}";
                                string strUrl = "http://" + device.DeviceIPAddress + ":" + device.Port + "/ISAPI/AccessControl/RemoteControl/door/1?format=json&devIndex=" + index;
                                string strRsp = string.Empty;

                                clienthttp http = new clienthttp();
                                int iRet = http.HttpRequest(device.UserName, device.Password, strUrl, "PUT", strReq, ref strRsp);
                                if (iRet == (int)HttpStatus.Http200)
                                {
                                    res = true;
                                }
                            }
                        }
                    }
                }
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region device log

        public ActionResult GetDevliceLog()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "DeviceLogList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewBag.DeviceList = new SelectList(GetDevices(), "DeviceId", "DeviceName");


                DeviceLogSearchModel deviceLog = new DeviceLogSearchModel();
                deviceLog.fromdate = deviceLog.todate = DateTime.Now;
                deviceLog.DeviceList = new List<SelectListItem>();
                devices = GetDevices();

                foreach (var item in devices)
                {
                    SelectListItem se = new SelectListItem();
                    se.Value = item.DeviceId.ToString();
                    se.Text = item.DeviceName;
                    deviceLog.DeviceList.Add(se);
                }

                return View("GetDevliceLog", deviceLog);
            }
        }

        public ActionResult GetLogData(DeviceLogSearchModel deviceLog)
        {
            VMSDBEntities db = new VMSDBEntities();
            List<EmployeePunchModel> list = new List<EmployeePunchModel>();

            list = GetDeviceLog();

            var device = db.DevicesTBs.Where(d => d.DeviceId == deviceLog.DeviceId).FirstOrDefault();

            list = list.Where(d => d.deviceAccountId == device.DeviceAccountId && d.time.Date >= Convert.ToDateTime(deviceLog.fromdate) && d.time.Date <= Convert.ToDateTime(deviceLog.todate)).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private static List<EmployeePunchModel> GetDeviceLog()
        {
            List<EmployeePunchModel> Model = new List<EmployeePunchModel>();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var data = entities.EmployeePunchTBs.ToList().OrderBy(d => d.Id);

                foreach (var item in data)
                {
                    EmployeePunchModel courier = new EmployeePunchModel();
                    courier.major = item.major;
                    courier.minor = item.minor;
                    courier.time = Convert.ToDateTime(item.time);
                    courier.Atime = Convert.ToDateTime(item.time).ToString("hh:mm:ss");
                    courier.ADate = Convert.ToDateTime(item.time).ToString("dd/MM/yy");
                    courier.employeeNoString = item.employeeNoString != null ? item.employeeNoString : "";
                    courier.cardNo = item.cardNo != null ? item.cardNo : "";
                    courier.cardReaderNo = item.cardReaderNo != null ? item.cardReaderNo : "";
                    courier.doorNo = item.doorNo != null ? item.doorNo : "";
                    courier.currentVerifyMode = item.currentVerifyMode;
                    courier.serialNo = item.serialNo;
                    courier.type = item.type;
                    courier.mask = item.mask;
                    courier.name = item.name != null ? item.name : "";
                    courier.userType = item.userType != null ? item.userType : "";
                    courier.deviceAccountId = item.DeviceAccountId;
                    Model.Add(courier);
                }
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        public ActionResult DownloadLogData(DeviceLogSearchModel deviceLog)
        {
            VMSDBEntities db = new VMSDBEntities();
            List<EmployeePunchTB> savelist = new List<EmployeePunchTB>();
            List<DeviceLogsTB> punchlist = new List<DeviceLogsTB>();
            List<EventInfo> loglist = new List<EventInfo>();

            var device = db.DevicesTBs.Where(d => d.DeviceId == deviceLog.DeviceId).FirstOrDefault();

            if (device != null)
            {
                string url = "http://" + device.DeviceIPAddress + ":" + device.Port + "/ISAPI/ContentMgmt/DeviceMgmt/deviceList?format=json";

                string req = "{\"SearchDescription\" : {\"position\":0,\"maxResult\":5}}";

                //req = string.Empty;
                string reps = string.Empty;
                string strMatchNum = string.Empty;
                clienthttp clnt = new clienthttp();
                int iet = clnt.HttpRequest(device.UserName, device.Password, url, "POST", req, ref reps);
                if (iet == (int)HttpStatus.Http200)
                {
                    DeviceSearchRoot dr = JsonConvert.DeserializeObject<DeviceSearchRoot>(reps);
                    strMatchNum = Convert.ToString(dr.SearchResult.numOfMatches);

                    if ("0" != strMatchNum)
                    {
                        foreach (var item in dr.SearchResult.MatchList)
                        {
                            if (item.Device.EhomeParams.EhomeID == device.DeviceAccountId)
                            {
                                var devicedt = item.Device;

                                DateTime fdate = Convert.ToDateTime(deviceLog.fromdate);
                                string BeginTime = fdate.Year + "-" + fdate.Month.ToString("d2") + "-" + fdate.Day.ToString("d2") + "T00:00:00+05:30";

                                DateTime tdate = Convert.ToDateTime(deviceLog.todate);
                                string endTime = tdate.Year + "-" + tdate.Month.ToString("d2") + "-" + tdate.Day.ToString("d2") + "T23:00:00+05:30";


                                string index = devicedt.devIndex;
                                string strReq = "{\"AcsEventSearchDescription\" : {\"searchID\":\"0\",\"searchResultPosition\":0,\"maxResults\":50,\"AcsEventFilter\": {\"major\":0,\"minor\":0,\"startTime\": \"" + BeginTime + "\",\"endTime\": \"" + endTime + "\"}}}";
                                string strUrl = "http://" + device.DeviceIPAddress + ":" + device.Port + "/ISAPI/AccessControl/AcsEvent?format=json&devIndex=" + index;
                                string strRsp = string.Empty;

                                clienthttp http = new clienthttp();
                                int iRet = http.HttpRequest(device.UserName, device.Password, strUrl, "POST", strReq, ref strRsp);
                                if (iRet == (int)HttpStatus.Http200)
                                {
                                    EventSearchRoot drnew = JsonConvert.DeserializeObject<EventSearchRoot>(strRsp);
                                    strMatchNum = drnew.AcsEventSearchResult.numOfMatches;
                                    if ("0" != strMatchNum)
                                    {
                                        List<EmployeePunchTB> list = new List<EmployeePunchTB>();

                                        // loglist = drnew.AcsEventSearchResult.MatchList;

                                        foreach (var logitem in drnew.AcsEventSearchResult.MatchList)
                                        {
                                            if (logitem.currentVerifyMode != "invalid")
                                            {
                                                EmployeePunchTB info = new EmployeePunchTB();
                                                info.major = logitem.major;
                                                info.minor = logitem.minor;
                                                info.time = Convert.ToDateTime(logitem.time);
                                                info.DeviceAccountId = device.DeviceAccountId;
                                                info.employeeNoString = logitem.employeeNoString != null ? logitem.employeeNoString : "";
                                                info.cardNo = logitem.cardNo != null ? logitem.cardNo : "";
                                                info.cardReaderNo = logitem.cardReaderNo != null ? logitem.cardReaderNo : "";
                                                info.doorNo = logitem.doorNo != null ? logitem.doorNo : "";
                                                info.currentVerifyMode = logitem.currentVerifyMode;
                                                info.serialNo = logitem.serialNo;
                                                info.type = logitem.type;
                                                info.mask = logitem.mask;
                                                info.name = logitem.name != null ? logitem.name : "";
                                                info.userType = logitem.userType != null ? logitem.userType : "";
                                                savelist.Add(info);

                                                DeviceLogsTB tb = new DeviceLogsTB();
                                                tb.DeviceAccountId = Convert.ToInt32(device.DeviceAccountId);
                                                tb.AttendDate = info.time;
                                                tb.EmpID = logitem.employeeNoString != "" ? Convert.ToInt32(logitem.employeeNoString) : 0;
                                                tb.AccessCardNo = info.cardNo;
                                                tb.ADate = Convert.ToDateTime(info.time);
                                                TimeSpan timedt = Convert.ToDateTime(info.time).TimeOfDay;
                                                tb.ATime = timedt;
                                                punchlist.Add(tb);

                                                EventInfo dt = new EventInfo();
                                                dt.major = logitem.major;
                                                dt.minor = logitem.minor;
                                                dt.time = logitem.time;
                                                dt.Atime = Convert.ToDateTime(logitem.time).ToString("hh:mm:ss");
                                                dt.ADate = Convert.ToDateTime(logitem.time).ToString("dd/MM/yy");
                                                dt.employeeNoString = logitem.employeeNoString != null ? logitem.employeeNoString : "";
                                                dt.cardNo = logitem.cardNo != null ? logitem.cardNo : "";
                                                dt.cardReaderNo = logitem.cardReaderNo != null ? logitem.cardReaderNo : "";
                                                dt.doorNo = logitem.doorNo != null ? logitem.doorNo : "";
                                                dt.currentVerifyMode = logitem.currentVerifyMode;
                                                dt.serialNo = logitem.serialNo;
                                                dt.type = logitem.type;
                                                dt.mask = logitem.mask;
                                                dt.name = logitem.name != null ? logitem.name : ""; ;
                                                dt.userType = logitem.userType != null ? logitem.userType : "";
                                                loglist.Add(dt);
                                            }
                                        }

                                        if (savelist.Count() > 0)
                                        {
                                            using (VMSDBEntities entities = new VMSDBEntities())
                                            {
                                                //Loop and insert records.
                                                foreach (EmployeePunchTB events in savelist)
                                                {
                                                    entities.EmployeePunchTBs.Add(events);
                                                }

                                                foreach (DeviceLogsTB events in punchlist)
                                                {
                                                    entities.DeviceLogsTBs.Add(events);
                                                }
                                                int insertedRecords = entities.SaveChanges();
                                                // return Json(insertedRecords);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return Json(loglist, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public FileResult LogExportToExcel(DeviceLogSearchModel deviceLog, int DeviceId, DateTime fromdate, DateTime todate, FormCollection form)
        {

            if (DeviceId > 0)
            {
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[7] { new DataColumn("serialNo"),
                                            new DataColumn("Employee Id"),
                                            new DataColumn("Employee Name"),
                                            new DataColumn("card Number"),
                                            new DataColumn("Time"),
                                            new DataColumn("User Type"),
                                            new DataColumn("Current Verify Mode")});

                VMSDBEntities db = new VMSDBEntities();
                List<EmployeePunchTB> savelist = new List<EmployeePunchTB>();
                List<EventInfo> loglist = new List<EventInfo>();

                var device = db.DevicesTBs.Where(d => d.DeviceId == DeviceId).FirstOrDefault();

                if (device != null)
                {
                    string url = "http://" + device.DeviceIPAddress + ":" + device.Port + "/ISAPI/ContentMgmt/DeviceMgmt/deviceList?format=json";

                    string req = "{\"SearchDescription\" : {\"position\":0,\"maxResult\":5}}";

                    //req = string.Empty;
                    string reps = string.Empty;
                    string strMatchNum = string.Empty;
                    clienthttp clnt = new clienthttp();
                    int iet = clnt.HttpRequest(device.UserName, device.Password, url, "POST", req, ref reps);
                    if (iet == (int)HttpStatus.Http200)
                    {
                        DeviceSearchRoot dr = JsonConvert.DeserializeObject<DeviceSearchRoot>(reps);
                        strMatchNum = Convert.ToString(dr.SearchResult.numOfMatches);

                        if ("0" != strMatchNum)
                        {
                            foreach (var item in dr.SearchResult.MatchList)
                            {
                                if (item.Device.EhomeParams.EhomeID == device.DeviceAccountId)
                                {
                                    var devicedt = item.Device;

                                    DateTime fdate = Convert.ToDateTime(fromdate);
                                    string BeginTime = fdate.Year + "-" + fdate.Month.ToString("d2") + "-" + fdate.Day.ToString("d2") + "T00:00:00+05:30";

                                    DateTime tdate = Convert.ToDateTime(todate);
                                    string endTime = tdate.Year + "-" + tdate.Month.ToString("d2") + "-" + tdate.Day.ToString("d2") + "T23:00:00+05:30";


                                    string index = devicedt.devIndex;
                                    string strReq = "{\"AcsEventSearchDescription\" : {\"searchID\":\"0\",\"searchResultPosition\":0,\"maxResults\":50,\"AcsEventFilter\": {\"major\":0,\"minor\":0,\"startTime\": \"" + BeginTime + "\",\"endTime\": \"" + endTime + "\"}}}";
                                    string strUrl = "http://" + device.DeviceIPAddress + ":" + device.Port + "/ISAPI/AccessControl/AcsEvent?format=json&devIndex=" + index;
                                    string strRsp = string.Empty;

                                    clienthttp http = new clienthttp();
                                    int iRet = http.HttpRequest(device.UserName, device.Password, strUrl, "POST", strReq, ref strRsp);
                                    if (iRet == (int)HttpStatus.Http200)
                                    {
                                        EventSearchRoot drnew = JsonConvert.DeserializeObject<EventSearchRoot>(strRsp);
                                        strMatchNum = drnew.AcsEventSearchResult.numOfMatches;
                                        if ("0" != strMatchNum)
                                        {
                                            foreach (var logitem in drnew.AcsEventSearchResult.MatchList)
                                            {
                                                EventInfo log = new EventInfo();
                                                log.major = logitem.major;
                                                log.minor = logitem.minor;
                                                log.time = logitem.time;
                                                log.employeeNoString = logitem.employeeNoString != null ? logitem.employeeNoString : "";
                                                log.cardNo = logitem.cardNo != null ? logitem.cardNo : "";
                                                log.cardReaderNo = logitem.cardReaderNo != null ? logitem.cardReaderNo : "";
                                                log.doorNo = logitem.doorNo != null ? logitem.doorNo : "";
                                                log.currentVerifyMode = logitem.currentVerifyMode;
                                                log.serialNo = logitem.serialNo;
                                                log.type = logitem.type;
                                                log.mask = logitem.mask;
                                                log.name = logitem.name != null ? logitem.name : ""; ;
                                                log.userType = logitem.userType != null ? logitem.userType : "";
                                                loglist.Add(log);

                                                string xldate = string.Format("{0:dd-MM-yyyy h:mm:ss tt}", logitem.time);
                                                dt.Rows.Add(logitem.serialNo, logitem.employeeNoString, logitem.name, logitem.cardNo, logitem.time, logitem.userType, logitem.currentVerifyMode);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DeviceLog.xlsx");
                    }
                }
            }

            return File("", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DeviceLog.xlsx");
        }

        [HttpPost]
        public ActionResult LogExportToPdf(int? pageNumber, DeviceLogSearchModel deviceLog)
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[7] { new DataColumn("serialNo"),
                                            new DataColumn("Employee Id"),
                                            new DataColumn("Employee Name"),
                                            new DataColumn("card Number"),
                                            new DataColumn("Time"),
                                            new DataColumn("User Type"),
                                            new DataColumn("Current Verify Mode")});

            VMSDBEntities db = new VMSDBEntities();
            List<EmployeePunchTB> savelist = new List<EmployeePunchTB>();
            List<EventInfo> loglist = new List<EventInfo>();

            var device = db.DevicesTBs.Where(d => d.DeviceId == deviceLog.DeviceId).FirstOrDefault();

            if (device != null)
            {
                string url = "http://" + device.DeviceIPAddress + ":" + device.Port + "/ISAPI/ContentMgmt/DeviceMgmt/deviceList?format=json";

                string req = "{\"SearchDescription\" : {\"position\":0,\"maxResult\":5}}";

                //req = string.Empty;
                string reps = string.Empty;
                string strMatchNum = string.Empty;
                clienthttp clnt = new clienthttp();
                int iet = clnt.HttpRequest(device.UserName, device.Password, url, "POST", req, ref reps);
                if (iet == (int)HttpStatus.Http200)
                {
                    DeviceSearchRoot dr = JsonConvert.DeserializeObject<DeviceSearchRoot>(reps);
                    strMatchNum = Convert.ToString(dr.SearchResult.numOfMatches);

                    if ("0" != strMatchNum)
                    {
                        foreach (var item in dr.SearchResult.MatchList)
                        {
                            if (item.Device.EhomeParams.EhomeID == device.DeviceAccountId)
                            {
                                var devicedt = item.Device;

                                DateTime fdate = Convert.ToDateTime(deviceLog.fromdate);
                                string BeginTime = fdate.Year + "-" + fdate.Month.ToString("d2") + "-" + fdate.Day.ToString("d2") + "T00:00:00+05:30";

                                DateTime tdate = Convert.ToDateTime(deviceLog.todate);
                                string endTime = tdate.Year + "-" + tdate.Month.ToString("d2") + "-" + tdate.Day.ToString("d2") + "T23:00:00+05:30";


                                string index = devicedt.devIndex;
                                string strReq = "{\"AcsEventSearchDescription\" : {\"searchID\":\"0\",\"searchResultPosition\":0,\"maxResults\":50,\"AcsEventFilter\": {\"major\":0,\"minor\":0,\"startTime\": \"" + BeginTime + "\",\"endTime\": \"" + endTime + "\"}}}";
                                string strUrl = "http://" + device.DeviceIPAddress + ":" + device.Port + "/ISAPI/AccessControl/AcsEvent?format=json&devIndex=" + index;
                                string strRsp = string.Empty;

                                clienthttp http = new clienthttp();
                                int iRet = http.HttpRequest(device.UserName, device.Password, strUrl, "POST", strReq, ref strRsp);
                                if (iRet == (int)HttpStatus.Http200)
                                {
                                    EventSearchRoot drnew = JsonConvert.DeserializeObject<EventSearchRoot>(strRsp);
                                    strMatchNum = drnew.AcsEventSearchResult.numOfMatches;
                                    if ("0" != strMatchNum)
                                    {
                                        foreach (var logitem in drnew.AcsEventSearchResult.MatchList)
                                        {
                                            EventInfo log = new EventInfo();
                                            log.major = logitem.major;
                                            log.minor = logitem.minor;
                                            log.time = logitem.time;
                                            log.employeeNoString = logitem.employeeNoString != null ? logitem.employeeNoString : "";
                                            log.cardNo = logitem.cardNo != null ? logitem.cardNo : "";
                                            log.cardReaderNo = logitem.cardReaderNo != null ? logitem.cardReaderNo : "";
                                            log.doorNo = logitem.doorNo != null ? logitem.doorNo : "";
                                            log.currentVerifyMode = logitem.currentVerifyMode;
                                            log.serialNo = logitem.serialNo;
                                            log.type = logitem.type;
                                            log.mask = logitem.mask;
                                            log.name = logitem.name != null ? logitem.name : ""; ;
                                            log.userType = logitem.userType != null ? logitem.userType : "";
                                            loglist.Add(log);

                                            string xldate = string.Format("{0:dd-MM-yyyy h:mm:ss tt}", logitem.time);
                                            dt.Rows.Add(logitem.serialNo, logitem.employeeNoString, logitem.name, logitem.cardNo, logitem.time, logitem.userType, logitem.currentVerifyMode);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                int pdfRowIndex = 1;

                string filename = "DeviceLogList-" + DateTime.Now.ToString("dd-MM-yyyy hh_mm_s_tt");
                string filepath = Server.MapPath("\\") + "" + filename + ".pdf";
                Document document = new Document(PageSize.A4, 5f, 5f, 10f, 10f);
                FileStream fs = new FileStream(filepath, FileMode.Create);
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                document.Open();

                Font font1 = FontFactory.GetFont(FontFactory.COURIER_BOLD, 10);
                Font font2 = FontFactory.GetFont(FontFactory.COURIER, 8);

                float[] columnDefinitionSize = { 1F, 2F, 3F, 2F, 2F, 1F, 1F, 1F, 1F, 1F };
                PdfPTable table;
                PdfPCell cell;

                table = new PdfPTable(columnDefinitionSize)
                {
                    WidthPercentage = 100
                };

                cell = new PdfPCell
                {
                    BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
                };

                table.AddCell(new Phrase("serialNo", font1));
                table.AddCell(new Phrase("Employee Id", font1));
                table.AddCell(new Phrase("Employee Name", font1));
                table.AddCell(new Phrase("card Number", font1));
                table.AddCell(new Phrase("Time", font1));
                table.AddCell(new Phrase("User Type", font1));
                table.AddCell(new Phrase("Current Verify Mode", font1));
                table.HeaderRows = 1;

                foreach (DataRow data in dt.Rows)
                {
                    table.AddCell(new Phrase(data["serialNo"].ToString(), font2));
                    table.AddCell(new Phrase(data["Employee Id"].ToString(), font2));
                    table.AddCell(new Phrase(data["Employee Name"].ToString(), font2));
                    table.AddCell(new Phrase(data["card Number"].ToString(), font2));
                    table.AddCell(new Phrase(data["Time"].ToString(), font2));
                    table.AddCell(new Phrase(data["User Type"].ToString(), font2));
                    table.AddCell(new Phrase(data["Current Verify Mode"].ToString(), font2));

                    pdfRowIndex++;
                }

                document.Add(table);
                document.Close();
                document.CloseDocument();
                document.Dispose();
                writer.Close();
                writer.Dispose();
                fs.Close();
                fs.Dispose();

                FileStream sourceFile = new FileStream(filepath, FileMode.Open);
                float fileSize = 0;
                fileSize = sourceFile.Length;
                byte[] getContent = new byte[Convert.ToInt32(Math.Truncate(fileSize))];
                sourceFile.Read(getContent, 0, Convert.ToInt32(sourceFile.Length));
                sourceFile.Close();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Length", getContent.Length.ToString());
                Response.AddHeader("Content-Disposition", "attachment; filename=" + filename + ".pdf;");
                Response.BinaryWrite(getContent);
                Response.Flush();
                Response.End();
            }
            return View("GetDeviceLog");
        }


        #endregion

        #region DB Setting

        public ActionResult DBSetting()
        {
            ViewBag.UserId = 1;
            ViewBag.UserName = "sagar123@gmail.com";
            DBModel dB = new DBModel();
            dB = GetDBSetting();
            if(dB.DataSource != null)
            {
                ViewBag.Status = "Exist";
            }
            else
            {
                ViewBag.Status = "";
            }

            
            return View();
        }

        private static DBModel GetDBSetting()
        {
            DBModel  Model = new DBModel();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var data = entities.DBSettingTBs.FirstOrDefault();

                Model.DataSource = data.DataSource;
                Model.DBName = data.DBName;
                Model.UserId = data.UserId;
                Model.Password = data.Password;
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        public ActionResult AddDBSetting()
        {
            ViewBag.UserId = 1;
            ViewBag.UserName = "sagar123@gmail.com";
            DBModel dB = new DBModel();
            return View("AddDB", dB);
        }

        [HttpPost]
        public async Task<ActionResult> SaveDB(DBModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //update web.config
                    string name = "VMSDBEntities";
                    bool isNew = false;
                    string path = Server.MapPath("~/Web.Config");
                    XmlDocument doc = new XmlDocument();
                    doc.Load(path);
                    XmlNodeList list = doc.DocumentElement.SelectNodes(string.Format("connectionStrings/add[@name='{0}']", name));
                    XmlNode node;
                    isNew = list.Count == 0;
                    if (isNew)
                    {
                        node = doc.CreateNode(XmlNodeType.Element, "add", null);
                        XmlAttribute attribute = doc.CreateAttribute("name");
                        attribute.Value = name;
                        node.Attributes.Append(attribute);

                        attribute = doc.CreateAttribute("connectionString");
                        attribute.Value = "";
                        node.Attributes.Append(attribute);

                        attribute = doc.CreateAttribute("providerName");
                        attribute.Value = "System.Data.SqlClient";
                        node.Attributes.Append(attribute);
                    }
                    else
                    {
                        node = list[0];
                    }
                    string conString = node.Attributes["connectionString"].Value;
                    EntityConnectionStringBuilder conStringBuilder = new EntityConnectionStringBuilder(conString);
                    conStringBuilder.Metadata = model.DBName;
                    if(model.UserId != null && model.Password != null)
                    {
                        conStringBuilder.Metadata = "res://*/Middleware.VMS.csdl|res://*/Middleware.VMS.ssdl|res://*/Middleware.VMS.msl";
                        conStringBuilder.Provider = "System.Data.SqlClient";
                        conStringBuilder.ProviderConnectionString = "data source=" + model.DataSource + ";initial catalog=" + model.DBName + ";persist security info=True;user id=" + model.UserId + ";password=" + model.Password + ";MultipleActiveResultSets=True;App=EntityFramework";
                    }
                    else
                    {
                        conStringBuilder.Metadata = "res://*/Middleware.VMS.csdl|res://*/Middleware.VMS.ssdl|res://*/Middleware.VMS.msl";
                        conStringBuilder.Provider = "System.Data.SqlClient";
                        conStringBuilder.ProviderConnectionString = "data source=" + model.DataSource + ";initial catalog=" + model.DBName + ";integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
                    }

                    //SqlConnectionStringBuilder conStringBuilder = new SqlConnectionStringBuilder(conString);

                    //conStringBuilder.InitialCatalog = model.DBName;
                    //conStringBuilder.DataSource = model.DataSource;
                    //if(model.UserId != null && model.Password != null)
                    //{
                    //    conStringBuilder.IntegratedSecurity = false;
                    //    conStringBuilder.UserID = model.UserId;
                    //    conStringBuilder.Password = model.Password;
                    //}
                    //else
                    //{
                    //    conStringBuilder.IntegratedSecurity = true;
                    //}


                    node.Attributes["connectionString"].Value = conStringBuilder.ConnectionString;
                    if (isNew)
                    {
                        doc.DocumentElement.SelectNodes("connectionStrings")[0].AppendChild(node);
                    }
                    doc.Save(path);

                    VMSDBEntities db = new VMSDBEntities();

                    DBSettingTB tB = new DBSettingTB();
                    tB.DataSource = model.DataSource;
                    tB.DBName = model.DBName;
                    tB.UserId = model.UserId;
                    tB.Password = model.Password;
                    db.DBSettingTBs.Add(tB);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return View("DBSetting", model);
            }
            //return RedirectToAction("GetDeviceList", "Setting", new { userId = ViewBag.UserId, userName = ViewBag.UserName });
            ViewBag.Result = "Success";
            model = new DBModel();
            return View("DBSetting", model);
        }

        #endregion

        #region api monitor 

        [HttpPost]
        public JsonResult A_GetApiMoniorList()
        {
            List<ApiMonitorModel> Model = new List<ApiMonitorModel>();
            Model = GetMonitorList();
            return Json(Model);
        }

        public ActionResult ApiMonitor()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "ApiMonitor";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewData["GetMonitorList"] = GetMonitorList();
                return View();
            }
        }

        private static List<ApiMonitorModel> GetMonitorList()
        {
            List<ApiMonitorModel> Model = new List<ApiMonitorModel>();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var data = entities.ApiMonitorTBs.ToList().OrderByDescending(d => d.Id);

                foreach (var item in data)
                {
                    ApiMonitorModel courier = new ApiMonitorModel();
                    courier.Id = item.Id;
                    courier.Page = item.Page;
                    courier.Action = item.Action;
                    courier.EmpCode = item.EmpCode;
                    courier.EmpName = item.EmpName;
                    courier.DeviceSRNo = item.DeviceSRNO;
                    courier.DeviceName = item.DeviceName;
                    courier.Status = item.Status;
                    courier.Command = item.Command;
                    courier.time = Convert.ToDateTime(item.time);                   
                    Model.Add(courier);
                }
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        #endregion
    }
}