using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VMS.Controllers.Account;
using VMS.Middleware;
using VMS.Models.Employee;
using VMS.Models.Visitor;

namespace VMS.Controllers.Employee
{
    public class EmployeeController : BaseController
    {
        VMSDBEntities db;
        List<VisitorEntryModel> visitorEntries;
        List<VisitorEntryModel> scheduleData;
        public EmployeeController()
        {
            db = new VMSDBEntities();
            visitorEntries = new List<VisitorEntryModel>();
            scheduleData = new List<VisitorEntryModel>();
        }

        // GET: Employee
        public ActionResult EmployeeDashboard()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "EmployeeDashboard";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                EmployeeDashboardModel dashboardModel = new EmployeeDashboardModel();
                dashboardModel = GetEmployeeDashboardDetails(Convert.ToInt32(userId));
                return View("EmployeeDashboard", dashboardModel);
            }
        }

        // GET: VisitorList
        public ActionResult GetEmployeeVisitorList()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "EmployeeVisitorList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                int UserId = Convert.ToInt32(userId);
                ViewData["GetVisitorList"] = GetVisitors(UserId);
                return View();
            }
        }

        public ActionResult VisitorStatus(string VisitorId)
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "EmployeeVisitorList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewBag.IdProofList = new SelectList(GetIdProofType(), "Name", "Name");
                ViewBag.MaterialList = new SelectList(GetMaterialList(), "Name", "Name");
                ViewBag.PriorityList = new SelectList(GetPrioritylList(), "Name", "Name");

                VisitorEntryModel visitor = new VisitorEntryModel();
                visitor = GetVisitorDetails(VisitorId);

                return View("VisitorStatus", visitor);
            }
        }

        [HttpPost]
        public JsonResult SaveVisitStatus(ApproveRejectModel data)
        {
            bool res = false;

            try
            {
                VisitorStatusTB tB = new VisitorStatusTB();
                
                tB.VisitId = data.VisitId;
                tB.UserId = data.UserId;
                tB.Status = data.Status;                
                db.VisitorStatusTBs.Add(tB);
                db.SaveChanges();

                res = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return Json(res);
        }


        [HttpPost]
        public async Task<ActionResult> SaveStatus(VisitorEntryModel visitor, string save, string cancel)
        {
            string userid = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            string username = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserId = userid;
            ViewBag.UserName = username;

            ViewBag.ActivePage = "EmployeeVisitorList";
           
            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }

            if (!string.IsNullOrEmpty(save))
            {
                visitor.Status = save;
            }
            if (!string.IsNullOrEmpty(cancel))
            {
                visitor.Status = cancel;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    VisitorStatusTB tB = new VisitorStatusTB();

                    if (tB.Id > 0)
                    {
                    }
                    else
                    {
                        // tB.Id = supplier.Id;
                        tB.VisitId = visitor.Id;
                        tB.UserId = visitor.EmployeeId;
                        tB.Status = visitor.Status;
                        tB.Remark = visitor.StatusRemark;
                        db.VisitorStatusTBs.Add(tB);
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

                List<StatusModel> statusList = new List<StatusModel>();
                StatusModel status = new StatusModel();
                status.Id = 1;
                status.Name = "Approve";
                statusList.Add(status);

                status = new StatusModel();
                status.Id = 2;
                status.Name = "Reject";
                statusList.Add(status);

                ViewBag.StatusList = new SelectList(statusList, "Name", "Name");

                return View("VisitorStatus", visitor);
            }
            return RedirectToAction("GetEmployeeVisitorList", "Employee", new { userId = ViewBag.UserId, userName = ViewBag.UserName });
        }

        private static List<VisitorEntryModel> GetVisitors(int UserId)
        {
            List<VisitorEntryModel> Model = new List<VisitorEntryModel>();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var visitors = entities.VisitorEntryTBs.Where(d => d.EmployeeId == UserId).ToList().OrderByDescending(d => d.Id);

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
                    visitor.InTime = item.InTime;
                    visitor.OutTime = item.OutTime;
                    visitor.VisitDateFrom = Convert.ToDateTime(item.FromDate);
                    visitor.VisitDateTo = Convert.ToDateTime(item.ToDate);
                    visitor.Purpose = item.Purpose;
                    visitor.Priority = item.Priority;

                    var status = entities.VisitorStatusTBs.Where(x => x.VisitId == item.Id).FirstOrDefault();
                    if (status == null)
                    {
                        visitor.Status = null;
                    }
                    else
                    {
                        visitor.Status = status.Status;
                    }

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

        private static VisitorEntryModel GetVisitorDetails(string VisitorId)
        {
            VisitorEntryModel Model = new VisitorEntryModel();

            VMSDBEntities entities = new VMSDBEntities();
            string baseadder = ConfigurationManager.AppSettings["WebUIUrl"];
            try
            {
                var visitor = entities.VisitorEntryTBs.Where(d => d.VisitorId == VisitorId).FirstOrDefault();

                Model.Id = visitor.Id;
                Model.VisitorId = visitor.VisitorId;
                Model.Name = visitor.Name;
                Model.Company = visitor.Company;
                Model.Contact = visitor.Contact;
                Model.Address = visitor.Address;
                Model.EmployeeId = Convert.ToInt32(visitor.EmployeeId);
                Model.InTime = visitor.InTime;
                Model.OutTime = visitor.OutTime;
                Model.VisitDateFrom = visitor.FromDate;
                Model.VisitDateTo = visitor.ToDate;                
                Model.EmailId = visitor.EmailId; 
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
                Model.PhotoPath = baseadder + visitor.Photo;
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        private static EmployeeDashboardModel GetEmployeeDashboardDetails(int UserId)
        {
            EmployeeDashboardModel Model = new EmployeeDashboardModel();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var visitors = entities.VisitorEntryTBs.Where(d => d.EmployeeId == UserId).ToList();
                Model.TotalVisitorCount = visitors.Count();

                var Aprovedvisitors = entities.VisitorStatusTBs.Where(d => d.UserId == UserId && d.Status == "Approve").ToList();
                Model.TotalVisitedCount = Aprovedvisitors.Count();

                var Rejectedvisitors = entities.VisitorStatusTBs.Where(d => d.UserId == UserId && d.Status == "Reject").ToList();
                Model.TotalRejectedVisitorCount = Rejectedvisitors.Count();

                var upcomevisitors = visitors.Where(d => d.FromDate > DateTime.Now).ToList();
                Model.TotalupcomingVisitorCount = upcomevisitors.Count();

                var userdata = entities.UserTBs.Where(d => d.UserId == UserId).FirstOrDefault();

                var deliveries = entities.CourierTBs.Where(d => d.EmployeeName == userdata.Name).ToList();
                Model.TotalDeliveries = deliveries.Count();

                Model.TotalDefaultVisitorCount = 0;

            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        #region for schedule meeting with visitor

        public ActionResult ScheduledVisitorList()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "ScheduledVisitorList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                int UserId = Convert.ToInt32(userId);
                ViewData["GetScheduledVisitorList"] = GetEmployeeScheduledVisit(UserId);
                return View();
            }
        }

        private static List<VisitorEntryModel> GetEmployeeScheduledVisit(int UserId)
        {
            List<VisitorEntryModel> Model = new List<VisitorEntryModel>();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var scheduleList = entities.EmployeeScheduledVisitTBs.Where(d => d.EmployeeId == UserId).ToList();

                foreach (var item in scheduleList)
                {
                    var visitors = entities.VisitorEntryTBs.Where(d => d.Id == item.VisitId).FirstOrDefault();

                    VisitorEntryModel visitor = new VisitorEntryModel();
                    visitor.Id = item.Id;
                    visitor.VisitorId = visitors.VisitorId;
                    visitor.Name = visitors.Name;
                    visitor.Contact = visitors.Contact;
                    visitor.Company = visitors.Company;
                    visitor.InTime = visitors.InTime;
                    visitor.OutTime = visitors.OutTime;
                    visitor.VisitDateFrom = Convert.ToDateTime(visitors.FromDate);
                    visitor.VisitDateTo = Convert.ToDateTime(visitors.ToDate);
                    visitor.Purpose = visitors.Purpose;
                    visitor.Priority = visitors.Priority;
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

        public ActionResult ScheduleVisit(int Id)
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "ScheduledVisitorList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                VisitorEntryModel visitor = new VisitorEntryModel();
                if (Id > 0)
                {
                    visitor = GetScheduledVisitSDetails(Id);
                }
                ViewBag.PriorityList = new SelectList(GetPrioritylList(), "Name", "Name");
                return View("ScheduleVisit",visitor);
            }
        }

        private static VisitorEntryModel GetScheduledVisitSDetails(int Id)
        {
            VisitorEntryModel Model = new VisitorEntryModel();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var schedule = entities.EmployeeScheduledVisitTBs.Where(d => d.Id == Id).FirstOrDefault();

                var visitors = entities.VisitorEntryTBs.Where(d => d.Id == schedule.VisitId).FirstOrDefault();

                Model.Id = schedule.Id;
                Model.VisitorId = visitors.VisitorId;
                Model.Name = visitors.Name;
                Model.Contact = visitors.Contact;
                Model.Company = visitors.Company;
                Model.InTime = visitors.InTime;
                Model.OutTime = visitors.OutTime;
                Model.VisitDateFrom = Convert.ToDateTime(visitors.FromDate);
                Model.VisitDateTo = Convert.ToDateTime(visitors.ToDate);
                Model.Purpose = visitors.Purpose;
                Model.Priority = visitors.Priority;
                
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        [HttpPost]
        public async Task<ActionResult> ScheduleVisit(VisitorEntryModel visitor)
        {
            string userid = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            string username = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserId = userid;
            ViewBag.UserName = username;

            ViewBag.ActivePage = "ScheduledVisitorList";
           
            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }

            //if (supplier.Id == 0)
            //{
            //    ModelState.Remove("Id");
            //}

            ModelState.Remove("InTime");
            ModelState.Remove("OutTime");
            ModelState.Remove("IdProof");
            ModelState.Remove("IdProofNo");
            ModelState.Remove("CardNo");

            if (ModelState.IsValid)
            {
                try
                {
                    if(visitor.Id > 0)
                    {
                        var check = db.EmployeeScheduledVisitTBs.Where(d => d.Id == visitor.Id).FirstOrDefault();

                        var checkvisit = db.VisitorEntryTBs.Where(d => d.Id == check.VisitId).FirstOrDefault();

                        checkvisit.Name = visitor.Name;
                        checkvisit.Company = visitor.Company;
                        checkvisit.Address = visitor.Address;
                        checkvisit.InTime = visitor.InTime;
                        checkvisit.OutTime = visitor.OutTime;
                        checkvisit.FromDate = visitor.VisitDateFrom;
                        checkvisit.ToDate = visitor.VisitDateTo;
                        checkvisit.EmailId = visitor.EmailId;
                        checkvisit.Contact = visitor.Contact;
                        checkvisit.Purpose = visitor.Purpose;
                        checkvisit.Remark = visitor.Remark;
                        checkvisit.Priority = visitor.Priority;
                        checkvisit.UserId = Convert.ToInt32(userid);
                        db.Entry(checkvisit).State = EntityState.Modified;

                        check.EmployeeId = checkvisit.EmployeeId;
                        check.VisitId = checkvisit.Id;
                        db.Entry(check).State = EntityState.Modified;

                        db.SaveChanges();
                    }
                    else
                    {
                        VisitorTB dt = new VisitorTB();

                        var visitordata = db.VisitorTBs.Where(d => d.Contact == visitor.Contact).FirstOrDefault();

                        if (visitordata != null)
                        {
                            // tB.Id = supplier.Id;
                            VisitorEntryTB tB = new VisitorEntryTB();
                            tB.EmployeeId = Convert.ToInt32(userid);
                            tB.VisitorId = visitordata.VisitorId;
                            tB.Name = visitor.Name;
                            tB.Company = visitor.Company;
                            tB.Address = visitor.Address;
                            tB.InTime = visitor.InTime;
                            tB.FromDate = visitor.VisitDateFrom;
                            tB.EmailId = visitor.EmailId;
                            tB.Contact = visitor.Contact;
                            tB.Purpose = visitor.Purpose;
                            tB.Remark = visitor.Remark;
                            tB.Priority = visitor.Priority;
                            tB.UserId = Convert.ToInt32(userid);
                            db.VisitorEntryTBs.Add(tB);
                            db.SaveChanges();

                            EmployeeScheduledVisitTB employee = new EmployeeScheduledVisitTB();
                            employee.EmployeeId = tB.EmployeeId;
                            employee.VisitId = tB.Id;
                            db.EmployeeScheduledVisitTBs.Add(employee);
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

                            // tB.Id = supplier.Id;
                            VisitorEntryTB tB = new VisitorEntryTB();
                            tB.EmployeeId = Convert.ToInt32(userid);
                            tB.VisitorId = dt.VisitorId;
                            tB.Name = visitor.Name;
                            tB.Company = visitor.Company;
                            tB.Address = visitor.Address;
                            tB.InTime = visitor.InTime;
                            tB.FromDate = visitor.VisitDateFrom;
                            tB.EmailId = visitor.EmailId;
                            tB.Contact = visitor.Contact;
                            tB.Purpose = visitor.Purpose;
                            tB.Remark = visitor.Remark;
                            tB.Priority = visitor.Priority;
                            tB.UserId = Convert.ToInt32(userid);
                            db.VisitorEntryTBs.Add(tB);
                            db.SaveChanges();

                            EmployeeScheduledVisitTB employee = new EmployeeScheduledVisitTB();
                            employee.EmployeeId = tB.EmployeeId;
                            employee.VisitId = tB.Id;
                            db.EmployeeScheduledVisitTBs.Add(employee);
                            db.SaveChanges();
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

                ViewBag.PriorityList = new SelectList(GetPrioritylList(), "Name", "Name");
                return View("ScheduleVisit", visitor);
            }
            //return RedirectToAction("GetSupplier", new { userId = userid, userName = username });
            return RedirectToAction("ScheduledVisitorList", "Employee", new { userId = ViewBag.UserId, userName = ViewBag.UserName });
        }

        #endregion

        public ActionResult GetVisitorListSearch(VisitorSearchModel visitor)
        {
            visitorEntries = GetVisitors(visitor.UserId);

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

        public ActionResult GetScheduleListSearch(VisitorSearchModel visitor)
        {
            scheduleData = GetEmployeeScheduledVisit(visitor.UserId);

            if (!string.IsNullOrWhiteSpace(visitor.Contact))
            {
                scheduleData = scheduleData.Where(d => d.Contact.Contains(visitor.Contact)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(visitor.Company))
            {
                scheduleData = scheduleData.Where(d => d.Company.Contains(visitor.Company)).ToList();
            }           

            if (!string.IsNullOrEmpty(visitor.FromDate) && string.IsNullOrEmpty(visitor.ToDate))
            {
                scheduleData = scheduleData.Where(d => d.VisitDateFrom <= Convert.ToDateTime(visitor.FromDate)).ToList();
            }

            if (!string.IsNullOrEmpty(visitor.FromDate) && !string.IsNullOrEmpty(visitor.ToDate))
            {
                scheduleData = scheduleData.Where(d => d.VisitDateFrom >= Convert.ToDateTime(visitor.FromDate) && d.VisitDateTo <= Convert.ToDateTime(visitor.ToDate)).ToList();
            }

            if (!string.IsNullOrEmpty(visitor.Time))
            {
                scheduleData = visitorEntries.Where(d => Convert.ToDateTime(d.InTime) >= Convert.ToDateTime(visitor.Time) && Convert.ToDateTime(d.OutTime) <= Convert.ToDateTime(visitor.Time)).ToList();
            }

            return Json(scheduleData, JsonRequestBehavior.AllowGet);
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
    }
}