using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VMS.Models.Admin
{
    public class DeviceModel
    {
        public int DeviceId { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter device name")]
        public string DeviceName { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter device serial No.")]
        public string DeviceSerialNo { get; set; }
        public string IPAddress { get; set; }
        public string DeviceStatus { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter Location")]
        public string DeviceLocation { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter Account Id")]
        public string DeviceAccountId { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter IP Address")]
        public string DeviceIPAddress { get; set; }
        [MaxLength(5)]
        [Required(ErrorMessage = "Please enter port")]
        public string Port { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter username")]
        public string UserName { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter password")]
        public string Password { get; set; }
    }

    public class DeviceLogSearchModel
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public DateTime fromdate { get; set; }
        public DateTime todate { get; set; }
        public List<SelectListItem> DeviceList { get; set; }
    }

    public class EmployeePunchModel
    {
        public int Id { get; set; }
        public string major { get; set; }
        public string minor { get; set; }
        public DateTime time { get; set; }
        public string ADate { get; set; }
        public string Atime { get; set; }
        public string employeeNoString { get; set; }
        public string cardNo { get; set; }
        public string cardReaderNo { get; set; }
        public string doorNo { get; set; }
        public string currentVerifyMode { get; set; }
        public string serialNo { get; set; }
        public string type { get; set; }
        public string mask { get; set; }
        public string name { get; set; }
        public string userType { get; set; }
        public string deviceIndex { get; set; }
        public string deviceAccountId { get; set; }
    }

    public class ApiMonitorModel
    {
        public int Id { get; set; }
        public string Page { get; set; }
        public string Action { get; set; }
        public DateTime time { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string DeviceName { get; set; }
        public string DeviceSRNo { get; set; }
        public string Status { get; set; }
        public string Command { get; set; }
    }

}