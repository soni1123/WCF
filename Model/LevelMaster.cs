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
    
    public partial class LevelMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LevelMaster()
        {
            this.Categories = new HashSet<Category>();
        }
    
        public int Id { get; set; }
        public Nullable<int> SubjectId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TopicId { get; set; }
        public string LevelName { get; set; }
        public string LevelCode { get; set; }
        public bool Status { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public int SiteId { get; set; }
        public int OrgId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Category> Categories { get; set; }
    }
}
