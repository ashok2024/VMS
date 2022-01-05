using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VMS.Models.Admin
{
    public class DesignationModel
    {
        public int Id { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter Designation")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please select Department")]
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        [Required(ErrorMessage = "Please select Company")]
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Please select Branch")]
        public int BranchId { get; set; }
        public string BranchName { get; set; }
    }
}