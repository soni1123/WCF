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
    
    public partial class MCQAnswersMock
    {
        public int Id { get; set; }
        public Nullable<int> TestQuestionMockId { get; set; }
        public string Answer { get; set; }
        public string Image { get; set; }
        public bool IsCorrect { get; set; }
        public Nullable<int> OrderNo { get; set; }
        public bool Status { get; set; }
        public bool Deleted { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<int> SiteId { get; set; }
        public Nullable<int> OrgId { get; set; }
    }
}