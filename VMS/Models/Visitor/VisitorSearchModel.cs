using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VMS.Models.Visitor
{
    public class VisitorSearchModel
    {
        public string Contact { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Time { get; set; }
        public int UserId { get; set; }
    }
}