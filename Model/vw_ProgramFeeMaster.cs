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
    
    public partial class vw_ProgramFeeMaster
    {
        public string PROGRAM_CODE { get; set; }
        public string PROGRAM_NAME { get; set; }
        public string EFFECTIVE_DATE { get; set; }
        public string PERSON { get; set; }
        public Nullable<decimal> APP_FEE { get; set; }
        public Nullable<decimal> CONSULT_FEE { get; set; }
        public bool STATUS { get; set; }
        public bool FEE_STATUS { get; set; }
        public string BR_ID { get; set; }
        public string UPDATED_BY { get; set; }
        public string LAST_UPDATED { get; set; }
        public int ID { get; set; }
    }
}