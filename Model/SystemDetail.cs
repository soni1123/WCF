//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace tys361.WCF.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class SystemDetail
    {
        public int Id { get; set; }
        public string MAC { get; set; }
        public Nullable<int> CreateTestId { get; set; }
        public Nullable<System.DateTime> StartedOn { get; set; }
        public Nullable<System.DateTime> EndedOn { get; set; }
        public bool IsCompleted { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public int SiteId { get; set; }
        public int OrgId { get; set; }
    }
}