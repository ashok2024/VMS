using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VMS.Controllers.Account;
using VMS.Middleware;
using VMS.Models.Admin;
using VMS.Controllers.Admin;

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
                Admin.MasterController _masterController = new MasterController();
                Admin.SettingController _settingController = new SettingController();
                ViewBag.CompanyList = new SelectList(_masterController.GetCompanyList(), "Id", "Name");
                ViewBag.DeviceList = new SelectList(_settingController.GetDevices(), "DeviceId", "DeviceName");
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
        //public List<DeviceModel> GetDevices(int CompanyId)
        //{
        //    List<DeviceModel> Model = new List<DeviceModel>();
        //    Admin.SettingController _settingController = new SettingController();
        //    ViewBag.DeviceList = new SelectList(_settingController.GetDevicesByCompanyId(CompanyId), "DeviceId", "DeviceName");
        //    return Model;
        //}
        [HttpPost]
        public ActionResult GetDevices(int CompanyId)
        {
            Admin.SettingController _settingController = new SettingController();
            ViewBag.DeviceList = new SelectList(_settingController.GetDevicesByCompanyId(CompanyId), "DeviceId", "DeviceName");
            return new EmptyResult();
        }
        public ActionResult GetFilter(int DeviceId)
        {
            AdminDashboardModel Model = new AdminDashboardModel();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var visitors = entities.VisitorEntryTBs.Where(x => x.DeviceId == DeviceId).ToList();
                Model.TotalVisitorCount = visitors.Count();

                var Aprovedvisitors = entities.VisitorStatusTBs.Where(d => d.Status == "Approve").ToList();
                Model.TotalVisitedCount = Aprovedvisitors.Count();

                var Rejectedvisitors = entities.VisitorStatusTBs.Where(d => d.Status == "Reject").ToList();
                Model.TotalRejectedVisitorCount = Rejectedvisitors.Count();

                var upcomevisitors = visitors.Where(d => d.FromDate > DateTime.Now).ToList();
                Model.TotalupcomingVisitorCount = upcomevisitors.Count();

                //var deliveries = entities.CourierTBs.ToList();
                //Model.TotalDeliveries = deliveries.Count();

                Model.TotalDefaultVisitorCount = 0;

                string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
                ViewBag.UserId = userId;
                string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
                ViewBag.UserName = userName;

                ViewBag.ActivePage = "AdminDashboard";

                Admin.MasterController _masterController = new MasterController();
                Admin.SettingController _settingController = new SettingController();
                ViewBag.CompanyList = new SelectList(_masterController.GetCompanyList(), "Id", "Name");
                ViewBag.DeviceList = new SelectList(_settingController.GetDevices(), "DeviceId", "DeviceName");

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View("AdminDashboard", Model);
        }

    }
}