using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VMS.Models.Admin
{
    public class CompanyModel
    {
        public int Id { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter Company")]
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}