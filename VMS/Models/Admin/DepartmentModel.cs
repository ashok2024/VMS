using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VMS.Models.Admin
{
    public class DepartmentModel
    {
        public int Id { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter Department")]
        public string Name { get; set; }        
        [Required(ErrorMessage = "Please select Company")]
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Please select Branch")]
        public int BranchId { get; set; }
        public string BranchName { get; set; }
    }
}