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
    
    public partial class LectureVideo
    {
        public int Id { get; set; }
        public Nullable<int> TopicId { get; set; }
        public Nullable<int> VideoTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string FileDoc { get; set; }
        public bool Status { get; set; }
        public bool Deleted { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<int> SiteId { get; set; }
        public Nullable<int> OrgId { get; set; }
    
        public virtual Topic Topic { get; set; }
    }
}
