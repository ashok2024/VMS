using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VMS.Controllers.Account;
using VMS.Middleware;
using VMS.Models.Account;
using VMS.Models.Visitor;

namespace VMS.Controllers.Visitor
{
    public class CourierController : BaseController
    {
        List<CourierModel> couriers;

        public CourierController()
        {
            couriers = new List<CourierModel>();
        }

        // GET: Courier
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult A_GetCouriersList()
        {
            List<CourierModel> Model = new List<CourierModel>();
            Model = GetCouriers();
            return Json(Model);
        }

        public ActionResult GetCourierList()
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "CourierList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                ViewData["GetCourierList"] = couriers = GetCouriers();
                //ViewBag.Designation = GetEmployee();
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
                    courier.EmployeeId = item.EmployeeId;
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

        public ActionResult AddCourier(int Id)
        {
            string userId = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            ViewBag.UserId = userId;
            string userName = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserName = userName;

            ViewBag.ActivePage = "CourierList";

            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }
            else
            {
                CourierModel courier = new CourierModel();
                if (Id > 0)
                {
                    courier = GetCourierDetails(Id);
                }
                else
                {
                    courier.Date = DateTime.Now;
                }
                ViewBag.EmpList = new SelectList(GetEmployeeList(), "UserId", "Name");
                return View("AddCourier", courier);
            }
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

        private static CourierModel GetCourierDetails(int Id)
        {
            CourierModel Model = new CourierModel();

            VMSDBEntities entities = new VMSDBEntities();

            try
            {
                var data = entities.CourierTBs.Where(d=>d.Id == Id).FirstOrDefault();
               
                Model.Id = data.Id;
                Model.CourierNo = data.CourierNo;
                Model.CourierCompany = data.CourierCompany;
                Model.CourierPersonName = data.CourierPersonName;
                Model.EmployeeName = data.EmployeeName;
                Model.EmployeeId = data.EmployeeId;
                Model.Time = data.Time;
                Model.Date = Convert.ToDateTime(data.Date);
                Model.Description = data.Description;

            }
            catch (Exception ex)
            {
                return Model;
                throw ex;
            }
            return Model;
        }

        [HttpPost]
        public async Task<ActionResult> Save(CourierModel courier)
        {

            VMSDBEntities db = new VMSDBEntities();

            string userid = (Request["userId"] == null) ? "" : Request["userId"].ToString();
            string username = (Request["userName"] == null) ? "" : Request["userName"].ToString();
            ViewBag.UserId = userid;
            ViewBag.UserName = username;

            ViewBag.ActivePage = "CourierList";

            string sessionid = (System.Web.HttpContext.Current.Session["SessionId"] != null) ? System.Web.HttpContext.Current.Session["SessionId"].ToString() : String.Empty;
            if (string.IsNullOrEmpty(sessionid))
            {
                return RedirectToAction("LogOut", "Account");
            }
            if (AppUser == null)
            {
                return RedirectToAction("LogOut", "Account");
            }

            //if (supplier.Id == 0)
            //{
            //    ModelState.Remove("Id");
            //}

            if (ModelState.IsValid)
            {
                try
                {

                    if(courier.Id > 0)
                    {
                        var check = db.CourierTBs.Where(d => d.Id == courier.Id).FirstOrDefault();

                        check.CourierNo = courier.CourierNo;
                        check.CourierCompany = courier.CourierCompany;
                        check.CourierPersonName = courier.CourierPersonName;
                        check.EmployeeId = courier.EmployeeId;
                        var user = db.UserTBs.Where(d => d.UserId == courier.EmployeeId).FirstOrDefault();
                        check.EmployeeName = user.Name;
                        check.Time = courier.Time;
                        check.Date = courier.Date;
                        check.Description = courier.Description;
                        check.UserId = Convert.ToInt32(userid);
                        db.Entry(check).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        CourierTB tB = new CourierTB();
                        tB.CourierNo = courier.CourierNo;
                        tB.CourierCompany = courier.CourierCompany;
                        tB.CourierPersonName = courier.CourierPersonName;
                        tB.EmployeeId = courier.EmployeeId;
                        var user = db.UserTBs.Where(d => d.UserId == courier.EmployeeId).FirstOrDefault();
                        tB.EmployeeName = user.Name;
                        tB.Time = courier.Time;
                        tB.Date = courier.Date;
                        tB.Description = courier.Description;
                        tB.UserId = Convert.ToInt32(userid);
                        db.CourierTBs.Add(tB);
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
                ViewBag.EmpList = new SelectList(GetEmployeeList(), "UserId", "Name");
                return View("AddCourier", courier);
            }
            //return RedirectToAction("GetSupplier", new { userId = userid, userName = username });
            return RedirectToAction("GetCourierList", "Courier", new { userId = ViewBag.UserId, userName = ViewBag.UserName });
        }

        public ActionResult GetCourierListSearch(CourierSearchModel courier)
        {
            couriers = GetCouriers();

            if (!string.IsNullOrWhiteSpace(courier.DeliveryNo))
            {
                couriers = couriers.Where(d => d.CourierNo.Contains(courier.DeliveryNo)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(courier.DeliveryCompany))
            {
                couriers = couriers.Where(d => d.CourierCompany.Contains(courier.DeliveryCompany)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(courier.EmployeeName))
            {
                couriers = couriers.Where(d => d.EmployeeName.Contains(courier.EmployeeName)).ToList();
            }

            if (!string.IsNullOrEmpty(courier.FromDate) && string.IsNullOrEmpty(courier.ToDate))
            {
                couriers = couriers.Where(d => d.Date <= Convert.ToDateTime(courier.FromDate)).ToList();
            }

            if (!string.IsNullOrEmpty(courier.FromDate) && !string.IsNullOrEmpty(courier.ToDate))
            {
                couriers = couriers.Where(d => d.Date >= Convert.ToDateTime(courier.FromDate) && d.Date <= Convert.ToDateTime(courier.ToDate)).ToList();
            }

            if (!string.IsNullOrEmpty(courier.Time))
            {
                couriers = couriers.Where(d => Convert.ToDateTime(d.Time) >= Convert.ToDateTime(courier.Time) && Convert.ToDateTime(d.Time) <= Convert.ToDateTime(courier.Time)).ToList();
            }

            return Json(couriers, JsonRequestBehavior.AllowGet);
        }
    }
}