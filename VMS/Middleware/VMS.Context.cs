﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VMS.Middleware
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class VMSDBEntities : DbContext
    {
        public VMSDBEntities()
            : base("name=VMSDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<ApiMonitorTB> ApiMonitorTBs { get; set; }
        public DbSet<BranchTB> BranchTBs { get; set; }
        public DbSet<CompanyTB> CompanyTBs { get; set; }
        public DbSet<CourierTB> CourierTBs { get; set; }
        public DbSet<DBSettingTB> DBSettingTBs { get; set; }
        public DbSet<DepartmentTB> DepartmentTBs { get; set; }
        public DbSet<DesignationTB> DesignationTBs { get; set; }
        public DbSet<DeviceLogsTB> DeviceLogsTBs { get; set; }
        public DbSet<DevicesTB> DevicesTBs { get; set; }
        public DbSet<EmployeePunchTB> EmployeePunchTBs { get; set; }
        public DbSet<EmployeeScheduledVisitTB> EmployeeScheduledVisitTBs { get; set; }
        public DbSet<MailSettingTB> MailSettingTBs { get; set; }
        public DbSet<UserTB> UserTBs { get; set; }
        public DbSet<VisitorEntryTB> VisitorEntryTBs { get; set; }
        public DbSet<VisitorStatusTB> VisitorStatusTBs { get; set; }
        public DbSet<VisitorTB> VisitorTBs { get; set; }
    }
}
