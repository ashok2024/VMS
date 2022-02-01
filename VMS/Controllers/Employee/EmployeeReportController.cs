using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VMS.Controllers.Account;
using VMS.Middleware;
using VMS.Models.Visitor;

namespace VMS.Controllers.Employee
{
    public class EmployeeReportController : BaseController
    {
        List<VisitorEntryModel> visitorEntries;
        List<VisitorEntryModel> RejectedvisitorEntries;
        public EmployeeReportController()
        {
            visitorEntries = new List<VisitorEntryModel>();
            RejectedvisitorEntries = new List<VisitorEntryModel>();
        }

        // GET: EmployeeReport
        public ActionResult Index()
        {
            return View();
        }

        #region Approved visitor report

        public ActionResult GetEmployeeVisitedVisitorList()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "EmployeeVisitedVisitorList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                int Id = Convert.ToInt32(userId);
                ViewData["GetVisitorList"] = visitorEntries = GetVisitedVisitors(Id);
                return View();
            }
        }

        private static List<VisitorEntryModel> GetVisitedVisitors(int Id)
        {
            List<VisitorEntryModel> Model = new List<VisitorEntryModel>();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var visitors = entities.VisitorEntryTBs.Where(d => d.EmployeeId == Id).ToList();

                foreach (var item in visitors)
                {
                    var status = entities.VisitorStatusTBs.Where(d => d.VisitId == item.Id).FirstOrDefault();

                    if (status != null)
                    {
                        if (status.Status == "Approve")
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
                }
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        public ActionResult GetVisitorListSearch(VisitorSearchModel visitor)
        {
            visitorEntries = GetVisitedVisitors(visitor.UserId);

            if (!string.IsNullOrWhiteSpace(visitor.Contact))
            {
                visitorEntries = visitorEntries.Where(d => d.Contact.Contains(visitor.Contact)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(visitor.Company))
            {
                visitorEntries = visitorEntries.Where(d => d.Company.Contains(visitor.Company)).ToList();
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

        #endregion

        #region rejected visitor report

        public ActionResult GetEmployeeRejectedVisitorList()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "EmployeeRejectedVisitorList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewData["GetVisitorList"] = RejectedvisitorEntries = GetRejectedVisitors(Convert.ToInt32(userId));
                return View();
            }
        }

        private static List<VisitorEntryModel> GetRejectedVisitors(int Id)
        {
            List<VisitorEntryModel> Model = new List<VisitorEntryModel>();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var visitors = entities.VisitorEntryTBs.Where(d => d.EmployeeId == Id).ToList();

                foreach (var item in visitors)
                {
                    var status = entities.VisitorStatusTBs.Where(d => d.VisitId == item.Id).FirstOrDefault();

                    if (status != null)
                    {
                        if (status.Status == "Reject")
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
                }
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        public ActionResult GetRejectVisitorListSearch(VisitorSearchModel visitor)
        {
            RejectedvisitorEntries = GetRejectedVisitors(visitor.UserId);

            if (!string.IsNullOrWhiteSpace(visitor.Contact))
            {
                RejectedvisitorEntries = RejectedvisitorEntries.Where(d => d.Contact.Contains(visitor.Contact)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(visitor.Company))
            {
                RejectedvisitorEntries = RejectedvisitorEntries.Where(d => d.Company.Contains(visitor.Company)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(visitor.Department))
            {
                RejectedvisitorEntries = RejectedvisitorEntries.Where(d => d.EmployeeDepartment.Contains(visitor.Department)).ToList();
            }

            if (!string.IsNullOrEmpty(visitor.FromDate) && string.IsNullOrEmpty(visitor.ToDate))
            {
                RejectedvisitorEntries = RejectedvisitorEntries.Where(d => d.VisitDateFrom <= Convert.ToDateTime(visitor.FromDate)).ToList();
            }

            if (!string.IsNullOrEmpty(visitor.FromDate) && !string.IsNullOrEmpty(visitor.ToDate))
            {
                RejectedvisitorEntries = RejectedvisitorEntries.Where(d => d.VisitDateFrom >= Convert.ToDateTime(visitor.FromDate) && d.VisitDateTo <= Convert.ToDateTime(visitor.ToDate)).ToList();
            }

            if (!string.IsNullOrEmpty(visitor.Time))
            {
                RejectedvisitorEntries = RejectedvisitorEntries.Where(d => Convert.ToDateTime(d.InTime) >= Convert.ToDateTime(visitor.Time) && Convert.ToDateTime(d.OutTime) <= Convert.ToDateTime(visitor.Time)).ToList();
            }

            return Json(RejectedvisitorEntries, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpPost]
        public ActionResult GetUpcomingVisitorListEmp()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "VisitedVisitorList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewData["GetUpcomingVisitorListE"] = visitorEntries = GetUpcomingVisitors();
                return View();
            }
        }

        private static List<VisitorEntryModel> GetUpcomingVisitors(string userID = "")
        {
            List<VisitorEntryModel> Model = new List<VisitorEntryModel>();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                int intEmployeeID = 0;
                if (!string.IsNullOrEmpty(userID))
                {
                    intEmployeeID = Convert.ToInt32(userID);
                }
                var visitors = entities.VisitorEntryTBs.Where(d => d.FromDate > DateTime.Now
                                                            && (!string.IsNullOrEmpty(userID) ? d.EmployeeId == intEmployeeID : true)
                                                            ).ToList().OrderByDescending(d => d.Id);

                foreach (var item in visitors)
                {
                    //var status = entities.VisitorStatusTBs.Where(d => d.VisitId == item.Id).FirstOrDefault();

                    //if (status != null)
                    //{
                    //if (status.Status == "Approve")
                    //{
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
                    //}
                    //}
                }
            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }
    }
}