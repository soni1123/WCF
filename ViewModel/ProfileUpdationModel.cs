using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tys361.WCF.ViewModel
{
    public class ProfileUpdationModel 
    {
        public ProfileUpdationModel()
        {
            //AvilabeProfile = new List<ProfileUpdationModel>();
        }
        //public int id { get; set; }
        public int userid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string mobile { get; set; } 
        public string father_name { get; set; }
        public string mother_name { get; set; }
        public string parent_email { get; set; }
        public string parent_mobile { get; set; }
        public string parentalternative_email { get; set; }
        public string parentalternative_mobile { get; set; }
        public string address { get; set; }

    }
    public class ProfilesUpdation
    {
       // public IList<ProfileUpdationModel> AvilabeProfile { get; set; }
        public string message { get; set; }
        public bool status { get; set; }

    }
    public class GetUpdationModel
    {
        public string name { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string gender { get; set; }
        public string father_name { get; set; }
        public string mother_name { get; set; }
        public string parent_email { get; set; }
        public string parent_mobile { get; set; }
        public string parentalternative_email { get; set; }
        public string parentalternative_mobile { get; set; }
        public string address { get; set; }
        public string message { get; set; }
        public bool status { get; set; }

    }
  
}