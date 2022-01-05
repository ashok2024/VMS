using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VMS.Models.Employee
{
    public class ApproveRejectModel
    {
        public int VisitId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }       
    }
}