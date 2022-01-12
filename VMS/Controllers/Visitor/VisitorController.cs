using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using VMS.Controllers.Account;
using VMS.Middleware;
using VMS.Models;
using VMS.Models.Account;
using VMS.Models.Admin;
using VMS.Models.Visitor;

namespace VMS.Controllers.Visitor
{
    public class VisitorController : BaseController
    {
        VMSDBEntities db;
        List<VisitorEntryModel> visitorEntries;
        public VisitorController()
        {
            db = new VMSDBEntities();
            visitorEntries = new List<VisitorEntryModel>();
        }

        // GET: Visitor
        public ActionResult Index()
        {
            return View();
        }


        // GET: VisitorList
        public ActionResult GetVisitorList()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "VisitorList";

            //string sessionid = (System.Web.HttpContext.Current.Session["SessionId"] != null) ? System.Web.HttpContext.Current.Session["SessionId"].ToString() : String.Empty;
            //if (string.IsNullOrEmpty(sessionid))
            //{
            //    return RedirectToAction("LogOut", "Account");
            //}
            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewData["GetVisitorList"] = visitorEntries = GetVisitors();
                return View();
            }
        }

        public ActionResult AddVisitor(int Id)
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "VisitorList";

            string sessionid = (System.Web.HttpContext.Current.Session["SessionId"] != null) ? System.Web.HttpContext.Current.Session["SessionId"].ToString() : String.Empty;
            if (string.IsNullOrEmpty(sessionid))
            {
                return RedirectToAction("LogOut", "Account");
            }
            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                VisitorEntryModel visitor = new VisitorEntryModel();

                ViewBag.EmpList = new SelectList(GetEmployeeList(), "UserId", "Name");
                ViewBag.IdProofList = new SelectList(GetIdProofType(), "Name", "Name");
                ViewBag.MaterialList = new SelectList(GetMaterialList(), "Name", "Name");
                ViewBag.PriorityList = new SelectList(GetPrioritylList(), "Name", "Name");
                ViewBag.VisitorTypeList = new SelectList(GetVisitorTypeList(), "Name", "Name");
                ViewBag.VehicleTypeList = new SelectList(GetVehicleTypeList(), "Name", "Name");
                ViewBag.DeviceList = new SelectList(GetDeviceList(), "DeviceId", "DeviceName");

                if (Id > 0)
                {
                    visitor = GetVisitorDetails(Id);
                }
                else
                {
                    visitor.PUCEndDate = DateTime.Now;
                }

                return View("AddVisitor", visitor);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadFile()
        {
            string _imgname = string.Empty;
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var pic = System.Web.HttpContext.Current.Request.Files["MyImages"];
                if (pic.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(pic.FileName);
                    var _ext = Path.GetExtension(pic.FileName);

                    _imgname = Guid.NewGuid().ToString();
                    var _comPath = Server.MapPath("/Uploads/VS_") + _imgname + _ext;
                    _imgname = "VS_" + _imgname + _ext;

                    ViewBag.Msg = _comPath;
                    var path = _comPath;

                    // Saving Image in Original Mode
                    pic.SaveAs(path);

                    // resizing image
                    MemoryStream ms = new MemoryStream();
                    WebImage img = new WebImage(_comPath);

                    if (img.Width > 200)
                        img.Resize(200, 200);
                    img.Save(_comPath);
                    // end resize
                }
            }
            return Json(Convert.ToString(_imgname), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UploadCaptureFile(string data)
        {
            string _imgname = Guid.NewGuid().ToString();
         
            string imageName = _imgname + ".png";
            string path = Server.MapPath("/Uploads/Photos");
            if ((!System.IO.Directory.Exists(path)))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            var _comPath = path + "/VS_"+imageName;

            Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
            data = regex.Replace(data, string.Empty);
            byte[] imageBytes = Convert.FromBase64String(data);
            System.IO.File.WriteAllBytes(_comPath, imageBytes);

            return Json(Convert.ToString(_imgname), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UploadCaptureCertificate(string data)
        {
            string _imgname = Guid.NewGuid().ToString();

            string imageName = _imgname + ".png";
            string path = Server.MapPath("/Uploads/Certificate");
            if ((!System.IO.Directory.Exists(path)))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            var _comPath = path + "/VS_" + imageName;

            Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
            data = regex.Replace(data, string.Empty);
            byte[] imageBytes = Convert.FromBase64String(data);
            System.IO.File.WriteAllBytes(_comPath, imageBytes);

            return Json(Convert.ToString(_imgname), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Save(VisitorEntryModel visitor)
        {
            string userid = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            string username = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserId = userid;
            ViewBag.UserName = username;

            ViewBag.ActivePage = "VisitorEntry";

            //string sessionid = (System.Web.HttpContext.Current.Session["SessionId"] != null) ? System.Web.HttpContext.Current.Session["SessionId"].ToString() : String.Empty;
            //if (string.IsNullOrEmpty(sessionid))
            //{
            //    return RedirectToAction("LogOut", "Account");
            //}
            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }

            //if (supplier.Id == 0)
            //{
            //    ModelState.Remove("Id");
            //}            



            //DateTime dat = Convert.ToDateTime(one + " " + two);
            //DateTime dt1 = DateTime.ParseExact(one + " " + two, "dd/MM/yy h:mm tt", CultureInfo.InvariantCulture);

            if (ModelState.IsValid)
            {
                try
                {
                    string addId = "";
                    VisitorTB dt = new VisitorTB();

                    if (visitor.Id > 0)
                    {
                        var checkVisitor = db.VisitorEntryTBs.Where(d => d.Id == visitor.Id).FirstOrDefault();

                        checkVisitor.EmployeeId = visitor.EmployeeId;
                        addId = checkVisitor.VisitorId;
                        checkVisitor.Name = visitor.Name;
                        checkVisitor.Company = visitor.Company;
                        checkVisitor.VisitorType = visitor.VisitorType;
                        checkVisitor.Address = visitor.Address;
                        checkVisitor.InTime = visitor.InTime;
                        checkVisitor.OutTime = visitor.OutTime;
                        checkVisitor.FromDate = visitor.VisitDateFrom;
                        checkVisitor.ToDate = visitor.VisitDateTo;
                        checkVisitor.IdProof = visitor.IdProof;
                        checkVisitor.IdProofNumber = visitor.IdProofNo;
                        checkVisitor.Photo = visitor.PhotoPathCapture == "" ? visitor.PhotoPath : visitor.PhotoPathCapture ;
                        checkVisitor.CertificateImagePath = visitor.captureCertificate;
                        checkVisitor.EmailId = visitor.EmailId;
                        checkVisitor.Contact = visitor.Contact;
                        checkVisitor.Purpose = visitor.Purpose;
                        checkVisitor.Remark = visitor.Remark;
                        checkVisitor.Priority = visitor.Priority;
                        checkVisitor.Material = visitor.Material;
                        checkVisitor.UserId = Convert.ToInt32(userid);
                        checkVisitor.VehicleType = visitor.VehicleType;
                        checkVisitor.VehicleNo = visitor.VehicleNo;
                        checkVisitor.VehiclePUCNo = visitor.VehiclePUCNo;
                        checkVisitor.PUCEndDate = visitor.PUCEndDate != null ? visitor.PUCEndDate : null;
                        checkVisitor.DeviceId = visitor.DeviceId;
                        checkVisitor.CardNo = visitor.CardNo;
                        db.Entry(checkVisitor).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        var visitordata = db.VisitorTBs.Where(d => d.Contact == visitor.Contact).FirstOrDefault();

                        if (visitordata != null)
                        {
                            VisitorEntryTB tB = new VisitorEntryTB();
                            tB.EmployeeId = visitor.EmployeeId;
                            tB.VisitorId = visitordata.VisitorId;
                            addId = visitordata.VisitorId;
                            tB.Name = visitor.Name;
                            tB.Company = visitor.Company;
                            tB.VisitorType = visitor.VisitorType;
                            tB.Address = visitor.Address;
                            tB.InTime = visitor.InTime;
                            tB.OutTime = visitor.OutTime;
                            tB.FromDate = visitor.VisitDateFrom;
                            tB.ToDate = visitor.VisitDateTo;
                            tB.IdProof = visitor.IdProof;
                            tB.IdProofNumber = visitor.IdProofNo;
                            tB.Photo = visitor.PhotoPathCapture == "" ? visitor.PhotoPath : visitor.PhotoPathCapture;
                            tB.CertificateImagePath = visitor.captureCertificate;
                            tB.EmailId = visitor.EmailId;
                            tB.Contact = visitor.Contact;
                            tB.Purpose = visitor.Purpose;
                            tB.Remark = visitor.Remark;
                            tB.Priority = visitor.Priority;
                            tB.Material = visitor.Material;
                            tB.VehicleType = visitor.VehicleType;
                            tB.VehicleNo = visitor.VehicleNo;
                            tB.VehiclePUCNo = visitor.VehiclePUCNo;
                            tB.PUCEndDate = visitor.PUCEndDate != null ? visitor.PUCEndDate : null;
                            tB.UserId = Convert.ToInt32(userid);
                            tB.DeviceId = visitor.DeviceId;
                            tB.CardNo = visitor.CardNo;
                            db.VisitorEntryTBs.Add(tB);
                            db.SaveChanges();
                        }
                        else
                        {
                            Random generator = new Random();
                            string rand = generator.Next(0, 1000000).ToString("D6");
                            dt.VisitorId = rand;
                            addId = rand;
                            dt.Name = visitor.Name;
                            dt.Contact = visitor.Contact;
                            dt.EmailId = visitor.EmailId;
                            dt.Company = visitor.Company;
                            dt.Photo = visitor.PhotoPath;
                            db.VisitorTBs.Add(dt);
                            db.SaveChanges();

                            VisitorEntryTB tB = new VisitorEntryTB();
                            tB.EmployeeId = visitor.EmployeeId;
                            tB.VisitorId = dt.VisitorId;
                            tB.Name = visitor.Name;
                            tB.Company = visitor.Company;
                            tB.VisitorType = visitor.VisitorType;
                            tB.Address = visitor.Address;
                            tB.InTime = visitor.InTime;
                            tB.OutTime = visitor.OutTime;
                            tB.FromDate = visitor.VisitDateFrom;
                            tB.ToDate = visitor.VisitDateTo;
                            tB.IdProof = visitor.IdProof;
                            tB.IdProofNumber = visitor.IdProofNo;
                            tB.Photo = visitor.PhotoPath;
                            tB.CertificateImagePath = visitor.captureCertificate;
                            tB.EmailId = visitor.EmailId;
                            tB.Contact = visitor.Contact;
                            tB.Purpose = visitor.Purpose;
                            tB.Remark = visitor.Remark;
                            tB.Priority = visitor.Priority;
                            tB.Material = visitor.Material;
                            tB.UserId = Convert.ToInt32(userid);
                            tB.VehicleType = visitor.VehicleType;
                            tB.VehicleNo = visitor.VehicleNo;
                            tB.VehiclePUCNo = visitor.VehiclePUCNo;
                            tB.PUCEndDate = visitor.PUCEndDate != null ? visitor.PUCEndDate : null;
                            tB.DeviceId = visitor.DeviceId;
                            tB.CardNo = visitor.CardNo;
                            db.VisitorEntryTBs.Add(tB);
                            db.SaveChanges();
                        }
                    }

                    var maildata = db.MailSettingTBs.FirstOrDefault();
                    string Host = maildata.Host;
                    string from = maildata.smtpfrom;
                    int Port = Convert.ToInt32(maildata.port);
                    string Username = maildata.username;
                    string pass = maildata.password;

                    string bodystring = string.Empty;

                    if (maildata != null)
                    {
                        #region send mail to visitor
                        if (visitor.EmailId != null)
                        {
                            //// Hader start
                            bodystring = "<!DOCTYPE html><html><head><title> VMS : Visit Email</title> </head> <body>";
                            bodystring += "<table width='100%' border='0' cellspacing='0' cellpadding='0'><tbody> ";
                            /// hader end

                            bodystring += "<tr><td align='center'>";
                            bodystring += "<table width='92% ' border='0' cellspacing='0' cellpadding='0' bgcolor='#ffffff' style='border-radius:10px; padding: 15px'>";
                            bodystring += "<tbody>  <tr> <td align='center' valign='top'>";
                            bodystring += "<table width='100% ' border='0' cellspacing='0' cellpadding='0' style='border - radius:10px; border: 2px solid #eac356; padding: 0 25px;'>";
                            bodystring += "<tbody>  <tr> <td>&nbsp;</td>";
                            bodystring += "<td style='font - family:Arial; font - size:15px; line - height:21px; color:#000000'><strong style='font-weight:200;display: block; font-size: 30px; line-height: 3;text-align: center;'> Visit </strong>You recently requested to visit through VMS. <br/>";
                            bodystring += "</td>";
                            bodystring += "<td>&nbsp;</td></tr>";

                            bodystring += "<tr> <td>&nbsp;</td> <td>&nbsp;</td>  <td>&nbsp;</td> </tr>";
                            bodystring += "<tr> <td>&nbsp;</td><td height='30'  valign='bottom' style='font - family:Arial; font - size:13px; line - height:20px; color:#393a3c'>Thank you & Regards, <br/> <strong>Team VMS</strong>.</td><td>&nbsp;</td></tr>";
                            bodystring += "<tr><td>&nbsp;</td>  <td>&nbsp;</td>  <td>&nbsp;</td>  </tr>";
                            bodystring += "<tr><td>&nbsp;</td>  <td>&nbsp;</td>  <td>&nbsp;</td>  </tr>";
                            bodystring += " </tbody>  </table> </td> </tr>";
                            bodystring += "</tbody> </table>  </td> </tr>";
                            bodystring += " </td></tr>";
                            bodystring += "<tr> <td height='25'>&nbsp;</td>  </tr>";
                            bodystring += "</tbody>  </table> </td> </tr> </tbody>  </table> </body> </html>";

                            //var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                            //string Host = smtpSection.Network.Host;
                            //string from = smtpSection.From;
                            //int Port = smtpSection.Network.Port;
                            //string Username = smtpSection.Network.UserName;
                            //string pass = smtpSection.Network.Password;

                            SmtpClient SmtpServer = new SmtpClient(Host);
                            SmtpServer.UseDefaultCredentials = true;
                            MailMessage mails = new MailMessage();
                            mails.From = new MailAddress(from);
                            //mails.From = new MailAddress("ashokmehta2024@gmail.com");
                            mails.To.Add(visitor.EmailId);
                            // mails.To.Add("Vishakhasupnekar1307@gmail.com");
                            mails.IsBodyHtml = true;
                            mails.Subject = "Visit Managment system";
                            mails.Body = bodystring;
                            SmtpServer.Port = Port;
                            SmtpServer.UseDefaultCredentials = true;
                            SmtpServer.Credentials = new System.Net.NetworkCredential(Username, pass);
                            //SmtpServer.EnableSsl = true;
                            SmtpServer.Send(mails);
                        }
                        #endregion

                        #region send mail to Employee

                        var user = db.UserTBs.Where(d => d.UserId == visitor.EmployeeId).FirstOrDefault();

                        if (user.Email != null)
                        {
                            bodystring = string.Empty;

                            //// Hader start
                            bodystring = "<!DOCTYPE html><html><head><title> VMS : Visitor Request</title> </head> <body>";
                            bodystring += "<table width='100%' border='0' cellspacing='0' cellpadding='0'><tbody> ";
                            /// hader end

                            bodystring += "<tr><td align='center'>";
                            bodystring += "<table width='92% ' border='0' cellspacing='0' cellpadding='0' bgcolor='#ffffff' style='border-radius:10px; padding: 15px'>";
                            bodystring += "<tbody>  <tr> <td align='center' valign='top'>";
                            bodystring += "<table width='100% ' border='0' cellspacing='0' cellpadding='0' style='border - radius:10px; border: 2px solid #eac356; padding: 0 25px;'>";
                            bodystring += "<tbody>  <tr> <td>&nbsp;</td>";
                            bodystring += "<td style='font - family:Arial; font - size:15px; line - height:21px; color:#000000'><strong style='font-weight:200;display: block; font-size: 30px; line-height: 3;text-align: center;'> VMS </strong>Vistor makes request to meet you through VMS. <br/>";
                            bodystring += "</td>";
                            bodystring += "<td>&nbsp;</td></tr>";

                            bodystring += "<tr> <td>&nbsp;</td> <td>&nbsp;</td>  <td>&nbsp;</td> </tr>";
                            bodystring += "<tr> <td>&nbsp;</td><td height='30'  valign='bottom' style='font - family:Arial; font - size:13px; line - height:20px; color:#393a3c'>Thank you & Regards, <br/> <strong>Team VMS</strong>.</td><td>&nbsp;</td></tr>";
                            bodystring += "<tr><td>&nbsp;</td>  <td>&nbsp;</td>  <td>&nbsp;</td>  </tr>";
                            bodystring += "<tr><td>&nbsp;</td>  <td>&nbsp;</td>  <td>&nbsp;</td>  </tr>";
                            bodystring += " </tbody>  </table> </td> </tr>";
                            bodystring += "</tbody> </table>  </td> </tr>";
                            bodystring += " </td></tr>";
                            bodystring += "<tr> <td height='25'>&nbsp;</td>  </tr>";
                            bodystring += "</tbody>  </table> </td> </tr> </tbody>  </table> </body> </html>";

                            //smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                            //Host = smtpSection.Network.Host;
                            //from = smtpSection.From;
                            //Port = smtpSection.Network.Port;
                            //Username = smtpSection.Network.UserName;
                            //pass = smtpSection.Network.Password;

                            SmtpClient SmtpServer = new SmtpClient(Host);

                            MailMessage mails = new MailMessage();
                            mails.From = new MailAddress(from);
                            mails.To.Add(user.Email);
                            //mails.To.Add("amy21690@gmail.com");
                            mails.IsBodyHtml = true;
                            mails.Subject = "Visit Managment system";
                            mails.Body = bodystring;
                            SmtpServer.Port = Port;
                            SmtpServer.Credentials = new System.Net.NetworkCredential(Username, pass);
                            SmtpServer.EnableSsl = true;
                            SmtpServer.Send(mails);
                        }

                        #endregion
                    }

                    #region call punch apis

                    var device = db.DevicesTBs.Where(d => d.DeviceId == visitor.DeviceId).FirstOrDefault();

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

                                        ApiMonitorTB at = new ApiMonitorTB();
                                        at.Command = "Check Device Exist";
                                        at.Page = "Visitor";
                                        at.time = DateTime.Now;
                                        at.DeviceSRNO = device.DeviceSerialNo;
                                        at.DeviceName = device.DeviceName;
                                        at.EmpCode = "";
                                        at.EmpName = "";
                                        at.Status = "Success";
                                        db.ApiMonitorTBs.Add(at);
                                        db.SaveChanges();

                                        var devicedt = item.Device;

                                        DateTime fdate = Convert.ToDateTime(visitor.VisitDateFrom);
                                        DateTime ftime = Convert.ToDateTime(visitor.InTime);
                                        string BeginTime = fdate.Year + "-" + fdate.Month.ToString("d2") + "-" + fdate.Day.ToString("d2") + "T" + ftime.Hour.ToString("d2") + ":" + ftime.Minute.ToString("d2") + ":" + ftime.Second.ToString("d2");

                                        DateTime tdate = Convert.ToDateTime(visitor.VisitDateTo);
                                        DateTime ttime = Convert.ToDateTime(visitor.OutTime);
                                        string endTime = tdate.Year + "-" + tdate.Month.ToString("d2") + "-" + tdate.Day.ToString("d2") + "T" + ttime.Hour.ToString("d2") + ":" + ttime.Minute.ToString("d2") + ":" + ttime.Second.ToString("d2");


                                        string strName = visitor.Name;
                                        string strUserType = "normal";
                                        string index = devicedt.devIndex;
                                        string strReq = "{\"UserInfo\" : [{\"employeeNo\": \"" + addId + "\",\"name\": \"" + strName + "\",\"userType\": \"" + strUserType + "\",\"Valid\" : {\"enable\": true,\"beginTime\": \"" + BeginTime + "\",\"endTime\": \"" + endTime + "\",\"timeType\" : \"local\"}}]}";
                                        string strUrl = "http://" + device.DeviceIPAddress + ":" + device.Port + "/ISAPI/AccessControl/UserInfo/Record?format=json&devIndex=" + index;
                                        string strRsp = string.Empty;

                                        clienthttp http = new clienthttp();
                                        int iRet = http.HttpRequest(device.UserName, device.Password, strUrl, "POST", strReq, ref strRsp);
                                        if (iRet == (int)HttpStatus.Http200)
                                        {
                                            at = new ApiMonitorTB();
                                            at.Command = "Add Visitor";
                                            at.Page = "Visitor";
                                            at.time = DateTime.Now;
                                            at.DeviceSRNO = device.DeviceSerialNo;
                                            at.DeviceName = device.DeviceName;
                                            at.EmpCode = addId;
                                            at.EmpName = strName;
                                            at.Status = "Success";
                                            db.ApiMonitorTBs.Add(at);
                                            db.SaveChanges();

                                            //for add card no
                                            strReq = "{\"CardInfo\":{\"employeeNo\":\"" + addId + "\",\"cardNo\":\"" + visitor.CardNo + "\",\"cardType\":\"normalCard\"}}";
                                            strUrl = "http://" + device.DeviceIPAddress + ":" + device.Port + "/ISAPI/AccessControl/CardInfo/Record?format=json&devIndex=" + index;
                                            strRsp = string.Empty;

                                            http = new clienthttp();
                                            iRet = http.HttpRequest(device.UserName, device.Password, strUrl, "POST", strReq, ref strRsp);

                                            if (iRet == (int)HttpStatus.Http200)
                                            {
                                                at = new ApiMonitorTB();
                                                at.Command = "Add Card";
                                                at.Page = "Visitor";
                                                at.time = DateTime.Now;
                                                at.DeviceSRNO = device.DeviceSerialNo;
                                                at.DeviceName = device.DeviceName;
                                                at.EmpCode = addId;
                                                at.EmpName = strName;
                                                at.Status = "Success";
                                                db.ApiMonitorTBs.Add(at);
                                                db.SaveChanges();

                                                string filePath = System.IO.Path.Combine(Server.MapPath("/Uploads/"), "M+ANIL_1002788.jpg");
                                                string strEmployeeID = addId;
                                                strReq = "{ \"FaceInfo\": {\"employeeNo\": \"" + strEmployeeID + "\",\"faceLibType\": \"blackFD\" }}";
                                                strUrl = "http://" + device.DeviceIPAddress + ":" + device.Port + "/ISAPI/Intelligent/FDLib/FaceDataRecord?format=json&devIndex=" + index;
                                                strRsp = string.Empty;

                                                string fileKeyName = "FaceImage";
                                                NameValueCollection stringDict = new NameValueCollection();
                                                stringDict.Add("FaceDataRecord", strReq);

                                                http = new clienthttp();
                                                iRet = http.HttpPostData(device.UserName, device.Password, strUrl, fileKeyName, filePath, stringDict, ref strRsp);
                                                if (iRet != (int)HttpStatus.Http200)
                                                {
                                                    at = new ApiMonitorTB();
                                                    at.Command = "Face Upload";
                                                    at.Page = "Visitor";
                                                    at.time = DateTime.Now;
                                                    at.DeviceSRNO = device.DeviceSerialNo;
                                                    at.DeviceName = device.DeviceName;
                                                    at.EmpCode = addId;
                                                    at.EmpName = strName;
                                                    at.Status = "Success";
                                                    db.ApiMonitorTBs.Add(at);
                                                    db.SaveChanges();
                                                }
                                                else
                                                {
                                                    at = new ApiMonitorTB();
                                                    at.Command = "Face Upload";
                                                    at.Page = "Visitor";
                                                    at.time = DateTime.Now;
                                                    at.DeviceSRNO = device.DeviceSerialNo;
                                                    at.DeviceName = device.DeviceName;
                                                    at.EmpCode = addId;
                                                    at.EmpName = strName;
                                                    at.Status = "Failed";
                                                    db.ApiMonitorTBs.Add(at);
                                                    db.SaveChanges();
                                                }
                                            }
                                            else
                                            {
                                                at = new ApiMonitorTB();
                                                at.Command = "Add Card";
                                                at.Page = "Visitor";
                                                at.time = DateTime.Now;
                                                at.DeviceSRNO = device.DeviceSerialNo;
                                                at.DeviceName = device.DeviceName;
                                                at.EmpCode = addId;
                                                at.EmpName = strName;
                                                at.Status = "Failed";
                                                db.ApiMonitorTBs.Add(at);
                                                db.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            at = new ApiMonitorTB();
                                            at.Command = "Add Visitor";
                                            at.Page = "Visitor";
                                            at.time = DateTime.Now;
                                            at.DeviceSRNO = device.DeviceSerialNo;
                                            at.DeviceName = device.DeviceName;
                                            at.EmpCode = addId;
                                            at.EmpName = strName;
                                            at.Status = "Failed";
                                            db.ApiMonitorTBs.Add(at);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
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

                ViewBag.EmpList = new SelectList(GetEmployeeList(), "UserId", "Name");
                ViewBag.IdProofList = new SelectList(GetIdProofType(), "Name", "Name");
                ViewBag.MaterialList = new SelectList(GetMaterialList(), "Name", "Name");
                ViewBag.PriorityList = new SelectList(GetPrioritylList(), "Name", "Name");
                ViewBag.VisitorTypeList = new SelectList(GetVisitorTypeList(), "Name", "Name");
                ViewBag.VehicleTypeList = new SelectList(GetVehicleTypeList(), "Name", "Name");
                ViewBag.DeviceList = new SelectList(GetDeviceList(), "DeviceId", "DeviceName");
                return View("AddVisitor", visitor);
            }
            //return RedirectToAction("GetSupplier", new { userId = userid, userName = username });
            return RedirectToAction("GetVisitorList", "Visitor", new { userId = ViewBag.UserId, userName = ViewBag.UserName });
        }

        private static List<VisitorEntryModel> GetVisitors()
        {
            List<VisitorEntryModel> Model = new List<VisitorEntryModel>();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var visitors = entities.VisitorEntryTBs.ToList().OrderByDescending(d => d.Id);

                foreach (var item in visitors)
                {
                    VisitorEntryModel visitor = new VisitorEntryModel();
                    visitor.Id = item.Id;
                    visitor.VisitorId = item.VisitorId;
                    visitor.Name = item.Name;
                    visitor.Company = item.Company;
                    visitor.Contact = item.Contact;
                    var user = entities.UserTBs.Where(d => d.UserId == item.EmployeeId).FirstOrDefault();
                    visitor.EmployeeId = Convert.ToInt32(item.EmployeeId);
                    visitor.EmployeeName = user.FirstName + " " + user.LastName;
                    visitor.EmployeeDepartment = user.Department;
                    visitor.InTime = item.InTime;
                    visitor.OutTime = item.OutTime;
                    visitor.VisitDateFrom = Convert.ToDateTime(item.FromDate);
                    visitor.VisitDateTo = Convert.ToDateTime(item.ToDate);
                    visitor.Purpose = item.Purpose;
                    Model.Add(visitor);
                }
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        private static List<UserModel> GetEmployeeList()
        {
            List<UserModel> Model = new List<UserModel>();

            VMSDBEntities entities = new VMSDBEntities();


            try
            {
                var UserData = entities.UserTBs.Where(c => c.UserType == "Employee").ToList();

                foreach (var item in UserData)
                {
                    UserModel emp = new UserModel();
                    emp.UserId = item.UserId;
                    emp.Name = item.FirstName + " " + item.LastName;
                    Model.Add(emp);
                }

            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        private static List<IdProofModel> GetIdProofType()
        {
            List<IdProofModel> Model = new List<IdProofModel>();

            IdProofModel emp = new IdProofModel();
            emp.Id = 1;
            emp.Name = "Aadhar Card";
            Model.Add(emp);

            emp = new IdProofModel();
            emp.Id = 2;
            emp.Name = "Pan Card";
            Model.Add(emp);

            emp = new IdProofModel();
            emp.Id = 3;
            emp.Name = "Voter Card";
            Model.Add(emp);

            emp = new IdProofModel();
            emp.Id = 4;
            emp.Name = "Driving License";
            Model.Add(emp);

            return Model;
        }

        private static List<MaterialModel> GetMaterialList()
        {
            List<MaterialModel> Model = new List<MaterialModel>();

            MaterialModel emp = new MaterialModel();
            emp.Id = 1;
            emp.Name = "Bags";
            Model.Add(emp);

            emp = new MaterialModel();
            emp.Id = 2;
            emp.Name = "Laptop";
            Model.Add(emp);

            emp = new MaterialModel();
            emp.Id = 3;
            emp.Name = "Charger";
            Model.Add(emp);

            emp = new MaterialModel();
            emp.Id = 4;
            emp.Name = "Notebook";
            Model.Add(emp);

            return Model;
        }

        private static List<PriorityModel> GetPrioritylList()
        {
            List<PriorityModel> Model = new List<PriorityModel>();

            PriorityModel emp = new PriorityModel();
            emp.Id = 1;
            emp.Name = "Low";
            Model.Add(emp);

            emp = new PriorityModel();
            emp.Id = 2;
            emp.Name = "Medium";
            Model.Add(emp);

            emp = new PriorityModel();
            emp.Id = 3;
            emp.Name = "High";
            Model.Add(emp);

            return Model;
        }

        private static List<VisitorTypeModel> GetVisitorTypeList()
        {
            List<VisitorTypeModel> Model = new List<VisitorTypeModel>();

            VisitorTypeModel emp = new VisitorTypeModel();
            emp.Id = 1;
            emp.Name = "Permenent";
            Model.Add(emp);

            emp = new VisitorTypeModel();
            emp.Id = 2;
            emp.Name = "Regular";
            Model.Add(emp);

            emp = new VisitorTypeModel();
            emp.Id = 3;
            emp.Name = "OneTime";
            Model.Add(emp);

            return Model;
        }

        private static List<VehicleTypeModel> GetVehicleTypeList()
        {
            List<VehicleTypeModel> Model = new List<VehicleTypeModel>();

            VehicleTypeModel emp = new VehicleTypeModel();
            emp.Id = 1;
            emp.Name = "2 Wheeler";
            Model.Add(emp);

            emp = new VehicleTypeModel();
            emp.Id = 2;
            emp.Name = "4 Wheeler";
            Model.Add(emp);
            return Model;
        }

        private static List<DeviceModel> GetDeviceList()
        {
            VMSDBEntities db = new VMSDBEntities();
            List<DeviceModel> Model = new List<DeviceModel>();

            var dt = db.DevicesTBs.OrderBy(d => d.DeviceName).ToList();

            foreach (var item in dt)
            {
                DeviceModel emp = new DeviceModel();
                emp.DeviceId = item.DeviceId;
                emp.DeviceName = item.DeviceName;
                Model.Add(emp);
            }

            return Model;
        }

        public ActionResult GetVisitorData(string Contact)
        {
            VisitorModel visitor = new VisitorModel();

            visitor = GetVisitorDetails(Contact);

            return Json(visitor, JsonRequestBehavior.AllowGet);
        }

        private static VisitorModel GetVisitorDetails(string Contact)
        {
            VisitorModel Model = new VisitorModel();

            VMSDBEntities db = new VMSDBEntities();

            try
            {
                var visitor = db.VisitorTBs.Where(d => d.Contact == Contact).FirstOrDefault();

                Model.VisitorId = visitor.VisitorId;
                Model.Name = visitor.Name;
                Model.Company = visitor.Company;
                Model.EmailId = visitor.EmailId;
                Model.Contact = visitor.Contact;
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        public ActionResult GetEmployeeData(int EmployeeId)
        {
            GetEmpDataModel emp = new GetEmpDataModel();

            emp = GetEmpDetails(EmployeeId);

            return Json(emp, JsonRequestBehavior.AllowGet);
        }

        private static GetEmpDataModel GetEmpDetails(int EmployeeId)
        {
            GetEmpDataModel Model = new GetEmpDataModel();

            VMSDBEntities db = new VMSDBEntities();

            try
            {
                var emp = db.UserTBs.Where(d => d.UserId == EmployeeId).FirstOrDefault();

                Model.EmployeeId = emp.UserId;
                Model.Company = emp.Company;
                Model.Department = emp.Department;
                Model.Designation = emp.Designation;
                Model.EmployeeEmailId = emp.Email;
                Model.EmployeeContact = emp.Phone;
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        //for edit flow
        private static VisitorEntryModel GetVisitorDetails(int Id)
        {
            VisitorEntryModel Model = new VisitorEntryModel();

            VMSDBEntities entities = new VMSDBEntities();
            string baseadder = ConfigurationManager.AppSettings["WebUIUrl"];
            try
            {
                var visitor = entities.VisitorEntryTBs.Where(d => d.Id == Id).FirstOrDefault();

                Model.Id = visitor.Id;
                Model.VisitorId = visitor.VisitorId;
                Model.Name = visitor.Name;
                Model.Company = visitor.Company;
                Model.Contact = visitor.Contact;
                Model.EmailId = visitor.EmailId;
                Model.Address = visitor.Address;
                Model.VisitorType = visitor.VisitorType;
                var user = entities.UserTBs.Where(d => d.UserId == visitor.EmployeeId).FirstOrDefault();
                Model.EmployeeId = Convert.ToInt32(visitor.EmployeeId);
                Model.EmployeeName = user.FirstName + " " + user.LastName;
                Model.EmployeeDepartment = user.Department;
                Model.Department = user.Department;
                Model.Designation = user.Designation;
                Model.InTime = visitor.InTime;
                Model.OutTime = visitor.OutTime;
                Model.VisitDateFrom = visitor.FromDate;
                Model.VisitDateTo = visitor.ToDate;
                Model.Purpose = visitor.Purpose;
                Model.Remark = visitor.Remark;
                Model.Priority = visitor.Priority;
                Model.IdProof = visitor.IdProof;
                Model.IdProofNo = visitor.IdProofNumber;
                Model.Material = visitor.Material;
                Model.VehicleType = visitor.VehicleType;
                Model.VehicleNo = visitor.VehicleNo;
                Model.VehiclePUCNo = visitor.VehiclePUCNo;
                Model.PUCEndDate = visitor.PUCEndDate;
                Model.PhotoPath = visitor.Photo;
                Model.DeviceId = Convert.ToInt32(visitor.DeviceId);
                Model.CardNo = visitor.CardNo;

            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }
        [HttpPost]
        public JsonResult A_GetVisitorList()
        {
            List<VisitorEntryModel> Model = new List<VisitorEntryModel>();
            Model = GetVisitors();
            return Json(Model);
        }

        public ActionResult GetVisitorListSearch(VisitorSearchModel visitor)
        {
            visitorEntries = GetVisitors();

            if (!string.IsNullOrWhiteSpace(visitor.Contact))
            {
                visitorEntries = visitorEntries.Where(d => d.Contact.Contains(visitor.Contact)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(visitor.Company))
            {
                visitorEntries = visitorEntries.Where(d => d.Company.Contains(visitor.Company)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(visitor.Department))
            {
                visitorEntries = visitorEntries.Where(d => d.EmployeeDepartment.Contains(visitor.Department)).ToList();
            }

            if (!string.IsNullOrEmpty(visitor.FromDate) && string.IsNullOrEmpty(visitor.ToDate))
            {
                visitorEntries = visitorEntries.Where(d => d.VisitDateFrom <= Convert.ToDateTime(visitor.FromDate)).ToList();
            }

            if (!string.IsNullOrEmpty(visitor.FromDate) && !string.IsNullOrEmpty(visitor.ToDate))
            {
                visitorEntries = visitorEntries.Where(d => d.VisitDateFrom >= Convert.ToDateTime(visitor.FromDate) && d.VisitDateTo <= Convert.ToDateTime(visitor.ToDate)).ToList();
            }

            if (!string.IsNullOrEmpty(visitor.Time))
            {
                visitorEntries = visitorEntries.Where(d => Convert.ToDateTime(d.InTime) >= Convert.ToDateTime(visitor.Time) && Convert.ToDateTime(d.OutTime) <= Convert.ToDateTime(visitor.Time)).ToList();
            }

            return Json(visitorEntries, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public FileResult ExportToExcel()
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[10] { new DataColumn("VisitorId"),
                                            new DataColumn("VisitorName"),
                                            new DataColumn("Visitor Company"),
                                            new DataColumn("Visitor Contact"),
                                            new DataColumn("Employee Name"),
                                            new DataColumn("Department"),
                                            new DataColumn("In Time"),
                                            new DataColumn("Out Time"),
                                            new DataColumn("From Date"),
                                            new DataColumn("To Date"),});

            visitorEntries = GetVisitors();

            foreach (var item in visitorEntries)
            {
                string fdate = string.Format("{0:dd-MM-yyyy}", item.VisitDateFrom);
                dt.Rows.Add(item.VisitorId, item.Name, item.Company, item.Contact, item.EmployeeName, item.Department, item.InTime, item.OutTime, fdate, item.VisitDateTo);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Visitor.xlsx");
                }
            }
        }

        [HttpPost]
        public ActionResult ExportToPdf(int? pageNumber)
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[10] { new DataColumn("VisitorId"),
                                            new DataColumn("VisitorName"),
                                            new DataColumn("VisitorCompany"),
                                            new DataColumn("VisitorContact"),
                                            new DataColumn("EmployeeName"),
                                            new DataColumn("Department"),
                                            new DataColumn("InTime"),
                                            new DataColumn("OutTime"),
                                            new DataColumn("FromDate"),
                                            new DataColumn("ToDate"),});

            visitorEntries = GetVisitors();

            foreach (var item in visitorEntries)
            {
                string fdate = string.Format("{0:dd-MM-yyyy}", item.VisitDateFrom);
                string tdate = string.Format("{0:dd-MM-yyyy}", item.VisitDateTo);
                dt.Rows.Add(item.VisitorId, item.Name, item.Company, item.Contact, item.EmployeeName, item.Department, item.InTime, item.OutTime, fdate, tdate);
            }

            if (dt.Rows.Count > 0)
            {
                int pdfRowIndex = 1;

                string filename = "VisitorList-" + DateTime.Now.ToString("dd-MM-yyyy hh_mm_s_tt");
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

                table.AddCell(new Phrase("VisitorId", font1));
                table.AddCell(new Phrase("VisitorName", font1));
                table.AddCell(new Phrase("VisitorCompany", font1));
                table.AddCell(new Phrase("VisitorContact", font1));
                table.AddCell(new Phrase("EmployeeName", font1));
                table.AddCell(new Phrase("Department", font1));
                table.AddCell(new Phrase("InTime", font1));
                table.AddCell(new Phrase("OutTime", font1));
                table.AddCell(new Phrase("FromDate", font1));
                table.AddCell(new Phrase("ToDate", font1));
                table.HeaderRows = 1;

                foreach (DataRow data in dt.Rows)
                {
                    table.AddCell(new Phrase(data["VisitorId"].ToString(), font2));
                    table.AddCell(new Phrase(data["VisitorName"].ToString(), font2));
                    table.AddCell(new Phrase(data["VisitorCompany"].ToString(), font2));
                    table.AddCell(new Phrase(data["VisitorContact"].ToString(), font2));
                    table.AddCell(new Phrase(data["EmployeeName"].ToString(), font2));
                    table.AddCell(new Phrase(data["Department"].ToString(), font2));
                    table.AddCell(new Phrase(data["InTime"].ToString(), font2));
                    table.AddCell(new Phrase(data["OutTime"].ToString(), font2));
                    table.AddCell(new Phrase(data["FromDate"].ToString(), font2));
                    table.AddCell(new Phrase(data["ToDate"].ToString(), font2));

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
            return View("GetVisitorList");
        }

        public ActionResult VisitorRegister()
        {
            VisitorEntryModel visitor = new VisitorEntryModel();

            ViewBag.EmpList = new SelectList(GetEmployeeList(), "UserId", "Name");
            ViewBag.IdProofList = new SelectList(GetIdProofType(), "Name", "Name");
            ViewBag.MaterialList = new SelectList(GetMaterialList(), "Name", "Name");
            ViewBag.PriorityList = new SelectList(GetPrioritylList(), "Name", "Name");
            ViewBag.VisitorTypeList = new SelectList(GetVisitorTypeList(), "Name", "Name");
            ViewBag.VehicleTypeList = new SelectList(GetVehicleTypeList(), "Name", "Name");
            visitor.PUCEndDate = DateTime.Now;

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(VisitorEntryModel visitor)
        {
            if (visitor.Id == 0)
            {
                ModelState.Remove("Id");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    VisitorTB dt = new VisitorTB();

                    if (visitor.Id > 0)
                    {
                        var checkVisitor = db.VisitorEntryTBs.Where(d => d.Id == visitor.Id).FirstOrDefault();

                        checkVisitor.EmployeeId = visitor.EmployeeId;
                        checkVisitor.Name = visitor.Name;
                        checkVisitor.Company = visitor.Company;
                        checkVisitor.VisitorType = visitor.VisitorType;
                        checkVisitor.Address = visitor.Address;
                        checkVisitor.InTime = visitor.InTime;
                        checkVisitor.OutTime = visitor.OutTime;
                        checkVisitor.FromDate = visitor.VisitDateFrom;
                        checkVisitor.ToDate = visitor.VisitDateTo;
                        checkVisitor.IdProof = visitor.IdProof;
                        checkVisitor.IdProofNumber = visitor.IdProofNo;
                        checkVisitor.Photo = visitor.PhotoPath;
                        checkVisitor.EmailId = visitor.EmailId;
                        checkVisitor.Contact = visitor.Contact;
                        checkVisitor.Purpose = visitor.Purpose;
                        checkVisitor.Remark = visitor.Remark;
                        checkVisitor.Priority = visitor.Priority;
                        checkVisitor.Material = visitor.Material;
                        checkVisitor.VehicleType = visitor.VehicleType;
                        checkVisitor.VehicleNo = visitor.VehicleNo;
                        checkVisitor.VehiclePUCNo = visitor.VehiclePUCNo;
                        checkVisitor.PUCEndDate = visitor.PUCEndDate != null ? visitor.PUCEndDate : null;
                        db.Entry(checkVisitor).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        var visitordata = db.VisitorTBs.Where(d => d.Contact == visitor.Contact).FirstOrDefault();

                        if (visitordata != null)
                        {
                            VisitorEntryTB tB = new VisitorEntryTB();
                            tB.EmployeeId = visitor.EmployeeId;
                            tB.VisitorId = visitordata.VisitorId;
                            tB.Name = visitor.Name;
                            tB.Company = visitor.Company;
                            tB.VisitorType = visitor.VisitorType;
                            tB.Address = visitor.Address;
                            tB.InTime = visitor.InTime;
                            tB.OutTime = visitor.OutTime;
                            tB.FromDate = visitor.VisitDateFrom;
                            tB.ToDate = visitor.VisitDateTo;
                            tB.IdProof = visitor.IdProof;
                            tB.IdProofNumber = visitor.IdProofNo;
                            tB.Photo = visitor.PhotoPath;
                            tB.EmailId = visitor.EmailId;
                            tB.Contact = visitor.Contact;
                            tB.Purpose = visitor.Purpose;
                            tB.Remark = visitor.Remark;
                            tB.Priority = visitor.Priority;
                            tB.Material = visitor.Material;
                            tB.VehicleType = visitor.VehicleType;
                            tB.VehicleNo = visitor.VehicleNo;
                            tB.VehiclePUCNo = visitor.VehiclePUCNo;
                            tB.PUCEndDate = visitor.PUCEndDate != null ? visitor.PUCEndDate : null;
                            db.VisitorEntryTBs.Add(tB);
                            db.SaveChanges();
                        }
                        else
                        {
                            Random generator = new Random();
                            string rand = generator.Next(0, 1000000).ToString("D6");
                            dt.VisitorId = rand;
                            dt.Name = visitor.Name;
                            dt.Contact = visitor.Contact;
                            dt.EmailId = visitor.EmailId;
                            dt.Company = visitor.Company;
                            dt.Photo = visitor.PhotoPath;
                            db.VisitorTBs.Add(dt);
                            db.SaveChanges();

                            VisitorEntryTB tB = new VisitorEntryTB();
                            tB.EmployeeId = visitor.EmployeeId;
                            tB.VisitorId = dt.VisitorId;
                            tB.Name = visitor.Name;
                            tB.Company = visitor.Company;
                            tB.VisitorType = visitor.VisitorType;
                            tB.Address = visitor.Address;
                            tB.InTime = visitor.InTime;
                            tB.OutTime = visitor.OutTime;
                            tB.FromDate = visitor.VisitDateFrom;
                            tB.ToDate = visitor.VisitDateTo;
                            tB.IdProof = visitor.IdProof;
                            tB.IdProofNumber = visitor.IdProofNo;
                            tB.Photo = visitor.PhotoPath;
                            tB.EmailId = visitor.EmailId;
                            tB.Contact = visitor.Contact;
                            tB.Purpose = visitor.Purpose;
                            tB.Remark = visitor.Remark;
                            tB.Priority = visitor.Priority;
                            tB.Material = visitor.Material;
                            tB.VehicleType = visitor.VehicleType;
                            tB.VehicleNo = visitor.VehicleNo;
                            tB.VehiclePUCNo = visitor.VehiclePUCNo;
                            tB.PUCEndDate = visitor.PUCEndDate != null ? visitor.PUCEndDate : null;
                            db.VisitorEntryTBs.Add(tB);
                            db.SaveChanges();
                        }
                    }

                    var maildata = db.MailSettingTBs.FirstOrDefault();
                    string Host = maildata.Host;
                    string from = maildata.smtpfrom;
                    int Port = Convert.ToInt32(maildata.port);
                    string Username = maildata.username;
                    string pass = maildata.password;

                    string bodystring = string.Empty;

                    if (maildata != null)
                    {
                        #region send mail to visitor
                        if (visitor.EmailId != null)
                        {
                            //// Hader start
                            bodystring = "<!DOCTYPE html><html><head><title> VMS : Visit Email</title> </head> <body>";
                            bodystring += "<table width='100%' border='0' cellspacing='0' cellpadding='0'><tbody> ";
                            /// hader end

                            bodystring += "<tr><td align='center'>";
                            bodystring += "<table width='92% ' border='0' cellspacing='0' cellpadding='0' bgcolor='#ffffff' style='border-radius:10px; padding: 15px'>";
                            bodystring += "<tbody>  <tr> <td align='center' valign='top'>";
                            bodystring += "<table width='100% ' border='0' cellspacing='0' cellpadding='0' style='border - radius:10px; border: 2px solid #eac356; padding: 0 25px;'>";
                            bodystring += "<tbody>  <tr> <td>&nbsp;</td>";
                            bodystring += "<td style='font - family:Arial; font - size:15px; line - height:21px; color:#000000'><strong style='font-weight:200;display: block; font-size: 30px; line-height: 3;text-align: center;'> Visit </strong>You recently requested to visit through VMS. <br/>";
                            bodystring += "</td>";
                            bodystring += "<td>&nbsp;</td></tr>";

                            bodystring += "<tr> <td>&nbsp;</td> <td>&nbsp;</td>  <td>&nbsp;</td> </tr>";
                            bodystring += "<tr> <td>&nbsp;</td><td height='30'  valign='bottom' style='font - family:Arial; font - size:13px; line - height:20px; color:#393a3c'>Thank you & Regards, <br/> <strong>Team VMS</strong>.</td><td>&nbsp;</td></tr>";
                            bodystring += "<tr><td>&nbsp;</td>  <td>&nbsp;</td>  <td>&nbsp;</td>  </tr>";
                            bodystring += "<tr><td>&nbsp;</td>  <td>&nbsp;</td>  <td>&nbsp;</td>  </tr>";
                            bodystring += " </tbody>  </table> </td> </tr>";
                            bodystring += "</tbody> </table>  </td> </tr>";
                            bodystring += " </td></tr>";
                            bodystring += "<tr> <td height='25'>&nbsp;</td>  </tr>";
                            bodystring += "</tbody>  </table> </td> </tr> </tbody>  </table> </body> </html>";

                            //var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                            //string Host = smtpSection.Network.Host;
                            //string from = smtpSection.From;
                            //int Port = smtpSection.Network.Port;
                            //string Username = smtpSection.Network.UserName;
                            //string pass = smtpSection.Network.Password;

                            SmtpClient SmtpServer = new SmtpClient(Host);

                            MailMessage mails = new MailMessage();
                            mails.From = new MailAddress(from);
                            mails.To.Add(visitor.EmailId);
                            // mails.To.Add("Vishakhasupnekar1307@gmail.com");
                            mails.IsBodyHtml = true;
                            mails.Subject = "Visit Managment system";
                            mails.Body = bodystring;
                            SmtpServer.Port = Port;
                            SmtpServer.UseDefaultCredentials = true;
                            SmtpServer.Credentials = new System.Net.NetworkCredential(Username, pass);
                            SmtpServer.EnableSsl = true;
                            SmtpServer.Send(mails);
                        }
                        #endregion

                        #region send mail to Employee

                        var user = db.UserTBs.Where(d => d.UserId == visitor.EmployeeId).FirstOrDefault();

                        if (user.Email != null)
                        {
                            bodystring = string.Empty;

                            //// Hader start
                            bodystring = "<!DOCTYPE html><html><head><title> VMS : Visitor Request</title> </head> <body>";
                            bodystring += "<table width='100%' border='0' cellspacing='0' cellpadding='0'><tbody> ";
                            /// hader end

                            bodystring += "<tr><td align='center'>";
                            bodystring += "<table width='92% ' border='0' cellspacing='0' cellpadding='0' bgcolor='#ffffff' style='border-radius:10px; padding: 15px'>";
                            bodystring += "<tbody>  <tr> <td align='center' valign='top'>";
                            bodystring += "<table width='100% ' border='0' cellspacing='0' cellpadding='0' style='border - radius:10px; border: 2px solid #eac356; padding: 0 25px;'>";
                            bodystring += "<tbody>  <tr> <td>&nbsp;</td>";
                            bodystring += "<td style='font - family:Arial; font - size:15px; line - height:21px; color:#000000'><strong style='font-weight:200;display: block; font-size: 30px; line-height: 3;text-align: center;'> VMS </strong>Vistor makes request to meet you through VMS. <br/>";
                            bodystring += "</td>";
                            bodystring += "<td>&nbsp;</td></tr>";

                            bodystring += "<tr> <td>&nbsp;</td> <td>&nbsp;</td>  <td>&nbsp;</td> </tr>";
                            bodystring += "<tr> <td>&nbsp;</td><td height='30'  valign='bottom' style='font - family:Arial; font - size:13px; line - height:20px; color:#393a3c'>Thank you & Regards, <br/> <strong>Team VMS</strong>.</td><td>&nbsp;</td></tr>";
                            bodystring += "<tr><td>&nbsp;</td>  <td>&nbsp;</td>  <td>&nbsp;</td>  </tr>";
                            bodystring += "<tr><td>&nbsp;</td>  <td>&nbsp;</td>  <td>&nbsp;</td>  </tr>";
                            bodystring += " </tbody>  </table> </td> </tr>";
                            bodystring += "</tbody> </table>  </td> </tr>";
                            bodystring += " </td></tr>";
                            bodystring += "<tr> <td height='25'>&nbsp;</td>  </tr>";
                            bodystring += "</tbody>  </table> </td> </tr> </tbody>  </table> </body> </html>";

                            //smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                            //Host = smtpSection.Network.Host;
                            //from = smtpSection.From;
                            //Port = smtpSection.Network.Port;
                            //Username = smtpSection.Network.UserName;
                            //pass = smtpSection.Network.Password;

                            SmtpClient SmtpServer = new SmtpClient(Host);

                            MailMessage mails = new MailMessage();
                            mails.From = new MailAddress(from);
                            mails.To.Add(user.Email);
                            //mails.To.Add("amy21690@gmail.com");
                            mails.IsBodyHtml = true;
                            mails.Subject = "Visit Managment system";
                            mails.Body = bodystring;
                            SmtpServer.Port = Port;
                            SmtpServer.Credentials = new System.Net.NetworkCredential(Username, pass);
                            SmtpServer.EnableSsl = true;
                            SmtpServer.Send(mails);
                        }

                        #endregion
                    }

                    ViewBag.Message = "saved";
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

                ViewBag.EmpList = new SelectList(GetEmployeeList(), "UserId", "Name");
                ViewBag.IdProofList = new SelectList(GetIdProofType(), "Name", "Name");
                ViewBag.MaterialList = new SelectList(GetMaterialList(), "Name", "Name");
                ViewBag.PriorityList = new SelectList(GetPrioritylList(), "Name", "Name");
                ViewBag.VisitorTypeList = new SelectList(GetVisitorTypeList(), "Name", "Name");
                ViewBag.VehicleTypeList = new SelectList(GetVehicleTypeList(), "Name", "Name");
                return View("VisitorRegister", visitor);
            }

            ViewBag.EmpList = new SelectList(GetEmployeeList(), "UserId", "Name");
            ViewBag.IdProofList = new SelectList(GetIdProofType(), "Name", "Name");
            ViewBag.MaterialList = new SelectList(GetMaterialList(), "Name", "Name");
            ViewBag.PriorityList = new SelectList(GetPrioritylList(), "Name", "Name");
            ViewBag.VisitorTypeList = new SelectList(GetVisitorTypeList(), "Name", "Name");
            ViewBag.VehicleTypeList = new SelectList(GetVehicleTypeList(), "Name", "Name");

            return View("VisitorRegister");
        }
    }
}