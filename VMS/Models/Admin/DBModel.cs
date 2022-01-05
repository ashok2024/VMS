using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VMS.Models.Admin
{
    public class DBModel
    {
        public string DataSource { get; set; }
        public string DBName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}