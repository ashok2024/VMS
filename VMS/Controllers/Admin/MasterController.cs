using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LinqToExcel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using VMS.Controllers.Account;
using VMS.Middleware;
using VMS.Models;
using VMS.Models.Account;
using VMS.Models.Admin;
using VMS.Models.Employee;
using VMS.Models.Visitor;

namespace VMS.Controllers.Admin
{
    public class MasterController : BaseController
    {
        List<EmployeeModel> employees;

        public MasterController()
        {
            employees = new List<EmployeeModel>();
        }

        // GET: Master
        public ActionResult Index()
        {
            return View();
        }

        #region Company Master
        [HttpGet]
        public ActionResult Company()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "CompanyList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewData["GetCompanyList"] = GetCompanyList();
                return View();
            }
        }
        
        private static List<CompanyModel> GetCompanyList()
        {
            List<CompanyModel> Model = new List<CompanyModel>();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var emp = entities.CompanyTBs.ToList().OrderBy(d => d.Name);

                foreach (var item in emp)
                {
                    CompanyModel employee = new CompanyModel();
                    employee.Id = item.Id;
                    employee.Name = item.Name;
                    employee.ContactPerson = item.ContactPerson;
                    employee.Phone = item.Phone;
                    employee.Address = item.Address;
                    Model.Add(employee);
                }
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }        
        [HttpPost]
        public JsonResult AjaxMethod()
        {
            VMSDBEntities entities = new VMSDBEntities();
            List<CompanyModel> Model = new List<CompanyModel>();
            var emp = entities.CompanyTBs.ToList().OrderBy(d => d.Name);

            foreach (var item in emp)
            {
                CompanyModel company = new CompanyModel();
                company.Id = item.Id;
                company.Name = item.Name;
                company.ContactPerson = item.ContactPerson;
                company.Phone = item.Phone;
                company.Address = item.Address;              
                Model.Add(company);               
            }
            return Json(Model);
        }      
        [HttpGet]
        public ActionResult AddCompany(int Id)
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "CompanyList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                CompanyModel company = new CompanyModel();

                if (Id > 0)
                {
                    company = GetCompanyDetails(Id);
                }
                return View("AddCompany", company);
            }
        }

        private static CompanyModel GetCompanyDetails(int Id)
        {
            CompanyModel Model = new CompanyModel();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var emp = entities.CompanyTBs.Where(d => d.Id == Id).FirstOrDefault();

                Model.Id = emp.Id;
                Model.Name = emp.Name;
                Model.ContactPerson = emp.ContactPerson;
                Model.Phone = emp.Phone;
                Model.Address = emp.Address;
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        [HttpPost]
        public async Task<ActionResult> AddCompany(CompanyModel emp)
        {
            string userid = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            string username = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserId = userid;
            ViewBag.UserName = username;

            ViewBag.ActivePage = "CompanyList";


            string sessionid = (System.Web.HttpContext.Current.Session["SessionId"] != null) ? System.Web.HttpContext.Current.Session["SessionId"].ToString() : String.Empty;
            if (string.IsNullOrEmpty(sessionid))
            {
                return RedirectToAction("LogOut", "Account");
            }
            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    VMSDBEntities db = new VMSDBEntities();

                    CompanyTB tB = new CompanyTB();

                    if (emp.Id > 0)
                    {
                        var companydt = db.CompanyTBs.Where(d => d.Id == emp.Id).FirstOrDefault();

                        companydt.Name = emp.Name;
                        companydt.ContactPerson = emp.ContactPerson;
                        companydt.Phone = emp.Phone;
                        companydt.Address = emp.Address;
                        db.Entry(companydt).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        tB.Name = emp.Name;
                        tB.ContactPerson = emp.ContactPerson;
                        tB.Phone = emp.Phone;
                        tB.Address = emp.Address;
                        db.CompanyTBs.Add(tB);
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

                return View("AddCompany", emp);
            }
            return RedirectToAction("Company", "Master", new { userId = ViewBag.UserId, userName = ViewBag.UserName });
        }
        #endregion

        #region Branch Master

        [HttpPost]
        public JsonResult A_GetBranchList()
        {            
            List<BranchModel> Model = new List<BranchModel>();
            Model = GetBranchList();
            return Json(Model);
        }
        [HttpGet]
        public ActionResult Branch()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "BranchList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewData["GetBranchList"] = GetBranchList();
                return View();
            }
        }

        private static List<BranchModel> GetBranchList()
        {
            List<BranchModel> Model = new List<BranchModel>();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var emp = (from d in entities.BranchTBs
                           join c in entities.CompanyTBs on d.CompanyId equals c.Id
                           select new
                           {
                               d.Id,
                               d.Name,
                               companyId = c.Id,
                               company = c.Name,
                               contactperson = d.ContactPerson,
                               phone = d.Phone,
                               address = d.Address
                           }).ToList();

                foreach (var item in emp)
                {
                    BranchModel employee = new BranchModel();
                    employee.Id = item.Id;
                    employee.Name = item.Name;
                    employee.CompanyId = item.companyId;
                    employee.CompanyName = item.company;
                    employee.ContactPerson = item.contactperson;
                    employee.Phone = item.phone;
                    employee.Address = item.address;
                    Model.Add(employee);
                }
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        [HttpGet]
        public ActionResult AddBranch(int Id)
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "BranchList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                BranchModel branch = new BranchModel();

                if (Id > 0)
                {
                    branch = GetBranchDetails(Id);
                }

                ViewBag.CompanyList = new SelectList(GetCompanyList(), "Id", "Name");
                return View("AddBranch", branch);
            }
        }

        private static BranchModel GetBranchDetails(int Id)
        {
            BranchModel Model = new BranchModel();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var emp = (from d in entities.BranchTBs
                           join c in entities.CompanyTBs on d.CompanyId equals c.Id
                           where d.Id == Id
                           select new
                           {
                               d.Id,
                               d.Name,
                               companyId = c.Id,
                               company = c.Name,
                               contactperson = d.ContactPerson,
                               phone = d.Phone,
                               address = d.Address
                           }).FirstOrDefault();

                Model.Id = emp.Id;
                Model.Name = emp.Name;
                Model.CompanyId = emp.companyId;
                Model.CompanyName = emp.company;
                Model.ContactPerson = emp.contactperson;
                Model.Phone = emp.phone;
                Model.Address = emp.address;
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        [HttpPost]
        public async Task<ActionResult> AddBranch(BranchModel emp)
        {
            string userid = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            string username = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserId = userid;
            ViewBag.UserName = username;

            ViewBag.ActivePage = "BranchList";

            string sessionid = (System.Web.HttpContext.Current.Session["SessionId"] != null) ? System.Web.HttpContext.Current.Session["SessionId"].ToString() : String.Empty;
            if (string.IsNullOrEmpty(sessionid))
            {
                return RedirectToAction("LogOut", "Account");
            }
            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    VMSDBEntities db = new VMSDBEntities();

                    BranchTB tB = new BranchTB();

                    if (emp.Id > 0)
                    {
                        var companydt = db.BranchTBs.Where(d => d.Id == emp.Id).FirstOrDefault();

                        companydt.Name = emp.Name;
                        companydt.CompanyId = emp.CompanyId;
                        companydt.ContactPerson = emp.ContactPerson;
                        companydt.Phone = emp.Phone;
                        companydt.Address = emp.Address;
                        db.Entry(companydt).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        tB.Name = emp.Name;
                        tB.CompanyId = emp.CompanyId;
                        tB.ContactPerson = emp.ContactPerson;
                        tB.Phone = emp.Phone;
                        tB.Address = emp.Address;                        
                        db.BranchTBs.Add(tB);
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

                ViewBag.CompanyList = new SelectList(GetCompanyList(), "Id", "Name");
                return View("AddBranch", emp);
            }
            return RedirectToAction("Branch", "Master", new { userId = ViewBag.UserId, userName = ViewBag.UserName });
        }
        #endregion

        #region Employee Master
        [HttpGet]
        public ActionResult Employee()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "Employee";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewData["GetEmployeeList"] = GetEmployeeList();

                SessionModel emp = new SessionModel();
                emp.UserId = userId;
                emp.UserName = userName;
                return View("Employee",emp);
            }
        }

        private static List<EmployeeModel> GetEmployeeList()
        {
            List<EmployeeModel> Model = new List<EmployeeModel>();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var emp = entities.UserTBs.Where(d => d.UserType != "Admin").ToList().OrderByDescending(d => d.UserId);

                foreach (var item in emp)
                {
                    EmployeeModel employee = new EmployeeModel();
                    employee.UserId = item.UserId;
                    employee.EmpCode = item.EmpCode;
                    employee.Name = item.FirstName + " " + item.LastName;
                    employee.FirstName = item.FirstName;
                    employee.LastName = item.LastName;
                    employee.CompanyId = item.CompanyId;
                    employee.Company = entities.CompanyTBs.Where(d => d.Id == item.CompanyId).Select(x => x.Name).FirstOrDefault();
                    employee.BranchId = item.BranchId;
                    employee.Branch = entities.BranchTBs.Where(d => d.Id == item.BranchId).Select(x => x.Name).FirstOrDefault();
                    employee.DepartmentId = item.DepartmentId;
                    employee.Department = entities.DepartmentTBs.Where(d => d.Id == item.DepartmentId).Select(x => x.Name).FirstOrDefault();
                    employee.DesignationId = item.DesignationId;
                    employee.Designation = entities.DesignationTBs.Where(d => d.Id == item.DesignationId).Select(x => x.Name).FirstOrDefault();
                    employee.Phone = item.Phone;
                    employee.Email = item.Email;
                    employee.BirthDate = Convert.ToDateTime(item.BirthDate);
                    employee.Password = item.Password;
                    employee.Address = item.Address;
                    employee.UserType = item.UserType;
                    employee.IdProofNo = item.IdProofNumber;
                    employee.PhotoPath = item.Photo;
                    employee.DeviceId = Convert.ToInt32(item.DeviceId);
                    employee.CardNo = item.CardNo;
                    Model.Add(employee);
                }
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        // GET: Employee
        [HttpGet]
        public ActionResult AddEmployee(int EmpId)
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "AddEmployee";

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
                AddEmployeeModel emp = new AddEmployeeModel();

                List<UserTypeModel> usertypeList = new List<UserTypeModel>();
                UserTypeModel userType = new UserTypeModel();
                userType.Id = 1;
                userType.Name = "Employee";
                usertypeList.Add(userType);

                userType = new UserTypeModel();
                userType.Id = 2;
                userType.Name = "Reception";
                usertypeList.Add(userType);

                if (UserId > 0)
                {
                    emp = GetEmployeeDetails(EmpId);
                }

                ViewBag.CompanyList = new SelectList(GetCompanyList(), "Id", "Name");
                ViewBag.BranchList = new SelectList(GetBranchList(), "Id", "Name");
                ViewBag.DepartmentList = new SelectList(GetDepartmentList(), "Id", "Name");
                ViewBag.DesignationList = new SelectList(GetDesignationList(), "Id", "Name");
                ViewBag.UserTypeList = new SelectList(usertypeList, "Name", "Name");
                ViewBag.DeviceList = new SelectList(GetDeviceList(), "DeviceId", "DeviceName");

                return View("AddEmployee", emp);
            }
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

        private static AddEmployeeModel GetEmployeeDetails(int UserId)
        {
            AddEmployeeModel Model = new AddEmployeeModel();
            if (UserId > 0)
            {
                VMSDBEntities entities = new VMSDBEntities();
                string baseadder = ConfigurationManager.AppSettings["WebUIUrl"];

                try
                {
                    var emp = entities.UserTBs.Where(d => d.UserId == UserId).FirstOrDefault();

                    Model.EmpCode = emp.EmpCode;
                    Model.UserId = emp.UserId;
                    Model.Name = emp.Name;
                    Model.EmailId = emp.Email;
                    Model.Contact = emp.Phone;
                    Model.BirthDate = Convert.ToDateTime(emp.BirthDate);
                    Model.Address = emp.Address;
                    Model.CompanyId = emp.CompanyId;
                    Model.Company = entities.CompanyTBs.Where(d => d.Id == emp.CompanyId).Select(x => x.Name).FirstOrDefault();
                    Model.BranchId = emp.BranchId;
                    Model.Branch = entities.BranchTBs.Where(d => d.Id == emp.BranchId).Select(x => x.Name).FirstOrDefault();
                    Model.DepartmentId = emp.DepartmentId;
                    Model.Department = entities.DepartmentTBs.Where(d => d.Id == emp.DepartmentId).Select(x => x.Name).FirstOrDefault();
                    Model.DesignationId = emp.DesignationId;
                    Model.Designation = entities.DesignationTBs.Where(d => d.Id == emp.DesignationId).Select(x => x.Name).FirstOrDefault();
                    Model.UserName = emp.UserName;
                    Model.Password = emp.Password;
                    Model.UserType = emp.UserType;
                    Model.IdProofNo = emp.IdProofNumber;
                    Model.PhotoPath = emp.Photo;
                    Model.IdProofNo = emp.IdProofNumber;
                    Model.DeviceId = Convert.ToInt32(emp.DeviceId);
                    Model.CardNo = emp.CardNo;
                }
                catch (Exception ex)
                {
                    return Model;
                    throw ex;
                }
            }
            return Model;
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
                    var _comPath = Server.MapPath("/Uploads/emp_") + _imgname + _ext;
                    _imgname = "emp_" + _imgname + _ext;

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
        public async Task<ActionResult> AddEmployee(AddEmployeeModel emp)
        {
            string userid = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            string username = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserId = userid;
            ViewBag.UserName = username;

            ViewBag.ActivePage = "AddEmployee";

            //string sessionid = (System.Web.HttpContext.Current.Session["SessionId"] != null) ? System.Web.HttpContext.Current.Session["SessionId"].ToString() : String.Empty;
            //if (string.IsNullOrEmpty(sessionid))
            //{
            //    return RedirectToAction("LogOut", "Account");
            //}
            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }

            if (emp.UserId > 0)
            {

            }
            else
            {
                if (emp.UserName == null)
                {
                    ModelState.AddModelError("UserName", "please enter Username");
                }
                if (emp.Password == null)
                {
                    ModelState.AddModelError("Password", "please enter Password");
                }
            }

            List<UserTypeModel> usertypeList = new List<UserTypeModel>();
            UserTypeModel userType = new UserTypeModel();

            if (ModelState.IsValid)
            {
                try
                {
                    VMSDBEntities db = new VMSDBEntities();

                    UserTB tB = new UserTB();
                    
                    if (emp.UserId > 0)
                    {
                        var userdt = db.UserTBs.Where(d => d.UserId == emp.UserId).FirstOrDefault();

                        userdt.EmpCode = emp.EmpCode;
                        userdt.Name = emp.Name;
                        userdt.FirstName = Convert.ToString(emp.Name.Split(' ')[0]);
                        userdt.Email = emp.EmailId;
                        userdt.Phone = emp.Contact;
                        userdt.BirthDate = emp.BirthDate;
                        userdt.Address = emp.Address;
                        userdt.CompanyId = emp.CompanyId;
                        userdt.BranchId = emp.BranchId;
                        userdt.DepartmentId = emp.DepartmentId;
                        userdt.DesignationId = emp.DesignationId;
                        userdt.UserType = emp.UserType;
                        userdt.DeviceId = emp.DeviceId;
                        userdt.CardNo = emp.CardNo;
                        userdt.IdProofNumber = emp.IdProofNo;
                        userdt.Photo = emp.PhotoPath;
                        db.Entry(userdt).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        tB.EmpCode = emp.EmpCode;
                        tB.Name = emp.Name;
                        tB.FirstName = Convert.ToString(emp.Name.Split(' ')[0]);
                        tB.Email = emp.EmailId;
                        tB.Phone = emp.Contact;
                        tB.BirthDate = emp.BirthDate;
                        tB.Address = emp.Address;
                        tB.CompanyId = emp.CompanyId;
                        tB.BranchId = emp.BranchId;
                        tB.DepartmentId = emp.DepartmentId;
                        tB.DesignationId = emp.DesignationId;
                        tB.UserName = emp.UserName;
                        tB.Password = emp.Password;
                        tB.UserType = emp.UserType;
                        tB.DeviceId = emp.DeviceId;
                        tB.CardNo = emp.CardNo;
                        tB.IdProofNumber = emp.IdProofNo;
                        tB.Photo = emp.PhotoPath;
                        db.UserTBs.Add(tB);
                        db.SaveChanges();
                    }

                    #region call punch apis

                    var device = db.DevicesTBs.Where(d => d.DeviceId == emp.DeviceId).FirstOrDefault();

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
                                        at.Page = "Employee";
                                        at.time = DateTime.Now;
                                        at.DeviceSRNO = device.DeviceSerialNo;
                                        at.DeviceName = device.DeviceName;
                                        at.EmpCode = "";
                                        at.EmpName = "";
                                        at.Status = "Success";
                                        db.ApiMonitorTBs.Add(at);
                                        db.SaveChanges();

                                        var devicedt = item.Device;

                                        DateTime fdate = Convert.ToDateTime(DateTime.Now);
                                       // DateTime ftime = Convert.ToDateTime(DateTime.Now);
                                        string BeginTime = fdate.Year + "-" + fdate.Month.ToString("d2") + "-" + fdate.Day.ToString("d2") + "T" + fdate.Hour.ToString("d2") + ":" + fdate.Minute.ToString("d2") + ":" + fdate.Second.ToString("d2");

                                        DateTime tdate = Convert.ToDateTime(DateTime.Now);
                                       // DateTime ttime = Convert.ToDateTime(visitor.OutTime);
                                        string endTime = tdate.Year + "-" + tdate.Month.ToString("d2") + "-" + tdate.Day.ToString("d2") + "T" + tdate.Hour.ToString("d2") + ":" + tdate.Minute.ToString("d2") + ":" + tdate.Second.ToString("d2");


                                        string strName = emp.Name;
                                        string strUserType = "normal";
                                        string index = devicedt.devIndex;
                                        string strReq = "{\"UserInfo\" : [{\"employeeNo\": \"" + emp.EmpCode + "\",\"name\": \"" + strName + "\",\"userType\": \"" + strUserType + "\",\"Valid\" : {\"enable\": true,\"beginTime\": \"" + BeginTime + "\",\"endTime\": \"" + endTime + "\",\"timeType\" : \"local\"}}]}";
                                        string strUrl = "http://" + device.DeviceIPAddress + ":" + device.Port + "/ISAPI/AccessControl/UserInfo/Record?format=json&devIndex=" + index;
                                        string strRsp = string.Empty;

                                        clienthttp http = new clienthttp();
                                        int iRet = http.HttpRequest(device.UserName, device.Password, strUrl, "POST", strReq, ref strRsp);
                                        if (iRet == (int)HttpStatus.Http200)
                                        {
                                            at = new ApiMonitorTB();
                                            at.Command = "Add Employee";
                                            at.Page = "Employee";
                                            at.time = DateTime.Now;
                                            at.DeviceSRNO = device.DeviceSerialNo;
                                            at.DeviceName = device.DeviceName;
                                            at.EmpCode = emp.EmpCode;
                                            at.EmpName = strName;
                                            at.Status = "Success";
                                            db.ApiMonitorTBs.Add(at);
                                            db.SaveChanges();

                                            //for add card no
                                            strReq = "{\"CardInfo\":{\"employeeNo\":\"" + emp.EmpCode + "\",\"cardNo\":\"" + emp.CardNo + "\",\"cardType\":\"normalCard\"}}";
                                            strUrl = "http://" + device.DeviceIPAddress + ":" + device.Port + "/ISAPI/AccessControl/CardInfo/Record?format=json&devIndex=" + index;
                                            strRsp = string.Empty;

                                            http = new clienthttp();
                                            iRet = http.HttpRequest(device.UserName, device.Password, strUrl, "POST", strReq, ref strRsp);

                                            if (iRet == (int)HttpStatus.Http200)
                                            {
                                                at = new ApiMonitorTB();
                                                at.Command = "Add Card";
                                                at.Page = "Employee";
                                                at.time = DateTime.Now;
                                                at.DeviceSRNO = device.DeviceSerialNo;
                                                at.DeviceName = device.DeviceName;
                                                at.EmpCode = emp.EmpCode;
                                                at.EmpName = strName;
                                                at.Status = "Success";
                                                db.ApiMonitorTBs.Add(at);
                                                db.SaveChanges();

                                                string filePath = System.IO.Path.Combine(Server.MapPath("/Uploads/"), "M+ANIL_1002788.jpg");
                                                string strEmployeeID = emp.EmpCode;
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
                                                    at.Page = "Employee";
                                                    at.time = DateTime.Now;
                                                    at.DeviceSRNO = device.DeviceSerialNo;
                                                    at.DeviceName = device.DeviceName;
                                                    at.EmpCode = strEmployeeID;
                                                    at.EmpName = strName;
                                                    at.Status = "Success";
                                                    db.ApiMonitorTBs.Add(at);
                                                    db.SaveChanges();
                                                }
                                                else
                                                {
                                                    at = new ApiMonitorTB();
                                                    at.Command = "Face Upload";
                                                    at.Page = "Employee";
                                                    at.time = DateTime.Now;
                                                    at.DeviceSRNO = device.DeviceSerialNo;
                                                    at.DeviceName = device.DeviceName;
                                                    at.EmpCode = strEmployeeID;
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
                                                at.Page = "Employee";
                                                at.time = DateTime.Now;
                                                at.DeviceSRNO = device.DeviceSerialNo;
                                                at.DeviceName = device.DeviceName;
                                                at.EmpCode = emp.EmpCode;
                                                at.EmpName = strName;
                                                at.Status = "Failed";
                                                db.ApiMonitorTBs.Add(at);
                                                db.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            at = new ApiMonitorTB();
                                            at.Command = "Add Employee";
                                            at.Page = "Employee";
                                            at.time = DateTime.Now;
                                            at.DeviceSRNO = device.DeviceSerialNo;
                                            at.DeviceName = device.DeviceName;
                                            at.EmpCode = emp.EmpCode;
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
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage);
                        }
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

                
                userType.Id = 1;
                userType.Name = "Employee";
                usertypeList.Add(userType);

                userType = new UserTypeModel();
                userType.Id = 2;
                userType.Name = "Reception";
                usertypeList.Add(userType);

                ViewBag.CompanyList = new SelectList(GetCompanyList(), "Id", "Name");
                ViewBag.BranchList = new SelectList(GetBranchList(), "Id", "Name");
                ViewBag.DepartmentList = new SelectList(GetDepartmentList(), "Id", "Name");
                ViewBag.DesignationList = new SelectList(GetDesignationList(), "Id", "Name");
                ViewBag.UserTypeList = new SelectList(usertypeList, "Name", "Name");
                ViewBag.DeviceList = new SelectList(GetDeviceList(), "DeviceId", "DeviceName");

                ViewBag.Result = "somthing went wrong";
                return View("AddEmployee", emp);
            }

            userType = new UserTypeModel();
            userType.Id = 1;
            userType.Name = "Employee";
            usertypeList.Add(userType);

            userType = new UserTypeModel();
            userType.Id = 2;
            userType.Name = "Reception";
            usertypeList.Add(userType);

            ViewBag.CompanyList = new SelectList(GetCompanyList(), "Id", "Name");
            ViewBag.BranchList = new SelectList(GetBranchList(), "Id", "Name");
            ViewBag.DepartmentList = new SelectList(GetDepartmentList(), "Id", "Name");
            ViewBag.DesignationList = new SelectList(GetDesignationList(), "Id", "Name");
            ViewBag.UserTypeList = new SelectList(usertypeList, "Name", "Name");
            ViewBag.DeviceList = new SelectList(GetDeviceList(), "DeviceId", "DeviceName");

            ViewBag.Result = "Save";
            return View("AddEmployee", emp);
           // return RedirectToAction("Employee", "Master", new { userId = ViewBag.UserId, userName = ViewBag.UserName });
        }

        #endregion

        #region courier list

        public ActionResult GetCourierList()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "AdminCourierList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewData["GetCourierList"] = GetCouriers();
                return View();
            }
        }

        private static List<CourierModel> GetCouriers()
        {
            List<CourierModel> Model = new List<CourierModel>();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var data = entities.CourierTBs.ToList().OrderByDescending(d => d.Id);

                foreach (var item in data)
                {
                    CourierModel courier = new CourierModel();
                    courier.Id = item.Id;
                    courier.CourierNo = item.CourierNo;
                    courier.CourierCompany = item.CourierCompany;
                    courier.CourierPersonName = item.CourierPersonName;
                    courier.EmployeeName = item.EmployeeName;
                    courier.Time = item.Time;
                    courier.Date = Convert.ToDateTime(item.Date);
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

        #region Department Master
        [HttpPost]
        public JsonResult A_GetDepartmentList()
        {
            List<DepartmentModel> Model = new List<DepartmentModel>();
            Model = GetDepartmentList();
            return Json(Model);
        }
        [HttpGet]
        public ActionResult Department()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "DepartmentList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewData["GetDepartmentList"] = GetDepartmentList();
                return View();
            }
        }

        private static List<DepartmentModel> GetDepartmentList()
        {
            List<DepartmentModel> Model = new List<DepartmentModel>();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var emp = entities.DepartmentTBs.ToList().OrderBy(d => d.Name);

                foreach (var item in emp)
                {
                    var company = entities.CompanyTBs.Where(d => d.Id == item.CompanyId).FirstOrDefault();
                    var branch = entities.BranchTBs.Where(d => d.Id == item.BranchId).FirstOrDefault();

                    DepartmentModel employee = new DepartmentModel();
                    employee.Id = item.Id;
                    employee.Name = item.Name;
                    employee.CompanyId = company != null ? company.Id : 0;
                    employee.CompanyName = company != null ? company.Name : "";
                    employee.BranchId = branch != null ? branch.Id : 0;
                    employee.BranchName = branch != null ? branch.Name : "";
                    Model.Add(employee);
                }
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        [HttpGet]
        public ActionResult AddDepartment(int Id)
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "DepartmentList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                DepartmentModel department = new DepartmentModel();

                if (Id > 0)
                {
                    department = GetDepartmentListDetails(Id);
                }
                ViewBag.CompanyList = new SelectList(GetCompanyList(), "Id", "Name");
                ViewBag.BranchList = new SelectList(GetBranchList(), "Id", "Name");
                return View("AddDepartment", department);
            }
        }

        private static DepartmentModel GetDepartmentListDetails(int Id)
        {
            DepartmentModel Model = new DepartmentModel();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var emp = entities.DepartmentTBs.Where(d => d.Id == Id).FirstOrDefault();

                var company = entities.CompanyTBs.Where(d => d.Id == emp.CompanyId).FirstOrDefault();
                var branch = entities.BranchTBs.Where(d => d.Id == emp.BranchId).FirstOrDefault();

                Model.Id = emp.Id;
                Model.Name = emp.Name;
                Model.CompanyId = company != null ? company.Id : 0;
                Model.CompanyName = company != null ? company.Name : "";
                Model.BranchId = branch != null ? branch.Id : 0;
                Model.BranchName = branch != null ? branch.Name : "";
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        [HttpPost]
        public async Task<ActionResult> AddDepartment(DepartmentModel emp)
        {
            string userid = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            string username = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserId = userid;
            ViewBag.UserName = username;

            ViewBag.ActivePage = "DepartmentList";


            string sessionid = (System.Web.HttpContext.Current.Session["SessionId"] != null) ? System.Web.HttpContext.Current.Session["SessionId"].ToString() : String.Empty;
            if (string.IsNullOrEmpty(sessionid))
            {
                return RedirectToAction("LogOut", "Account");
            }
            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    VMSDBEntities db = new VMSDBEntities();

                    DepartmentTB tB = new DepartmentTB();

                    if (emp.Id > 0)
                    {
                        var companydt = db.DepartmentTBs.Where(d => d.Id == emp.Id).FirstOrDefault();

                        companydt.Name = emp.Name;
                        companydt.CompanyId = emp.CompanyId;
                        companydt.BranchId = emp.BranchId;
                        db.Entry(companydt).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        tB.Name = emp.Name;
                        tB.CompanyId = emp.CompanyId;
                        tB.BranchId = emp.BranchId;
                        db.DepartmentTBs.Add(tB);
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

                ViewBag.CompanyList = new SelectList(GetCompanyList(), "Id", "Name");
                ViewBag.BranchList = new SelectList(GetBranchList(), "Id", "Name");
                return View("AddDepartment", emp);
            }
            return RedirectToAction("Department", "Master", new { userId = ViewBag.UserId, userName = ViewBag.UserName });
        }
        #endregion

        #region Designation Master
        [HttpPost]
        public JsonResult A_GetDesignationList()
        {
            List<DesignationModel> Model = new List<DesignationModel>();
            Model = GetDesignationList();
            return Json(Model);
        }
        [HttpGet]
        public ActionResult Designation()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "DesignationList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewData["GetDesignationList"] = GetDesignationList();
                return View();
            }
        }

        private static List<DesignationModel> GetDesignationList()
        {
            List<DesignationModel> Model = new List<DesignationModel>();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var emp = (from d in entities.DesignationTBs
                           join c in entities.DepartmentTBs on d.DepartmentId equals c.Id
                           select new
                           {
                               d.Id,
                               d.Name,
                               departmentId = c.Id,
                               department = c.Name,
                               companyId = d.CompanyId,
                               branchId = d.BranchId,
                           }).ToList();

                foreach (var item in emp)
                {
                    var company = entities.CompanyTBs.Where(d => d.Id == item.companyId).FirstOrDefault();
                    var branch = entities.BranchTBs.Where(d => d.Id == item.branchId).FirstOrDefault();

                    DesignationModel employee = new DesignationModel();
                    employee.Id = item.Id;
                    employee.Name = item.Name;
                    employee.DepartmentId = item.departmentId;
                    employee.DepartmentName = item.department;
                    employee.CompanyId = company != null ? company.Id : 0;
                    employee.CompanyName = company != null ? company.Name : "";
                    employee.BranchId = branch != null ? branch.Id : 0;
                    employee.BranchName = branch != null ? branch.Name : "";
                    Model.Add(employee);
                }
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        [HttpGet]
        public ActionResult AddDesignation(int Id)
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "DesignationList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                DesignationModel branch = new DesignationModel();

                if (Id > 0)
                {
                    branch = GetDesignationDetails(Id);
                }

                ViewBag.CompanyList = new SelectList(GetCompanyList(), "Id", "Name");
                ViewBag.BranchList = new SelectList(GetBranchList(), "Id", "Name");
                ViewBag.DepartmentList = new SelectList(GetDepartmentList(), "Id", "Name");
                return View("AddDesignation", branch);
            }
        }

        private static DesignationModel GetDesignationDetails(int Id)
        {
            DesignationModel Model = new DesignationModel();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var emp = (from d in entities.DesignationTBs
                           join c in entities.DepartmentTBs on d.DepartmentId equals c.Id
                           where d.Id == Id
                           select new
                           {
                               d.Id,
                               d.Name,
                               departmentId = c.Id,
                               department = c.Name,
                               companyId = d.CompanyId,
                               branchId = d.BranchId,
                           }).FirstOrDefault();

                Model.Id = emp.Id;
                Model.Name = emp.Name;
                Model.DepartmentId = emp.departmentId;
                Model.DepartmentName = emp.department;

                var company = entities.CompanyTBs.Where(d => d.Id == emp.companyId).FirstOrDefault();
                var branch = entities.BranchTBs.Where(d => d.Id == emp.branchId).FirstOrDefault();

                Model.CompanyId = company != null ? company.Id : 0;
                Model.CompanyName = company != null ? company.Name : "";
                Model.BranchId = branch != null ? branch.Id : 0;
                Model.BranchName = branch != null ? branch.Name : "";
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        [HttpPost]
        public async Task<ActionResult> AddDesignation(DesignationModel emp)
        {
            string userid = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            string username = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserId = userid;
            ViewBag.UserName = username;

            ViewBag.ActivePage = "DesignationList";

            string sessionid = (System.Web.HttpContext.Current.Session["SessionId"] != null) ? System.Web.HttpContext.Current.Session["SessionId"].ToString() : String.Empty;
            if (string.IsNullOrEmpty(sessionid))
            {
                return RedirectToAction("LogOut", "Account");
            }
            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    VMSDBEntities db = new VMSDBEntities();

                    DesignationTB tB = new DesignationTB();

                    if (emp.Id > 0)
                    {
                        var companydt = db.DesignationTBs.Where(d => d.Id == emp.Id).FirstOrDefault();

                        companydt.Name = emp.Name;
                        companydt.DepartmentId = emp.DepartmentId;
                        companydt.CompanyId = emp.CompanyId;
                        companydt.BranchId = emp.BranchId;
                        db.Entry(companydt).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        tB.Name = emp.Name;
                        tB.DepartmentId = emp.DepartmentId;
                        tB.CompanyId = emp.CompanyId;
                        tB.BranchId = emp.BranchId;
                        db.DesignationTBs.Add(tB);
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

                ViewBag.CompanyList = new SelectList(GetCompanyList(), "Id", "Name");
                ViewBag.BranchList = new SelectList(GetBranchList(), "Id", "Name");
                ViewBag.DepartmentList = new SelectList(GetDepartmentList(), "Id", "Name");
                return View("AddDesignation", emp);
            }
            return RedirectToAction("Designation", "Master", new { userId = ViewBag.UserId, userName = ViewBag.UserName });
        }
        #endregion

        #region Admin export part

        [HttpPost]
        public FileResult EmpExportToExcel()
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[7] { new DataColumn("Name"),
                                            new DataColumn("Phone"),
                                            new DataColumn("Email"),
                                            new DataColumn("Company"),
                                            new DataColumn("Department"),
                                            new DataColumn("Designation"),
                                            new DataColumn("UserType"),
                                           });

            employees = GetEmployeeList();

            foreach (var item in employees)
            {
                dt.Rows.Add(item.Name, item.Phone, item.Email, item.Company, item.Department, item.Designation, item.UserType);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeList.xlsx");
                }
            }
        }

        [HttpPost]
        public ActionResult EmpExportToPdf(int? pageNumber)
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[7] { new DataColumn("Name"),
                                            new DataColumn("Phone"),
                                            new DataColumn("Email"),
                                            new DataColumn("Company"),
                                            new DataColumn("Department"),
                                            new DataColumn("Designation"),
                                            new DataColumn("UserType"),
                                           });

            employees = GetEmployeeList();

            foreach (var item in employees)
            {
                dt.Rows.Add(item.Name, item.Phone, item.Email, item.Company, item.Department, item.Designation, item.UserType);
            }

            if (dt.Rows.Count > 0)
            {
                int pdfRowIndex = 1;

                string filename = "EmployeeList-" + DateTime.Now.ToString("dd-MM-yyyy hh_mm_s_tt");
                string filepath = Server.MapPath("\\") + "" + filename + ".pdf";
                Document document = new Document(PageSize.A4, 5f, 5f, 10f, 10f);
                FileStream fs = new FileStream(filepath, FileMode.Create);
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                document.Open();

                Font font1 = FontFactory.GetFont(FontFactory.COURIER_BOLD, 10);
                Font font2 = FontFactory.GetFont(FontFactory.COURIER, 8);

                float[] columnDefinitionSize = { 1F, 1F, 1F, 1F, 1F, 1F, 1F };
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

                table.AddCell(new Phrase("Name", font1));
                table.AddCell(new Phrase("Phone", font1));
                table.AddCell(new Phrase("Email", font1));
                table.AddCell(new Phrase("Company", font1));
                table.AddCell(new Phrase("Department", font1));
                table.AddCell(new Phrase("Designation", font1));
                table.AddCell(new Phrase("UserType", font1));
                table.HeaderRows = 1;

                foreach (DataRow data in dt.Rows)
                {
                    table.AddCell(new Phrase(data["Name"].ToString(), font2));
                    table.AddCell(new Phrase(data["Phone"].ToString(), font2));
                    table.AddCell(new Phrase(data["Email"].ToString(), font2));
                    table.AddCell(new Phrase(data["Company"].ToString(), font2));
                    table.AddCell(new Phrase(data["Department"].ToString(), font2));
                    table.AddCell(new Phrase(data["Designation"].ToString(), font2));
                    table.AddCell(new Phrase(data["UserType"].ToString(), font2));

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
            return View("Employee");
        }

        #endregion

        #region import emp data

        public FileResult DownloadExcel()
        {
            string path = "/Doc/Emp.xlsx";
            return File(path, "application/vnd.ms-excel", "Employee.xlsx");
        }
        [HttpPost]
        public JsonResult A_GetEmployeeList()
        {
            List<EmployeeModel> Model = new List<EmployeeModel>();
            Model = GetEmployeeList();
            return Json(Model);
        }
        [HttpPost]
        public ActionResult Employee(SessionModel user, HttpPostedFileBase FileUpload)
        {
            string userid = user.UserId;
            string username = user.UserName;
            ViewBag.UserId = userid;
            ViewBag.UserName = username;

            ViewBag.ActivePage = "Employee";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }

            VMSDBEntities db = new VMSDBEntities();

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/Doc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
                    var ds = new DataSet();
                    adapter.Fill(ds, "ExcelTable");
                    DataTable dtable = ds.Tables["ExcelTable"];
                    string sheetName = "Sheet1";
                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<ImportEmployeeModel>(sheetName) select a;
                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(a.Name) && !string.IsNullOrEmpty(a.UserType) && !string.IsNullOrEmpty(a.Phone))
                            {
                                UserTB tB = new UserTB();
                                tB.Name = a.Name;
                                tB.EmpCode = a.EmpCode;
                                tB.FirstName = Convert.ToString(a.Name.Split(' ')[0]);
                                tB.Email = a.EmailId;
                                tB.Phone = a.Phone;
                                tB.BirthDate = a.BirthDate;
                                tB.Address = a.Address;
                                var company = db.CompanyTBs.Where(d => d.Name == a.Company).FirstOrDefault();
                                tB.CompanyId = company.Id;
                                var branch = db.BranchTBs.Where(d => d.Name == a.Branch).FirstOrDefault();
                                tB.BranchId = branch.Id;
                                var department = db.DepartmentTBs.Where(d => d.Name == a.Department).FirstOrDefault();
                                tB.DepartmentId = department.Id;
                                var designation = db.DesignationTBs.Where(d => d.Name == a.Designation).FirstOrDefault();
                                tB.DesignationId = designation.Id;
                                tB.UserName = a.UserName;
                                tB.Password = a.Password;
                                tB.UserType = a.UserType;
                                tB.CardNo = a.CardNumber;
                                db.UserTBs.Add(tB);
                                db.SaveChanges();
                            }
                            else
                            {   
                                ViewBag.Result = "name,address,phone is required";
                            }
                        }
                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {
                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {
                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                                }
                            }
                        }
                    }
                    //deleting excel file from folder
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                }
                else
                {
                    ViewBag.Result = "Only Excel file format is allowed";
                }
            }
            else
            {
                ViewBag.Result = "Failed";
            }

            ViewBag.Result = "Successfully Imported";

            ViewData["GetEmployeeList"] = GetEmployeeList();
            return View("Employee");
        }
        #endregion

        #region Delete Employee

        [HttpGet]
        public ActionResult DeleteEmployeeFromDevice()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "DeviceEmployee";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewData["GetDeviceList"] = GetDevices();
                ViewData["GetEmployeeList"] = GetEmployeeList();

                SessionModel emp = new SessionModel();
                emp.UserId = userId;
                emp.UserName = userName;

                ViewBag.CompanyList = new SelectList(GetCompanyList(), "Id", "Name");
                ViewBag.DepartmentList = new SelectList(GetDepartmentList(), "Id", "Name");

                return View("DeleteEmployeeFromDevice", emp);
            }
        }

        private static List<DeviceModel> GetDevices()
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

        [HttpPost]
        public JsonResult DeleteEmployee(string dev,string Emp)
        {
            VMSDBEntities db = new VMSDBEntities();
            bool res = true;

            string[] devices = dev.Split(',');

            string[] empdt = Emp.Split(',');
            string strEmployrrList = string.Empty;
            List<string> listEmployeeNo = new List<string>();
            foreach (var item in empdt)
            {
                if(item != "")
                {
                    listEmployeeNo.Add(item.ToString());
                }
            }

            for (int i = 0; i < listEmployeeNo.Count; i++)
            {
                strEmployrrList += "{\"employeeNo\":\"" + listEmployeeNo[i] + "\"}";
                if (1 != listEmployeeNo.Count - i)
                {
                    strEmployrrList += ",";
                }
            }

            foreach (string d in devices)
            {
                if(d != "")
                {
                    var dv = db.DevicesTBs.Where(x => x.DeviceAccountId == d).FirstOrDefault();

                    string url = "http://" + dv.DeviceIPAddress + ":" + dv.Port + "/ISAPI/ContentMgmt/DeviceMgmt/deviceList?format=json";

                    string req = "{\"SearchDescription\" : {\"position\":0,\"maxResult\":5}}";

                    //req = string.Empty;
                    string reps = string.Empty;
                    string strMatchNum = string.Empty;
                    clienthttp clnt = new clienthttp();
                    int iet = clnt.HttpRequest(dv.UserName, dv.Password, url, "POST", req, ref reps);
                    if (iet == (int)HttpStatus.Http200)
                    {
                        DeviceSearchRoot dr = JsonConvert.DeserializeObject<DeviceSearchRoot>(reps);
                        strMatchNum = Convert.ToString(dr.SearchResult.numOfMatches);

                        if ("0" != strMatchNum)
                        {
                            foreach (var devv in dr.SearchResult.MatchList)
                            {
                                if (devv.Device.EhomeParams.EhomeID == dv.DeviceAccountId)
                                {
                                    var devicedt = devv.Device;

                                    ApiMonitorTB at = new ApiMonitorTB();
                                    at.Command = "Check Device Exist";
                                    at.Page = "Delete Employee";
                                    at.time = DateTime.Now;
                                    at.DeviceSRNO = dv.DeviceSerialNo;
                                    at.DeviceName = dv.DeviceName;
                                    at.EmpCode = "";
                                    at.EmpName = "";
                                    at.Status = "Success";
                                    db.ApiMonitorTBs.Add(at);
                                    db.SaveChanges();

                                    string strReq = "{\"UserInfoDetail\": {\"mode\": \"byEmployeeNo\",\"EmployeeNoList\": [" + strEmployrrList + "]}}";
                                    string strUrl = "http://" + dv.DeviceIPAddress + ":" + dv.Port + "/ISAPI/AccessControl/UserInfoDetail/Delete?format=json&devIndex=" + devicedt.devIndex;
                                    string strRsp = string.Empty;
                                    clnt = new clienthttp();
                                    int iRet = clnt.HttpRequest(dv.UserName, dv.Password, strUrl, "PUT", strReq, ref strRsp);
                                    if (iRet == (int)HttpStatus.Http200)
                                    {
                                        res = true;

                                        for (int i = 0; i < listEmployeeNo.Count; i++)
                                        {
                                            at = new ApiMonitorTB();
                                            at.Command = "Delete Employee";
                                            at.Page = "Delete Employee";
                                            at.time = DateTime.Now;
                                            at.DeviceSRNO = dv.DeviceSerialNo;
                                            at.DeviceName = dv.DeviceName;
                                            at.EmpCode = listEmployeeNo[i];
                                            var emp = db.UserTBs.Where(c => c.EmpCode == at.EmpCode).FirstOrDefault();
                                            at.EmpName = emp.Name;
                                            at.Status = "Success";
                                            db.ApiMonitorTBs.Add(at);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }                
            }


            return Json(res);
        }

        public ActionResult GetDeleteEmpListSearch(int CompanyId,int DeptId)
        {
            List<EmployeeModel> emp = GetEmployeeList();

            if (CompanyId > 0)
            {
                emp = emp.Where(d => d.CompanyId == CompanyId).ToList();
            }

            if (DeptId > 0)
            {
                emp = emp.Where(d => d.DepartmentId == DeptId).ToList();
            }

            return Json(emp, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region upload employee

        [HttpGet]
        public ActionResult UploadEmployeeToDevice()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "UploadEmployee";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewData["GetDeviceList"] = GetDevices();
                ViewData["GetEmployeeList"] = GetEmployeeList();

                SessionModel emp = new SessionModel();
                emp.UserId = userId;
                emp.UserName = userName;

                ViewBag.CompanyList = new SelectList(GetCompanyList(), "Id", "Name");
                ViewBag.DepartmentList = new SelectList(GetDepartmentList(), "Id", "Name");

                return View("UploadEmployeeToDevice", emp);
            }
        }

        [HttpPost]
        public JsonResult UploadEmployee(string dev, string Emp)
        {
            VMSDBEntities db = new VMSDBEntities();
            bool res = true;

            string[] devices = dev.Split(',');

            string[] empdt = Emp.Split(',');
            string strEmployrrList = string.Empty;
            List<string> listEmployeeNo = new List<string>();
            foreach (var item in empdt)
            {
                if (item != "")
                {
                    listEmployeeNo.Add(item.ToString());
                }
            }

            //for (int i = 0; i < listEmployeeNo.Count; i++)
            //{
            //    strEmployrrList += "{\"employeeNo\":\"" + listEmployeeNo[i] + "\"}";
            //    if (1 != listEmployeeNo.Count - i)
            //    {
            //        strEmployrrList += ",";
            //    }
            //}

            foreach (string d in devices)
            {
                if (d != "")
                {
                    var dv = db.DevicesTBs.Where(x => x.DeviceAccountId == d).FirstOrDefault();

                    string url = "http://" + dv.DeviceIPAddress + ":" + dv.Port + "/ISAPI/ContentMgmt/DeviceMgmt/deviceList?format=json";

                    string req = "{\"SearchDescription\" : {\"position\":0,\"maxResult\":5}}";

                    //req = string.Empty;
                    string reps = string.Empty;
                    string strMatchNum = string.Empty;
                    clienthttp clnt = new clienthttp();
                    int iet = clnt.HttpRequest(dv.UserName, dv.Password, url, "POST", req, ref reps);
                    if (iet == (int)HttpStatus.Http200)
                    {
                        DeviceSearchRoot dr = JsonConvert.DeserializeObject<DeviceSearchRoot>(reps);
                        strMatchNum = Convert.ToString(dr.SearchResult.numOfMatches);

                        if ("0" != strMatchNum)
                        {
                            foreach (var devv in dr.SearchResult.MatchList)
                            {
                                if (devv.Device.EhomeParams.EhomeID == dv.DeviceAccountId)
                                {
                                    var devicedt = devv.Device;

                                    ApiMonitorTB at = new ApiMonitorTB();
                                    at.Command = "Check Device Exist";
                                    at.Page = "Upload Employee";
                                    at.time = DateTime.Now;
                                    at.DeviceSRNO = dv.DeviceSerialNo;
                                    at.DeviceName = dv.DeviceName;
                                    at.EmpCode = "";
                                    at.EmpName = "";
                                    at.Status = "Success";
                                    db.ApiMonitorTBs.Add(at);
                                    db.SaveChanges();

                                    for (int i = 0; i < listEmployeeNo.Count; i++)
                                    {  
                                        strEmployrrList += ",";

                                        string filePath = System.IO.Path.Combine(Server.MapPath("/Uploads/"), "M+ANIL_1002788.jpg");
                                        string strEmployeeID = listEmployeeNo[i];
                                        string strReq = "{ \"FaceInfo\": {\"employeeNo\": \"" + strEmployeeID + "\",\"faceLibType\": \"blackFD\" }}";
                                        string strUrl = "http://" + dv.DeviceIPAddress + ":" + dv.Port + "/ISAPI/Intelligent/FDLib/FaceDataRecord?format=json&devIndex=" + devicedt.devIndex;
                                        string strRsp = string.Empty;

                                        string fileKeyName = "FaceImage";
                                        NameValueCollection stringDict = new NameValueCollection();
                                        stringDict.Add("FaceDataRecord", strReq);

                                        clnt = new clienthttp();
                                        int iRet = clnt.HttpPostData(dv.UserName, dv.Password, strUrl, fileKeyName, filePath, stringDict, ref strRsp);
                                        if (iRet != (int)HttpStatus.Http200)
                                        {
                                            at = new ApiMonitorTB();
                                            at.Command = "Face Upload";
                                            at.Page = "Upload Employee";
                                            at.time = DateTime.Now;
                                            at.DeviceSRNO = dv.DeviceSerialNo;
                                            at.DeviceName = dv.DeviceName;
                                            at.EmpCode = strEmployeeID;
                                            var emp = db.UserTBs.Where(c => c.EmpCode == at.EmpCode).FirstOrDefault();
                                            at.EmpName = emp.Name;
                                            at.Status = "Success";
                                            db.ApiMonitorTBs.Add(at);
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            at = new ApiMonitorTB();
                                            at.Command = "Face Upload";
                                            at.Page = "Upload Employee";
                                            at.time = DateTime.Now;
                                            at.DeviceSRNO = dv.DeviceSerialNo;
                                            at.DeviceName = dv.DeviceName;
                                            at.EmpCode = strEmployeeID;
                                            var emp = db.UserTBs.Where(c => c.EmpCode == at.EmpCode).FirstOrDefault();
                                            at.EmpName = emp.Name;
                                            at.Status = "Failed";
                                            db.ApiMonitorTBs.Add(at);
                                            db.SaveChanges();
                                        }
                                    }                                    

                                    //string strReq = "{\"UserInfoDetail\": {\"mode\": \"byEmployeeNo\",\"EmployeeNoList\": [" + strEmployrrList + "]}}";
                                    //string strUrl = "http://" + dv.DeviceIPAddress + ":" + dv.Port + "/ISAPI/AccessControl/UserInfoDetail/Delete?format=json&devIndex=" + devicedt.devIndex;
                                    //string strRsp = string.Empty;
                                    //clnt = new clienthttp();
                                    //int iRet = clnt.HttpRequest(dv.UserName, dv.Password, strUrl, "PUT", strReq, ref strRsp);
                                    //if (iRet == (int)HttpStatus.Http200)
                                    //{
                                    //    res = true;
                                    //}
                                }
                            }
                        }
                    }
                }
            }


            return Json(res);
        }

        #endregion

    }
}