using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VMS.Controllers.Account;
using VMS.Middleware;
using VMS.Models.Admin;

namespace VMS.Controllers.Admin
{
    public class AdminController : BaseController
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin
        public ActionResult AdminDashboard()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "AdminDashboard";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                AdminDashboardModel dashboardModel = new AdminDashboardModel();
                dashboardModel = GetAdminDashboardDetails();
                return View("AdminDashboard", dashboardModel);
            }
        }

        private static AdminDashboardModel GetAdminDashboardDetails()
        {
            AdminDashboardModel Model = new AdminDashboardModel();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var visitors = entities.VisitorEntryTBs.ToList();
                Model.TotalVisitorCount = visitors.Count();

                var Aprovedvisitors = entities.VisitorStatusTBs.Where(d => d.Status == "Approve").ToList();
                Model.TotalVisitedCount = Aprovedvisitors.Count();

                var Rejectedvisitors = entities.VisitorStatusTBs.Where(d => d.Status == "Reject").ToList();
                Model.TotalRejectedVisitorCount = Rejectedvisitors.Count();

                var upcomevisitors = visitors.Where(d => d.FromDate > DateTime.Now).ToList();
                Model.TotalupcomingVisitorCount = upcomevisitors.Count();

                var deliveries = entities.CourierTBs.ToList();
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
    }
}