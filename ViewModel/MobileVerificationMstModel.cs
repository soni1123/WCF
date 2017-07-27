using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tys361.WCF.ViewModel
{
    #region Register User
    public class MobileVerificationMstModel
    {
       // public int Id { get; set; }
        public string mobile_no { get; set; }
      //  public string VerificationCode { get; set; }
       // public string CurrentDate { get; set; }
       // public string SessFrom { get; set; }
       // public string SessTo { get; set; }
        public string full_name { get; set; }
        public string email { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
    }
    public class RegisterStudentModel
    {
       // public int Id { get; set; }
        public string mobile_no { get; set; }
        public string full_name { get; set; }
        public string email { get; set; }
    }

    public class otp
    {
        public int user_id { get; set; }
        public string mobile_no { get; set; }
        public string verification_code { get; set; }
        public string full_name { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
        public string email { get; set; }
    }
    public class resendotp
    {
        //public int Id { get; set; }
        public string full_name { get; set; }
        public string mobile_no { get; set; }
       public bool status { get; set; }
        public string message { get; set; }
        public string email { get; set; }
     

    }
    public class setpassword
    {
        //public int Id { get; set; }
        public string new_password { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
        public string confirm_paswword { get; set; }
    }
    public class changepassword
    {
        public string old_password { get; set; }
        public string new_password { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
        public string confirm_paswword { get; set; }
    }
    public class forgotpass
    {
        public string email { get; set; }
        public string new_password { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
        public string confirm_paswword { get; set; }
    }

    public class pass
    {
        public bool status { get; set; }
        public string message { get; set; }
    }

    #endregion

    #region Login User
    public class LoginEmailStudentModel
    {
        public bool is_password { get; set; }
        public string otp { get; set; }
        public bool is_email { get; set; }
        public string user_data { get; set; }
        public bool success { get; set; }
        public string message { get; set; }

    }
    public class LoginPasswrdStudentModel
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string password { get; set; }
        public string email_id { get; set; }
        public string user_name { get; set; }
        public string mobile { get; set; }
        public int user_id { get; set; }
        public bool is_email { get; set; }



    }

    public class LoginotpStudentModel
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string verification_code { get; set; }
        public string email_id { get; set; }
        public string user_name { get; set; }
        public string mobile { get; set; }
        public int user_id { get; set; }
        public bool is_email { get; set; }
    }



    public class LoginOtpStudentModel
    {
        public bool success { get; set; }
        public string message { get; set; }
        public bool is_email { get; set; }
        public string data { get; set; }

    }
    #endregion
}