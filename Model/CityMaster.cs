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
    
    public partial class CityMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CityMaster()
        {
            this.SchoolMasters = new HashSet<SchoolMaster>();
        }
    
        public int Id { get; set; }
        public Nullable<int> StateMasterId { get; set; }
        public string CityName { get; set; }
        public string Code { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> Deleted { get; set; }
    
        public virtual StateMaster StateMaster { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SchoolMaster> SchoolMasters { get; set; }
    }
}
