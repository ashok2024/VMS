using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VMS.Models.Account
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please enter Email Address")]
        [Display(Name = "Username")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Invalid Password")]
        public string Password { get; set; }

        [Display(Name = "Keep me signed in untill I sign out.")]
        public bool RememberMe { get; set; }

        public string SessionId { get; set; }        
        public string UserType { get; set; }
    }
}