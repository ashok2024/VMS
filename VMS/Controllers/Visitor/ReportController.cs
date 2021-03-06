using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VMS.Controllers.Account;
using VMS.Middleware;
using VMS.Models.Visitor;

namespace VMS.Controllers.Visitor
{
    public class ReportController : BaseController
    {
        List<VisitorEntryModel> visitorEntries;
        List<VisitorEntryModel> RejectedvisitorEntries;
        public ReportController()
        {
            visitorEntries = new List<VisitorEntryModel>();
            RejectedvisitorEntries = new List<VisitorEntryModel>();
        }

        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        // GET: VisitorList
        #region Approved visitor report

        [HttpPost]
        public JsonResult A_GetAprovedVisitorList(string userID = "")
        {            
            List<VisitorEntryModel> Model = new List<VisitorEntryModel>();
            Model = GetVisitedVisitors(userID);
            return Json(Model);
        }

        public ActionResult GetVisitedVisitorList()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;
            ViewData["userID"] = Convert.ToInt32(userId);

            ViewBag.ActivePage = "VisitedVisitorList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewData["GetVisitorList"] = visitorEntries = GetVisitedVisitors();
                return View();
            }
        }

        private static List<VisitorEntryModel> GetVisitedVisitors(string userID = "")
        {
            List<VisitorEntryModel> Model = new List<VisitorEntryModel>();

            VMSDBEntities entities = new VMSDBEntities();
            int intEmployeeID = 0;
            if (!string.IsNullOrEmpty(userID))
            {
                intEmployeeID = Convert.ToInt32(userID);
            }
            try
            {
                var visitors = (from d in entities.VisitorEntryTBs
                               join c in entities.VisitorStatusTBs on d.Id equals c.VisitId
                               where c.Status == "Approve" && (!string.IsNullOrEmpty(userID) ? d.EmployeeId == intEmployeeID : true)
                               select d).ToList();
                //var visitors = entities.VisitorEntryTBs.ToList().OrderByDescending(d => d.Id);

                foreach (var item in visitors)
                {
                    //var status = entities.VisitorStatusTBs.Where(d => d.VisitId == item.Id).FirstOrDefault();

                    //if (status != null)
                    //{
                    //    if (status.Status == "Approve")
                    //    {
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
                    //    }
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

        public ActionResult GetUpcomingVisitorList()
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
                ViewData["GetUpcomingVisitorList"] = visitorEntries = GetUpcomingVisitors();
                return View();
            }
        }
        [HttpPost]

        public ActionResult GetUpcomingVisitorListE()
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
        [HttpPost]
        public JsonResult A_GetUpcomingVisitorList(string userID = "")
        {
            List<VisitorEntryModel> Model = new List<VisitorEntryModel>();
            Model = GetUpcomingVisitors(userID);
            return Json(Model);
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

        [HttpPost]
        public JsonResult FilterGetAprovedVisitorList(string contactname, string company, string dept, string fromdate, string todate, string inTime, string userID = "")
        {
            List<VisitorEntryModel> Model = new List<VisitorEntryModel>();

            VMSDBEntities entities = new VMSDBEntities();
            List<VisitorEntryTB> visitors = new List<VisitorEntryTB>();
            try
            {
                DateTime fDate = new DateTime();
                DateTime tDate = new DateTime();
                if (!string.IsNullOrEmpty(fromdate))
                {
                    string strDate = fromdate;/* fromdate.Split('/')[1] + "/" + fromdate.Split('/')[0] + "/" + fromdate.Split('/')[2];*/
                    fDate = Convert.ToDateTime(strDate);
                }

                if (!string.IsNullOrEmpty(todate))
                {
                    string strDate = todate; /* todate.Split('/')[1] + "/" + todate.Split('/')[0] + "/" + todate.Split('/')[2];*/
                    tDate = Convert.ToDateTime(todate);
                }
                int intEmployeeID = 0;
                if (!string.IsNullOrEmpty(userID))
                {
                    intEmployeeID = Convert.ToInt32(userID);
                }

                var objData = (from d in entities.VisitorEntryTBs
                               join c in entities.VisitorStatusTBs on d.Id equals c.VisitId
                               where (c.Status == "Approve"
                                             && (!string.IsNullOrEmpty(userID) ? d.EmployeeId == intEmployeeID : true)
                                             && (!string.IsNullOrEmpty(company) ? d.Company.ToLower() == company.ToLower() : true)
                                             && (!string.IsNullOrEmpty(contactname) ? d.Name.ToLower() == contactname.ToLower() : true)
                                             && (!string.IsNullOrEmpty(dept) ? d.Name.ToLower() == dept.ToLower() : true)
                                             && (!string.IsNullOrEmpty(fromdate) ? d.FromDate >= fDate : true)
                                             && (!string.IsNullOrEmpty(todate) ? d.ToDate <= tDate : true)
                                             && (!string.IsNullOrEmpty(inTime) ? d.InTime == inTime : true))
                               select d).ToList();
                //visitors = (List<VisitorEntryTB>)entities.VisitorEntryTBs.ToList().OrderByDescending(d => d.Id);

                foreach (var item in objData)
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

            }
            return Json(Model);
        }

        
        [HttpPost]
        public JsonResult FilterGetUpcomingVisitorList(string contactname, string company, string dept, string fromdate, string todate, string inTime, string userID = "")
        {
            List<VisitorEntryModel> Model = new List<VisitorEntryModel>();

            VMSDBEntities entities = new VMSDBEntities();
            List<VisitorEntryTB> visitors = new List<VisitorEntryTB>();
            try
            {
                //visitors.Where(d => d.FromDate > DateTime.Now).ToList();
                DateTime fDate = new DateTime();
                DateTime tDate = new DateTime();
                if (!string.IsNullOrEmpty(fromdate))
                {
                    string strDate = fromdate;/* fromdate.Split('/')[1] + "/" + fromdate.Split('/')[0] + "/" + fromdate.Split('/')[2];*/
                    fDate = Convert.ToDateTime(strDate);
                }

                if (!string.IsNullOrEmpty(todate))
                {
                    string strDate = todate; /* todate.Split('/')[1] + "/" + todate.Split('/')[0] + "/" + todate.Split('/')[2];*/
                    tDate = Convert.ToDateTime(todate);
                }
                int intEmployeeID = 0;
                if (!string.IsNullOrEmpty(userID))
                {
                    intEmployeeID = Convert.ToInt32(userID);
                }

                var objData = (from d in entities.VisitorEntryTBs
                               where (d.FromDate > DateTime.Now
                                             && (!string.IsNullOrEmpty(userID) ? d.EmployeeId == intEmployeeID : true)
                                             && (!string.IsNullOrEmpty(company) ? d.Company.ToLower() == company.ToLower() : true)
                                             && (!string.IsNullOrEmpty(contactname) ? d.Name.ToLower() == contactname.ToLower() : true)
                                             && (!string.IsNullOrEmpty(dept) ? d.Name.ToLower() == dept.ToLower() : true)
                                             && (!string.IsNullOrEmpty(fromdate) ? d.FromDate >= fDate : true)
                                             && (!string.IsNullOrEmpty(todate) ? d.ToDate <= tDate : true)
                                             && (!string.IsNullOrEmpty(inTime) ? d.InTime == inTime : true))
                               select d).ToList();
                //visitors = (List<VisitorEntryTB>)entities.VisitorEntryTBs.ToList().OrderByDescending(d => d.Id);

                foreach (var item in objData)
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

            }
            return Json(Model);
        }

        public ActionResult GetVisitorListSearch(VisitorSearchModel visitor)
        {
            visitorEntries = GetVisitedVisitors();

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

        #endregion

        #region rejected visitor report

        [HttpPost]
        public JsonResult A_GetRejectedVisitorList(string userID = "")
        {
            List<VisitorEntryModel> Model = new List<VisitorEntryModel>();
            Model = GetRejectedVisitors(userID);
            return Json(Model);
        }

        public ActionResult GetRejectedVisitorList()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "RejectedVisitorList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewData["GetVisitorList"] = RejectedvisitorEntries = GetRejectedVisitors();
                return View();
            }
        }

        private static List<VisitorEntryModel> GetRejectedVisitors(string userID = "")
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
                var visitors = (from d in entities.VisitorEntryTBs
                                join c in entities.VisitorStatusTBs on d.Id equals c.VisitId
                                where c.Status == "Reject" && (!string.IsNullOrEmpty(userID) ? d.EmployeeId == intEmployeeID : true)
                                select d).ToList();
                //var visitors = entities.VisitorEntryTBs.ToList().OrderByDescending(d => d.Id);

                foreach (var item in visitors)
                {
                    //var status = entities.VisitorStatusTBs.Where(d => d.VisitId == item.Id).FirstOrDefault();

                    //if (status != null)
                    //{
                    //    if (status.Status == "Reject")
                    //    {
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
                    //    }
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
        [HttpPost]
        public JsonResult FilterGetRejectedVisitorList(string contactname, string company, string dept, string fromdate, string todate, string inTime, string userID = "")
        {
            List<VisitorEntryModel> Model = new List<VisitorEntryModel>();

            VMSDBEntities entities = new VMSDBEntities();
            List<VisitorEntryTB> visitors = new List<VisitorEntryTB>();
            try
            {
                DateTime fDate = new DateTime();
                DateTime tDate = new DateTime();
                if (!string.IsNullOrEmpty(fromdate))
                {
                    string strDate = fromdate;/* fromdate.Split('/')[1] + "/" + fromdate.Split('/')[0] + "/" + fromdate.Split('/')[2];*/
                    fDate = Convert.ToDateTime(strDate);
                }

                if (!string.IsNullOrEmpty(todate))
                {
                    string strDate = todate; /* todate.Split('/')[1] + "/" + todate.Split('/')[0] + "/" + todate.Split('/')[2];*/
                    tDate = Convert.ToDateTime(todate);
                }

                var objData = (from d in entities.VisitorEntryTBs
                               join c in entities.VisitorStatusTBs on d.Id equals c.VisitId
                               where (c.Status == "Reject"
                                             && (!string.IsNullOrEmpty(company) ? d.Company.ToLower() == company.ToLower() : true)
                                             && (!string.IsNullOrEmpty(contactname) ? d.Name.ToLower() == contactname.ToLower() : true)
                                             && (!string.IsNullOrEmpty(dept) ? d.Name.ToLower() == dept.ToLower() : true)
                                             && (!string.IsNullOrEmpty(fromdate) ? d.FromDate >= fDate : true)
                                             && (!string.IsNullOrEmpty(todate) ? d.ToDate <= tDate : true)
                                             && (!string.IsNullOrEmpty(inTime) ? d.InTime == inTime : true))
                               select d).ToList();
                //visitors = (List<VisitorEntryTB>)entities.VisitorEntryTBs.ToList().OrderByDescending(d => d.Id);

                foreach (var item in objData)
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

            }
            return Json(Model);
        }

        public ActionResult GetRejectVisitorListSearch(VisitorSearchModel visitor)
        {
            RejectedvisitorEntries = GetRejectedVisitors();

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

        #region export for reports

        [HttpPost]
        public FileResult VisitExportToExcel()
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

            visitorEntries = GetVisitedVisitors();

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
        public ActionResult VisitExportToPdf(int? pageNumber)
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

            visitorEntries = GetVisitedVisitors();

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
            return View("GetVisitedVisitorList");
        }

        [HttpPost]
        public FileResult RejectExportToExcel()
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

            visitorEntries = GetRejectedVisitors();

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
        public ActionResult RejectExportToPdf(int? pageNumber)
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

            visitorEntries = GetRejectedVisitors();

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
            return View("GetRejectedVisitorList");
        }
        [HttpPost]
        public FileResult UpcomingVisitExportToExcel()
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

            visitorEntries = GetUpcomingVisitors();

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
        public ActionResult UpcomingVisitExportToPdf(int? pageNumber)
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

            visitorEntries = GetUpcomingVisitors();

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
            return View("GetVisitedVisitorList");
        }

        #endregion
    }
}