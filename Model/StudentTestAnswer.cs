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
    
    public partial class StudentTestAnswer
    {
        public int Id { get; set; }
        public Nullable<int> StudentTestsId { get; set; }
        public Nullable<int> TestQuestionId { get; set; }
        public Nullable<int> MCQAnswersId { get; set; }
        public string Answers { get; set; }
        public Nullable<decimal> MarksObtained { get; set; }
        public Nullable<bool> IsNegative { get; set; }
        public string Remark { get; set; }
        public Nullable<bool> IsTopicWise { get; set; }
        public Nullable<bool> IsSkip { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<int> SiteId { get; set; }
        public Nullable<int> OrgId { get; set; }
    }
}
