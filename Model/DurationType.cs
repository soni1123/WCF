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
    
    public partial class DurationType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DurationType()
        {
            this.Tests = new HashSet<Test>();
        }
    
        public int Id { get; set; }
        public Nullable<int> DurationMinute { get; set; }
        public int MaxQuestions { get; set; }
        public Nullable<int> TimeAdjust { get; set; }
        public bool ForApp { get; set; }
        public bool Status { get; set; }
        public bool Deleted { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public int SiteId { get; set; }
        public int OrgId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Test> Tests { get; set; }
    }
}
