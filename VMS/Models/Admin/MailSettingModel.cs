using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VMS.Models.Admin
{
    public class MailSettingModel
    {
        public int Id { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter host")]
        public string Host { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter smtpfrom")]
        public string smtpfrom { get; set; }

        [StringLength(3)]
        [Required(ErrorMessage = "Please enter port")]
        public string port { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter username")]
        public string username { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter password")]
        public string password { get; set; }
    }
}