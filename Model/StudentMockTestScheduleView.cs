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
    
    public partial class StudentMockTestScheduleView
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public Nullable<int> UserDetailsId { get; set; }
        public Nullable<int> JumbleTypeId { get; set; }
        public Nullable<int> QuestionSetId { get; set; }
        public Nullable<int> ParticularYearId { get; set; }
        public Nullable<int> FromYear { get; set; }
        public Nullable<int> ToYear { get; set; }
        public Nullable<int> TestId { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Time { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<int> OrgId { get; set; }
        public Nullable<int> SiteId { get; set; }
        public Nullable<bool> IsTestChecked { get; set; }
        public Nullable<int> TestCheckedBy { get; set; }
        public string Refno { get; set; }
        public Nullable<int> QuestionPatternId { get; set; }
        public string Schedule { get; set; }
        public Nullable<bool> IsTYPTest { get; set; }
    }
}
