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
    
    public partial class Feedback
    {
        public int Id { get; set; }
        public string Answer { get; set; }
        public Nullable<int> FeedBackQuestionId { get; set; }
        public Nullable<int> StudentId { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<int> TeacherId { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<int> SiteId { get; set; }
        public Nullable<int> OrgId { get; set; }
        public int FeedbackAgainstStudentId { get; set; }
        public bool ForParent { get; set; }
        public int ForParentId { get; set; }
        public string options { get; set; }
    
        public virtual FeedbackQuestion FeedbackQuestion { get; set; }
    }
}
