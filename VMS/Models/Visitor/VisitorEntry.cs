using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VMS.Models.Visitor
{
    public class VisitorEntryModel
    {
        public int Id { get; set; }
        public string VisitorTBId { get; set; }
        public string VisitorId { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter Name")]
        public string Name { get; set; }
        [MaxLength(100)]
        [Required(ErrorMessage = "Please enter the company")]
        public string Company { get; set; }
       
        public string Address { get; set; }

        public string VisitorType { get; set; }

        [Required(ErrorMessage = "Please select the device")]
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }

        [Required(ErrorMessage = "Please enter card number")]
        public string CardNo { get; set; }

        [Required(ErrorMessage = "Please enter the In time")]
        public string InTime { get; set; }

        [Required(ErrorMessage = "Please enter the Out time")]
        public string OutTime { get; set; }

        [Required(ErrorMessage = "Please select Id proof")]
        public string IdProof { get; set; }

        [Required(ErrorMessage = "Please enter valid Proof Number")]
        public string IdProofNo { get; set; }

        public string PhotoPath { get; set; }        

        [Required(ErrorMessage = "Please enter the visit date from")]
        public DateTime? VisitDateFrom { get; set; }

        [Required(ErrorMessage = "Please enter the visit date to")]
        public DateTime? VisitDateTo { get; set; }

        [MaxLength(50)]
        [EmailAddress(ErrorMessage = "Please enter valid Email Id")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Please enter the Contact")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string Contact { get; set; }
        [MaxLength(200)]
        [Required(ErrorMessage = "Please enter the purpose")]
        public string Purpose { get; set; }
        public string Remark { get; set; }
        public string Priority { get; set; }        

        [Required(ErrorMessage = "Please select the employee")]
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmailId { get; set; }
        public string EmployeeContact { get; set; }
        public string EmployeeDepartment { get; set; }

        public string Department { get; set; }
        public string Designation { get; set; }

        public string StatusRemark { get; set; }
        public string Status { get; set; }

        public string Material { get; set; }
        public string VehicleNo { get; set; }
        public string VehicleType { get; set; }
        public string VehiclePUCNo { get; set; }
        public DateTime? PUCEndDate { get; set; }
    }

    public class StatusModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class IdProofModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class PriorityModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class MaterialModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class VisitorTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class VehicleTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}