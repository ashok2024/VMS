using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VMS.Models.Visitor
{
    public class VisitorDashboardModel
    {
        public int TotalVisitorCount { get; set; }
        public int TotalVisitedCount { get; set; }
        public int TotalupcomingVisitorCount { get; set; }
        public int TotalRejectedVisitorCount { get; set; }
        public int TotalDefaultVisitorCount { get; set; }
        public int TotalDeliveries { get; set; }
    }
}