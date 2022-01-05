using System.ComponentModel.DataAnnotations;

namespace VMS.Models.Admin
{
    public class BranchModel
    {
        public int Id { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter Branch")]
        public string Name { get; set; }        
        [Required(ErrorMessage = "Please select Company")]
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}