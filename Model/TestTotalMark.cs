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
    
    public partial class TestTotalMark
    {
        public int Id { get; set; }
        public int StudentTestId { get; set; }
        public int TotalCorrectAns { get; set; }
        public int TotalWrongAns { get; set; }
        public decimal MarksObtained { get; set; }
        public decimal TotalNegativeMarks { get; set; }
        public decimal TotalObtainedMarks { get; set; }
        public string UploadedAnsFile { get; set; }
        public bool Status { get; set; }
        public bool Deleted { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    
        public virtual StudentTest StudentTest { get; set; }
    }
}