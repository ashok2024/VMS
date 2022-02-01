using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VMS.Models.Visitor
{
    public class CourierModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter Courier Number")]
        public string CourierNo { get; set; }
        [Required(ErrorMessage = "Please select employee")]
        public int? EmployeeId { get; set; }

        public string EmployeeName { get; set; }        
        public string CourierCompany { get; set; }
        public string CourierPersonName { get; set; }
        [Required(ErrorMessage = "Please enter Time")]
        public string Time { get; set; }
        [Required(ErrorMessage = "Please enter Date")]
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public string photo { get; set; }
        public string certificate { get; set; }
        public string PhotoPath { get; set; }
        public string certificatePath { get; set; }
    }

    public class CourierSearchModel
    {
        public string DeliveryNo { get; set; }
        public string DeliveryCompany { get; set; }
        public string EmployeeName { get; set; }        
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Time { get; set; }
    }
}