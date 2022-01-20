using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VMS.Controllers.Account;
using VMS.Middleware;
using VMS.Models.Visitor;
using VMS.Controllers.Admin;

namespace VMS.Controllers.Visitor
{
    public class VisitorDashboardController : BaseController
    {
        // GET: 
        public ActionResult Index()
        {
            return View();
        }
        // GET: VisitorDashboard
        public ActionResult VisitorDashboard()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "VisitorDashboard";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                VisitorDashboardModel dashboardModel = new VisitorDashboardModel();
                dashboardModel = GetVisitorDashboardDetails();
                Admin.SettingController _settingController = new SettingController();
                ViewBag.DeviceList = new SelectList(_settingController.GetDevices(), "DeviceId", "DeviceName");
                return View("VisitorDashboard", dashboardModel);
            }
        }

        private static VisitorDashboardModel GetVisitorDashboardDetails()
        {
            VisitorDashboardModel Model = new VisitorDashboardModel();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var visitors = entities.VisitorEntryTBs.ToList();
                Model.TotalVisitorCount = visitors.Count();

                var Aprovedvisitors = entities.VisitorStatusTBs.Where(d => d.Status == "Approve").ToList();
                Model.TotalVisitedCount = Aprovedvisitors.Count();

                var Rejectedvisitors = entities.VisitorStatusTBs.Where(d => d.Status == "Reject").ToList();
                Model.TotalRejectedVisitorCount = Rejectedvisitors.Count();

                var upcomevisitors = entities.VisitorEntryTBs.Where(d => d.FromDate > DateTime.Now).ToList();
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