using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tys361.WCF.ViewModel
{
    #region buynow
    public class packageSelectionModel
    {
        public int id { get; set; }
        public string class_name { get; set; }
     
    }

    public class packageModel
    {
        public string message { get; set; }
        public bool status { get; set; }
        public List<packageSelectionModel> pkgselection = new List<packageSelectionModel>();

    }
    public class PackageclassViewModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public string class_name { get; set; }
        public int? class_id { get; set; }



        //public int class_id { get; set; }
        //public string class_name { get; set; }


        //public IList<PackageclassListModel> AvailableClass { get; set; }
        public List<PackageclassListModel> AvailableClass = new List<PackageclassListModel>();
        public IList<PackageSubjectViewModel> getsubjectlist = new List<PackageSubjectViewModel>();


    }
    public class PackageclassListModel
    {
        public string package_name { get; set; }
        public string amount_per_month { get; set; }
        public string valid_till { get; set; }
        public string total_amount { get; set; }
        public string logo_url { get; set; }
        public string back_color { get; set; }
        public string options { get; set; }
        public int id { get; set; }
        //public int class_id { get; set; }
        //public string class_name { get; set; }
    }
    public class PackageupgradeViewModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public int class_id { get; set; }
        public string class_name { get; set; }
        //public IList<PackageupgradeListModel> AvailableupgradeClass { get; set; }
        public List<PackageupgradeListModel> AvailableupgradeClass = new List<PackageupgradeListModel>();
        public IList<PackageSubjectViewModel> getsubjectlist = new List<PackageSubjectViewModel>();


    }
    public class PackagegetModel
    {
        public int UserId { get; set; }
        public int PackageId { get; set; }
        public string BillingEmail
        { get; set; }
        public string TransactionId { get; set; }
        public int ClassId { get; set; }
        public string Amount
        { get; set; }
        public decimal GetAmount
        { get; set; }
        public string BillingName
        { get; set; }
        public string BillingPhone { get; set; }
        public bool Status { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public int SiteId { get; set; }
        public int OrgId { get; set; }
        public bool Ispaid { get; set; }
        public string Mode { get; set; }
        public string Description { get; set; }
        public bool IsApp { get; set; }
        public string PaymentStatus { get; set; }
        public string subject_list { get; set; }

    }

    public class statuspkg
    {
        public bool status { get; set; }
        public string message { get; set; }
    }

    public class PackageupgradeListModel
    {
        public string package_name { get; set; }
        public string amount_per_month { get; set; }
        public string difference_amount { get; set; }
        public string valid_till { get; set; }
        public string total_amount { get; set; }
        public string logo_url { get; set; }
        public string back_color { get; set; }
        public string options { get; set; }
        public int id { get; set; }
    }
    public class PackageclassModel
    {
       // public int id { get; set; }
        public int class_id { get; set; }
    }
    public class PackageclasstypeModel
    {
        // public int id { get; set; }
        public int class_id { get; set; }
    }

    public class PackageSubjectViewModel 
    {
        public int subject_id { get; set; }
        public string subject_name { get; set; }
        public bool is_checked { get; set; }
        //public bool isfirst { get; set; }
    }
    #endregion
}