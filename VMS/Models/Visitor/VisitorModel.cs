using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VMS.Models.Visitor
{
    public class VisitorModel
    {
        public int Id { get; set; }
        public string VisitorId { get; set; }
        public string Name { get; set; }       
        public string Company { get; set; }
        public string Photo { get; set; }
        public string EmailId { get; set; }
        public string Contact { get; set; }
    }

    public class GetEmpDataModel
    {
        public int EmployeeId { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmployeeEmailId { get; set; }
        public string EmployeeContact { get; set; }
    }

}