using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VMS.Models.Employee
{
    public class EmployeeModel
    {
        public int UserId { get; set; }
        public string EmpCode { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Photo { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }        
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        public int? BranchId { get; set; }
        public string Branch { get; set; }
        public int? CompanyId { get; set; }
        public string Company { get; set; }
        public int? DepartmentId { get; set; }
        public string Department { get; set; }
        public int? DesignationId { get; set; }
        public string Designation { get; set; }

        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string CardNo { get; set; }
        public string IdProof { get; set; }
        public string IdProofNo { get; set; }
        public string PhotoPath { get; set; }
    }

    public class EmployeeDashboardModel
    {
        public int TotalVisitorCount { get; set; }
        public int TotalVisitedCount { get; set; }
        public int TotalupcomingVisitorCount { get; set; }
        public int TotalRejectedVisitorCount { get; set; }
        public int TotalDefaultVisitorCount { get; set; }
        public int TotalDeliveries { get; set; }
    }

    public class AddEmployeeModel
    {
        public int UserId { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter employee Code")]
        public string EmpCode { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter Name")]
        public string Name { get; set; }       
        [MaxLength(50)]
        [EmailAddress(ErrorMessage = "Please enter valid Email Id")]
        public string EmailId { get; set; }
        public string Contact { get; set; }            
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }        
        public string UserName { get; set; }        
        public string Password { get; set; }

        [Required(ErrorMessage = "Please select the usertype")]
        public string UserType { get; set; }
        
        [Required(ErrorMessage = "Please select the company")]
        public int? CompanyId { get; set; }
        public string Company { get; set; }

        [Required(ErrorMessage = "Please select the Branch")]
        public int? BranchId { get; set; }
        public string Branch { get; set; }

        [Required(ErrorMessage = "Please select the department")]
        public int? DepartmentId { get; set; }
        public string Department { get; set; }

        [Required(ErrorMessage = "Please select the designation")]
        public int? DesignationId { get; set; }
        public string Designation { get; set; }

        [Required(ErrorMessage = "Please select the device")]
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        [Required(ErrorMessage = "Please enter card number")]
        public string CardNo { get; set; }
        
        public string IdProof { get; set; }
        [Required(ErrorMessage = "Please enter valid Adhar Number")]
        public string IdProofNo { get; set; }
        public string PhotoPath { get; set; }
    }

    public class ImportEmployeeModel
    {
        public int UserId { get; set; }
        public string EmpCode { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }        
        public string Company { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string CardNumber { get; set; }
    }

    public class ImportExcel
    {
        [Required(ErrorMessage = "Please select file")]
        [FileExt(Allow = ".xls,.xlsx", ErrorMessage = "Only excel file")]
        public HttpPostedFileBase file { get; set; }
    }

    public class SessionModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }   
}