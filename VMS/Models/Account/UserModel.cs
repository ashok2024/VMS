using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VMS.Models.Account
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Photo { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
        public string Gst { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime LastLoginDate { get; set; }        
        public string UserType { get; set; }
    }
}