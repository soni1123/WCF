
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

using System.Threading.Tasks;
using tys361.WCF.CommonBL;
using tys361.WCF.Model;
using tys361.WCF.ViewModel;
using System.Web;
using System.Data.Entity;
using System.Net;

namespace tys361.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class tysService : ItysService

    {
        DBContextEntities con = new DBContextEntities();
        DBContextEntities  _defContexts = new DBContextEntities();

        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConn"].ConnectionString);
        CommBL comm = new CommBL();
        private readonly DateTime _currDateTime;

        public static string Otp = "";
        public int userId = 0;

        public bool IsValidUser()
        {
            try
            {
                IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
                WebHeaderCollection headers = request.Headers;
                string Provider = headers["Provider"].ToString();
                string ProviderUserId = headers["UserId"].ToString();
                string UserName = headers["MobileOrEmail"].ToString();

                if (Provider == "xiphias")
                {
                    var data = con.UserDetails.Where(r => r.Email == UserName || r.Mobile == UserName).ToList();
                    userId = data.Count() > 0 ? data.FirstOrDefault().UserId : 0;
                    if (userId != 0)
                    {
                        if (userId == Convert.ToInt32(ProviderUserId))
                            return true;
                    }
                    else
                    {
                        userId = 0; return false;
                    }
                    return false;
                }
                else
                {

                    userId = 0; return false;
                }
            }
            catch
            {
                userId = 0;
                return false;
            }
        }


        tysService()
        {
            this._currDateTime = DateTime.Now;
        }
        public string GetData()
        {
            if (IsValidUser())
            {
                return "Hi Guys, This is me";
            }
            else
                return "Not Authenticate user";
        }

        #region Register User
        public MobileVerificationMstModel MobileVerify(RegisterStudentModel regis)
        {
            MobileVerificationMstModel model = new MobileVerificationMstModel();
            model.email = regis.email;
            model.mobile_no = regis.mobile_no;
            model.full_name = regis.full_name;

            var date1 = con.MobileVerificationMsts.Where(a => a.MobileNo == regis.mobile_no && a.IsRegistered == true).ToList();
            if (date1.Count() > 0)
            {
                model.message = "This mobile number has already been registered...!";
                return model;
            }
            var date2 = con.MobileVerificationMsts.Where(a => a.Email == regis.email && a.IsRegistered == true).ToList();
            if (date2.Count() > 0)
            {
                model.message = "This e-mail Id has already been registered...!";
                return model;
            }
            if (regis.mobile_no == null)
            {
                model.message = "Please select Mobile no.";
                return model;
            }
            if (regis.mobile_no.Length != 10)
            {
                model.message = "Please enter 10 digits mobile no.";
                return model;
            }
            var data3= con.MobileVerificationMsts.Where(a => a.Email == regis.email &&a.MobileNo==regis.mobile_no  && a.IsRegistered == false).ToList();
            if (data3.Count > 0)
            {
                int i = comm.RegisteralerdyUser(regis);
                if (i > 0)
                {
                    model.message = "Success";
                    model.status = true;
                    return model;
                }
                else
                {
                    model.message = "Please the check the details";
                    model.status = false;
                    return model;
                }
            }
            else {
                int i = comm.RegisterUser(regis);
                if (i > 0)
                {
                    model.message = "Success";
                    model.status = true;
                    return model;
                }
                else
                {
                    model.message = "Incorrect details entered during pervious registration";
                    model.status = false;
                    return model;
                }
            }
           
           
        }

        public otp MobileVerifyOTP(otp otp)
        {
            var dateOTP1 = con.MobileVerificationMsts.Where(a => a.VerificationCode == otp.verification_code && a.IsRegistered == true).ToList();
            var dateOTP = con.MobileVerificationMsts.Where(a => a.VerificationCode == otp.verification_code && a.IsRegistered == false).ToList();
            int Id = 0;
            var mobilenumber = "";
            var name = "";
            var email = "";
            foreach (var itm in dateOTP)
            {
                mobilenumber = itm.MobileNo;
                Id = (int)itm.Id;
                name = itm.FullName;
                email = itm.Email;
            }

            if (dateOTP1.Count > 0)
            {

                string inval = "Invalid";
                otp.message = ("You are already Registered,Please Sign in");


            }

            else if (dateOTP.Count == 0)
            {

                string inval = "Invalid";
                otp.message = ("Please Enter Valid Verification Code");


            }
            else
            {
                string date1 = DateTime.Now.ToString();
                //DateTime c = Convert.ToDateTime(date.LastOrDefault().CurrentDate);
                DateTime c = Convert.ToDateTime(date1);
                TimeSpan diff = DateTime.Now - c;
                int min = diff.Minutes;

                if (min > 15)
                {

                    otp.verification_code = "";
                    string ExpireLink = "Expired";
                    otp.message = ("Verification Code is Expired,Please Try Again Later.");

                }
                int i = comm.OTP_verify(otp, name, email);
                if (i > 0)
                {
                    otp.message = "Success";
                    otp.status = true;
                    otp.user_id = con.AspNetUsers.Where(r => r.UserName == otp.email).FirstOrDefault().UId;
                    otp.full_name = name;
                    return otp;
                }
                else
                {
                    otp.message = "Please enter the correct OTP  ";
                    otp.status = false;
                    return otp;
                }

            }
            return otp;
        }

        public resendotp MobileVerifyresendOTP(resendotp resendotp)
        {
            var data = con.MobileVerificationMsts.Where(r => r.MobileNo == resendotp.mobile_no && r.Email == resendotp.email && r.IsRegistered == false).FirstOrDefault();
            var dataId = data != null ? data.Id : 0;

            int i = comm.Resend_OTP(resendotp, dataId);

            if (i > 0)
            {
                resendotp.message = "Success";
                resendotp.status = true;
                return resendotp;
            }
            else
            {
                resendotp.message = "Please enter the correct OTP ";
                resendotp.status = false;
                return resendotp;
            }
        }

        public pass set_password(setpassword set)
        {
            pass mod = new pass();
            try
            {
                if (IsValidUser())
                {
                    var email = con.AspNetUsers.Where(r => r.UId == userId).FirstOrDefault().Email;
                    if (userId == 0)
                    {
                        mod.message = "Unauthorized access";
                        mod.status = false;
                        return mod;

                    }

                    if (set.new_password == set.confirm_paswword)
                    {

                        string newpass = comm.HashPassword(set.confirm_paswword);
                        var identity = con.AspNetUsers.Where(r => r.UId == userId).FirstOrDefault();
                        identity.PasswordHash = newpass;
                        con.Entry(identity).State = EntityState.Modified;
                        con.SaveChanges();

                        var data = con.UserDetails.Where(r => r.Email == email).FirstOrDefault();
                        data.IsSetPass = true;
                        con.Entry(data).State = EntityState.Modified;
                        con.SaveChanges();
                        mod.message = "Password successfully Set.";
                        mod.status = true;
                        return mod;
                    }
                    else
                    {
                        mod.message = "Please check the confirm Password";
                        mod.status = false;
                        return mod;
                    }
                }
                else
                {
                    mod.message = "No Authentication User";
                    mod.status = false;
                    return mod;
                }
            }
            catch (Exception ex)
            {
                mod.message = "Failed to set Password";
                mod.status = false;
                return mod;
            }
        }

        public pass change_password(changepassword set)
        {
            pass mod = new pass();
            try
            {
                if (IsValidUser())
                {
                    var email = con.AspNetUsers.Where(r => r.UId == userId).FirstOrDefault().Email;
                    var pass = con.AspNetUsers.Where(r => r.UId == userId).FirstOrDefault();
                    string oldpass = pass.PasswordHash;
                    bool data = comm.VerifyHashedPassword(pass.PasswordHash, set.old_password);
                    if (data == true)
                    {
                        if (userId == 0)
                        {
                            mod.message = "Unauthorized access";
                            mod.status = false;
                            return mod;
                        }
                        if (set.new_password == set.confirm_paswword)
                        {

                            string newpass = comm.HashPassword(set.confirm_paswword);
                            var identity = con.AspNetUsers.Where(r => r.UId == userId).FirstOrDefault();
                            identity.PasswordHash = newpass;
                            con.Entry(identity).State = EntityState.Modified;
                            con.SaveChanges();
                            mod.message = "Password successfully Set.";
                            mod.status = true;
                            return mod;
                        }
                        else
                        {
                            mod.message = "Please check the confirm Password";
                            mod.status = false;
                            return mod;
                        }
                    }
                    else
                    {
                        mod.message = "Please Enter your valid Old Password";
                        mod.status = false;
                        return mod;
                    }
                }
                else
                {
                    mod.message = "No Authentication User";
                    mod.status = false;
                    return mod;
                }
            }
            catch (Exception ex)
            {
                mod.message = "Failed to set Password";
                mod.status = false;
                return mod;
            }
        }

        public pass forgotPassword(string email)
        {
            pass mod = new pass();
            try
            {
                var email1 = con.AspNetUsers.Where(r => r.UserName == email).FirstOrDefault();
                if (email != null)
                {
                    mod.message = "Valid User";
                    mod.status = true;
                    return mod;
                }
                else
                {
                    mod.message = "Not Valid User";
                    mod.status = false;
                    return mod;
                }
            }
            catch (Exception ex)
            {
                mod.message = "Failed to set Password";
                mod.status = false;
                return mod;
            }

        }
        public pass setforgot_password(forgotpass set)
        {
            pass mod = new pass();
            try
            {

                var identity = con.AspNetUsers.Where(r => r.UserName == set.email).FirstOrDefault();
                if (identity != null)
                {
                    if (set.new_password == set.confirm_paswword)
                    {
                        string newpass = comm.HashPassword(set.confirm_paswword);
                        //var identity = con.AspNetUsers.Where(r => r.UId == userId).FirstOrDefault();
                        identity.PasswordHash = newpass;
                        con.Entry(identity).State = EntityState.Modified;
                        con.SaveChanges();

                        mod.message = "Password successfully reset.";
                        mod.status = true;
                        return mod;
                    }
                    else
                    {
                        mod.message = "Please check the confirm Password";
                        mod.status = false;
                        return mod;
                    }
                }
                else
                {
                    mod.message = "Not a valid user";
                    mod.status = false;
                    return mod;
                }

            }
            catch (Exception ex)
            {
                mod.message = "Failed to set Password";
                mod.status = false;
                return mod;
            }
        }
        #endregion

        #region Profile Updation

        public GetUpdationModel GetProfile()
        {
            GetUpdationModel profilemodel = new GetUpdationModel();
            try
            {
                if (IsValidUser())
                {
                    var data = con.UserDetails.Where(r => r.UserId == userId).FirstOrDefault();
                    var parentdata = con.ParentDetails.Where(r => r.UserId == userId).FirstOrDefault();
                    var email = data != null ? data.Email : null;
                    if (email == data.Email)
                    {
                        //profilemodel.Id = data.Id;
                        //profilemodel.UserId = data.UserId;
                        profilemodel.email = data.Email;
                        profilemodel.name = data.Name;
                        profilemodel.mobile = data.Mobile;
                        profilemodel.address = data.Address;
                        profilemodel.gender = data.Gender;
                        profilemodel.message = "success";
                        profilemodel.status = true;

                    }
                    if (parentdata != null)
                    {
                        profilemodel.father_name = parentdata.FatherName;
                        profilemodel.mother_name = parentdata.MotherName;
                        profilemodel.parentalternative_email = parentdata.ParentAlternativeEmail;
                        profilemodel.parentalternative_mobile = parentdata.ParentAlternativeMobile;
                        profilemodel.parent_email = parentdata.ParentEmail;
                        profilemodel.parent_mobile = parentdata.ParentMobile;
                    }
                    return profilemodel;
                }
                else
                {
                    profilemodel.message = "Not an authenticated user";
                    profilemodel.status = false;
                    return profilemodel;
                }
            } 
            catch (Exception ex)
            {
                profilemodel.message = "Unable to fetch information";
                profilemodel.status = false;
                return profilemodel;
            }
        }
        public ProfilesUpdation ProfileUpdation(ProfileUpdationModel profilemod)
        {
            ProfilesUpdation model = new ProfilesUpdation();
            try
            {
                if (IsValidUser())
                {
                    comm.Profileupdate(profilemod);
                    model.message = "success";
                    model.status = true;
                    return model;
                }
                else
                {
                    model.message = "Error while updating";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.message = "Unable to fetch information";
                model.status = false;
                return model;
            }

        }

        #endregion

        #region Profile complete
        public ProfilePercentageModel profilePer()
        {
            if (IsValidUser())
            {
                ProfilePercentageModel model = new ProfilePercentageModel();
                int profilePercentage = 23;
                var userDetail = con.UserDetails.Where(u => u.UserId == userId).FirstOrDefault();

                if (userDetail != null)
                {
                    profilePercentage = profilePercentage + 21;
                    if (userDetail.Address != null)
                        profilePercentage = profilePercentage + 10;
                    if (userDetail.ImageUrl != null)
                    {
                        profilePercentage = profilePercentage + 23;
                        //model.profilePic = userDetail.ImageUrl.Split('/').Last();
                    }
                    //else
                    //{
                    //    model.profilePic = "NoImage.jpg";
                    //}
                    if (userDetail.Gender != null && userDetail.Gender != "0")
                        profilePercentage = profilePercentage + 23;

                    model.Mobile = userDetail.Mobile;
                    model.Address = userDetail.Address;
                    model.Email = userDetail.Email;
                    model.profilePercentage = profilePercentage;
                }

                return model;
            }

            else
                return null;
        }



        #endregion


        public DashBoardViewModel Dashboard()
        {
            DashBoardViewModel mod = new DashBoardViewModel();
            try
            {
                if (IsValidUser())
                {

                    var classID = con.UserDetails.Where(r => r.UserId == userId).FirstOrDefault().ClassId;
                    mod.pakages.Package = "Buy package";
                    mod.pakages.valid_till = "0";
                    if (classID != 0)
                    {
                        if (con.StudentPackageSelections.Where(c => c.UserId == userId).FirstOrDefault() != null)
                        {
                            int pkgId = con.StudentPackageSelections.Where(c => c.UserId == userId && c.Ispaid == true).OrderByDescending(r => r.Id).FirstOrDefault().PackageId != 0 ?
                            con.StudentPackageSelections.Where(c => c.UserId == userId && c.Ispaid == true).OrderByDescending(r => r.Id).FirstOrDefault().PackageId : 0;
                            var pkgSel = con.PackageSelections.Where(p => p.PackageId == pkgId && p.ClassId == classID).FirstOrDefault() != null ?
                               con.PackageSelections.Where(p => p.PackageId == pkgId && p.ClassId == classID).FirstOrDefault() : null;
                            int remDays = 0;
                            if (pkgSel != null)
                            {
                                TimeSpan validTil = (pkgSel.ValidTill - DateTime.Now);
                                remDays = validTil.Days;
                                var package = con.PackagesTypes.Where(p => p.Id == pkgSel.PackageId).FirstOrDefault();
                                var amount = con.StudentPackageSelections.Where(c => c.UserId == userId).FirstOrDefault().Amount.ToString("0.00");
                                mod.pakages.package_id = pkgSel.PackageId;
                                mod.pakages.Package = package.Name;
                                mod.pakages.color = package.BackGroundColor;
                                mod.pakages.amount = Convert.ToDecimal(amount);
                                mod.pakages.remender_days = remDays;
                                mod.pakages.valid_till = pkgSel.ValidTill.ToString("dd/MM/yyyy");
                                mod.pakages.Is_upgrade = true;

                                //package upgrade
                                var getdetails = con.UserDetails.Where(r => r.UserId == userId);
                                var getrecords = con.PackageSelections.Where(r => r.ClassId == getdetails.FirstOrDefault().ClassId).ToList();
                                int pkgid = Convert.ToInt32(getdetails.FirstOrDefault().PackageType);
                                var getprice = con.PackageSelections.Where(r => r.PackageId == pkgid && r.ClassId == getdetails.FirstOrDefault().ClassId);
                                mod.pakages.is_maximum_package = true;
                                foreach (var mm in getrecords)
                                {
                                    if (mm.Amount > getprice.FirstOrDefault().Amount)
                                    {
                                        mod.pakages.is_maximum_package = false;
                                    }
                                }

                            }
                        }
                    }
                    //rakshitha
                    mod.is_buyed_enable = false;
                    var getclasses = con.UserDetails.Where(r => r.UserId == userId).ToList();
                    if (getclasses.FirstOrDefault().ClassId == 0)
                    {
                        mod.is_buyed_enable = true;
                    }
                    else
                    {
                        mod.is_buyed_enable = false;
                    }

                    var getuserdetails = con.UserDetails.Where(r => r.UserId == userId).FirstOrDefault();
                    if (getuserdetails != null)
                    {
                        var getclss = con.ClassTypes.Where(r => r.Id == getuserdetails.ClassId).FirstOrDefault();
                        if (getclss != null)
                        {
                            string classname = getclss.Class;

                            var getsubjs = con.ClassMasters.Where(p => p.Class == classname).ToList();
                            foreach (var subj in getsubjs)
                            {
                                var getpkgselctn = con.StudentPackageSelections.Where(r => r.UserId == userId && r.SubjectId == subj.SubjectId).FirstOrDefault();
                                if (getpkgselctn == null)
                                {
                                    mod.is_buyed_enable = true;

                                }
                            }
                        }
                        else
                        {
                            mod.is_buyed_enable = true;
                        }
                    }
                    var checkuserdetail = con.StudentCategoryMappings.Where(r => r.UserDetailsId == userId && r.CategoryId != 0 && r.Deleted == false).ToList();
                    foreach (var item in checkuserdetail)
                    {
                        SubjectSelection sub = new SubjectSelection();
                        var subjectname = con.Categories.Distinct().Where(r => r.Id == item.CategoryId).ToList();
                        sub.subject_name = subjectname.FirstOrDefault().CategoryName;
                        mod.subject_list.Add(sub);
                    }

                    var udetails = con.UserDetails.Where(r => r.UserId == userId);
                    int UserDetailsId = udetails.Count() > 0 ? udetails.FirstOrDefault().Id : 0;
                    var studenttestId = con.TestAnswersEvaluations.Where(r => r.CandidateId == UserDetailsId).OrderByDescending(r => r.Id).ToList();

                    var countstudtest = con.StudentTests.Where(r => r.UserDetailsId == UserDetailsId && r.Deleted == false).ToList();
                    int totaltest = countstudtest.Count;

                    var getcount = con.ScheduleMockTests.Where(r => r.UserDetailsId == UserDetailsId && r.Deleted == false).ToList();


                    var counttest = con.StudentTestSchedules.Where(r => r.UserDetailsId == udetails.FirstOrDefault().Id && r.Deleted == false).ToList();
                    int schedulecount = counttest.Count + getcount.Count;


                    int rAnswre = 0;
                    int wAnswre = 0;
                    decimal Rightcal = 0;
                    decimal WrongCal = 0;
                    decimal TotalProf = 0;


                    foreach (var i in studenttestId)
                    {
                        if (Convert.ToBoolean(i.IsCorrect))
                            rAnswre = rAnswre + 1;
                        else
                            wAnswre = wAnswre + 1;
                    }
                    int count = studenttestId.Count();
                    if (count != 0)
                    {
                        Rightcal = Math.Round(((Convert.ToDecimal(rAnswre)) / (count)) * 100);
                        WrongCal = Math.Round(((Convert.ToDecimal(wAnswre)) / (count)) * 100);
                        TotalProf = Math.Round(((Convert.ToDecimal(rAnswre)) / (count)) * 100);
                    }

                    var testAnalytic = con.TestAnalytics.Where(t => t.UserId == userId && t.Deleted == false && t.Status == true).ToList();
                    int totalTime = 0;
                    Double speeeeed = 0;
                    if (testAnalytic.Count > 0)
                    {
                        foreach (var i in testAnalytic)
                        {
                            totalTime = Convert.ToInt32(totalTime + i.AnsTime);
                        }
                        speeeeed = (3600 * testAnalytic.Count()) / totalTime;
                    }
                    //mod.StudentProgress.Add(new StudentProgressModel
                    mod.StudentProgress.totalcorrect_ans = rAnswre;
                    mod.StudentProgress.totalwrong_ans = wAnswre;
                    mod.StudentProgress.accuracy = Rightcal;
                    mod.StudentProgress.wrongans_percentage = WrongCal;
                    mod.StudentProgress.correctans_percentage = Rightcal;
                    mod.StudentProgress.speed = Convert.ToDouble(Math.Round(speeeeed));
                    mod.StudentProgress.time_practiced = totalTime;
                    mod.StudentProgress.test_schedule = schedulecount;
                    mod.StudentProgress.total_test = totaltest;
                    mod.StudentProgress.total_attempt = count;

                    mod.Message = "success";
                    mod.Status = true;
                    return mod;
                }

                else
                {
                    mod.Message = "No Authentication User";
                    mod.Status = false;
                    return mod;
                }
            }
            catch (Exception ex)
            {
                mod.Message = "Failed";
                mod.Status = false;
                return mod;
            }
        }

        #region Student AllResult
        public StudnetAllResult StudentAllResult(int studenttestId1)
        {
            StudnetAllResult mod = new StudnetAllResult();
            try {
                if (IsValidUser())
                {
                    GetViewResult(studenttestId1);
                    var studenttestId = con.TestAnswersEvaluations.Where(r => r.CandidateId == userId).OrderByDescending(r => r.Id).ToList();
                    foreach (var j in studenttestId.GroupBy(r => r.StudentTestId).Select(g => g.FirstOrDefault()).Distinct())
                    {
                        var totalmark = con.TestTotalMarks.Where(r => r.StudentTestId == j.StudentTestId).ToList();
                        foreach (var i in totalmark)
                        {
                            string dates = con.StudentTests.Find(i.StudentTestId).CreatedOn.ToString();

                            var stTestanswersMod = con.StudentTestAnswerVws.Where(r => r.StudentTestsId == i.StudentTestId).ToList();
                            int val = Convert.ToInt32(stTestanswersMod.FirstOrDefault().TestQuestionId);
                            string getid = " ";
                            if (val != 0)
                            {
                                getid = con.TestSolutions.Where(r => r.TestQuestionsId == val).Count() != 0 ?
                                con.TestSolutions.Where(r => r.TestQuestionsId == val).FirstOrDefault().ModelAnswer : " ";
                            }
                            mod.AvailablestudentResult.Add(new StudentAllResultModel
                            {
                                id = i.Id,
                                totalcorrect_ans = i.TotalCorrectAns,
                                totalwrong_ans = i.TotalWrongAns,
                                studenttest_id = i.StudentTestId,
                                total_attempt = i.TotalCorrectAns + i.TotalWrongAns,
                                totalobtained_marks = i.TotalObtainedMarks,
                                totalnegative_marks = i.TotalNegativeMarks,
                                marks_obtained = i.MarksObtained,
                                created_on = dates.ToString(),
                                //TestOn = dates.ToString("dd") + "-" + dates.ToString("MMM"),
                                topic_name = con.Topics.Find(stTestanswersMod.FirstOrDefault().TopicId).TopicName,
                                question_patternName = con.QuestionPatterns.Find(stTestanswersMod.FirstOrDefault().QuestionPatternId).Name,
                            });
                        }
                    }
                    
                    mod.message = "Success";
                    mod.status = true;
                    return mod;
                }

                else {
                    mod.message = "Not an authenticated user";
                    mod.status = false;
                    return mod;
                }
            }
            catch (Exception ex)
            {
                mod.message = "Unable to fetch information";
                mod.status = false;
                return mod;
            }
        }


        #endregion

        #region Category Mock 
        /// <summary>
        /// Getting the list of the tests
        /// </summary>
        /// <returns></returns>
        public ListMock GetCatMockData()
        {
            ListMock model = new ListMock();
            try
            {
                if (IsValidUser())
                {
                    var userDetailsId = con.UserDetails.Where(r => r.UserId == userId).ToList();
                    int UserId = userDetailsId.Count() > 0 ? userDetailsId.FirstOrDefault().Id : 0;

                    var dataTestScheduled = con.StudentMockTestScheduleViews.Where(r => r.Schedule == "Start Now" && r.Deleted == false && r.Status == true && r.UserDetailsId == UserId && r.IsTYPTest == false);
                    //var dataTestScheduled = con.StudentMockTestScheduleViews.Where(r => r.Schedule == "Time Expired" && r.Deleted == false && r.Status == true && r.UserDetailsId == UserId);
                    int count = 1;
                    model.schedule_mock_test = dataTestScheduled.OrderByDescending(r => r.Id).ToList().Select(x =>
                    {
                        var m = new CategoryMock();
                        comm.PrepareModelScheduleMockTestTypes(m, x);
                        
                        count = count + 1;
                        m.display_date = m.date.ToString();

                        //m.display_date = m.date.ToString("dd-MM-yyyy");//ToString("dd-MM-yyyy h:mm:ss tt");
                        return m;
                    }).ToList();
                    if(model.schedule_mock_test.Count()>0)
                    {
                        model.message = "Success";
                    }
                    else
                    {
                        model.message = "Test was not scheduled successfully";
                    }
                   
                    model.status = true;
                    return model;
                }
                else
                {
                    model.message = "Not an authenticated user";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.message = "Please try again";
                model.status = false;
                return model;
            }
        }

        public ListMock GetCatMockTYPData()
        {
            ListMock model = new ListMock();
            try
            {
                if (IsValidUser())
                {
                    var userDetailsId = con.UserDetails.Where(r => r.UserId == userId).ToList();
                    int UserId = userDetailsId.Count() > 0 ? userDetailsId.FirstOrDefault().Id : 0;

                    var dataTestScheduled = con.StudentMockTestScheduleViews.Where(r => r.Schedule == "Start Now" && r.Deleted == false && r.Status == true && r.UserDetailsId == UserId && r.IsTYPTest==true);
                    //var dataTestScheduled = con.StudentMockTestScheduleViews.Where(r => r.Schedule == "Time Expired" && r.Deleted == false && r.Status == true && r.UserDetailsId == UserId);
                    int count = 1;
                    model.schedule_mock_test = dataTestScheduled.OrderByDescending(r => r.Id).ToList().Select(x =>
                    {
                        var m = new CategoryMock();
                        comm.PrepareModelScheduleMockTestTypes(m, x);

                        count = count + 1;
                        m.display_date = m.date.ToString();

                        //m.display_date = m.date.ToString("dd-MM-yyyy");//ToString("dd-MM-yyyy h:mm:ss tt");
                        return m;
                    }).ToList();
                    model.message = "success";
                    model.status = true;
                    return model;
                }
                else
                {
                    model.message = "No Authentication User";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.message = "Failed";
                model.status = false;
                return model;
            }
        }

        /// <summary>
        /// Total Question in test
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TestQuestionsListModel GetCategoryMockData(string id)
        {
            var model = new TestQuestionsListModel();
            try
            {
                if (IsValidUser())
                {
                    var userDetailsId = con.UserDetails.Where(r => r.UserId == userId).ToList();
                    var sts = con.ScheduleMockTests.Where(s => s.Id.ToString() == id).FirstOrDefault();
                    int? DurationTypeId = sts.DurationId;
                    var studenttest = new StudentTest();
                    model.category_id = Convert.ToInt32(id);
                    int? quespatternid = sts.QuestionPatternId;
                    var duration = con.DurationTypes.Find(DurationTypeId);
                    var testdetails = con.Tests.Where(r => r.Id == sts.TestId).FirstOrDefault(); // pick LevelId and TopicId
                    var TquestionsM = con.TestQuestionMocks.Where(r => r.LevelId == testdetails.LevelId && r.TopicId == testdetails.TopicId && r.QuestionPatternId == quespatternid).ToList();
                    //if (sts.JumbleTypeId != 0 && sts.FromYear != 0 && sts.ToYear != 0)
                    //{

                    //    TquestionsM = con.TestQuestionMocks.Where(r => r.LevelId == testdetails.LevelId && r.TopicId == testdetails.TopicId && r.QuestionPatternId == quespatternid && r.YearId >= sts.FromYear && r.YearId <= sts.ToYear).ToList();

                    //}
                    //else if (sts.JumbleTypeId != 0 && sts.FromYear == 0 && sts.ToYear == 0)
                    //{
                    //    TquestionsM = con.TestQuestionMocks.Where(r => r.LevelId == testdetails.LevelId && r.TopicId == testdetails.TopicId && r.QuestionPatternId == quespatternid).ToList();
                    //}
                    //else
                    //     if (sts.QuestionSetId != 0 && sts.ParticularYearId != 0)
                    //{
                    //    // int yearid = con.PreviousYears.Where(r => r.Year == sts.ParticularYearId).FirstOrDefault().Id;
                    //    TquestionsM = con.TestQuestionMocks.Where(r => r.LevelId == testdetails.LevelId && r.TopicId == testdetails.TopicId && r.QuestionPatternId == quespatternid && r.YearId == sts.ParticularYearId).ToList();
                    //}

                    List<TestQuestionMock> total_questions = TquestionsM;
                    int studetestid = 0;
                    if (TquestionsM.Count > 0)
                    {
                        //if (Qid == 0)
                        //{
                        if (sts.TestId > 0)
                        {
                            studenttest.TestId = sts.TestId;
                            studenttest.Refno = Guid.NewGuid().ToString().Substring(0, 10).ToUpper();
                            studenttest.UserDetailsId = userDetailsId.Count() > 0 ? userDetailsId.FirstOrDefault().Id : 0;
                            studenttest.Status = true;
                            studenttest.Date = _currDateTime;
                            studenttest.Deleted = false;
                            studenttest.CreatedBy = userId;
                            studenttest.CreatedOn = _currDateTime;
                            studenttest.UpdatedBy = userId;
                            studenttest.UpdatedOn = _currDateTime;
                            studenttest.IsMockTest = true;
                            studenttest.OrgId = 1;
                            studenttest.SiteId = 1;
                            studenttest.IsTestChecked = false;
                            studenttest.TestCheckedBy = 0;
                            con.StudentTests.Add(studenttest);
                            con.SaveChanges();
                            studetestid = studenttest.Id;

                            //model = comm.onlineCategoryMock(model.StudentTestId, questionid);
                        }
                        else
                        {
                            model.message = "Questions not found";
                            model.status = false;
                            return null;
                            //model.message="Please Select th data properly...";
                        }

                        model = comm.GetQuestionOptions(total_questions, model.studenttest_id);
                        model.duration = Convert.ToInt32(duration.DurationMinute);
                        int cate = Convert.ToInt32(id);
                        model.studenttest_id = studetestid;
                        model.category_id = cate;
                    }

                    else
                    {
                        model.message = "Questions not found.";
                        model.status = false;
                        return model;
                    }

                    model.message = "Success";
                    model.status = true;
                    return model;
                }
                else
                {
                    model.message = "Not an authenticated user";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.message = "Please try again.";
                model.status = false;
                return model;
            }

        }
        /// <summary>
        /// Total Question in test
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TestQuestionsListModel GetCategoryTypeData(string id)
        {
            var model = new TestQuestionsListModel();
            try
            {
                if (IsValidUser())
                {
                    var userDetailsId = con.UserDetails.Where(r => r.UserId == userId).ToList();
                    var sts = con.ScheduleMockTests.Where(s => s.Id.ToString() == id).FirstOrDefault();
                    int? DurationTypeId = sts.DurationId;
                    var studenttest = new StudentTest();
                    model.category_id = Convert.ToInt32(id);
                    int? quespatternid = sts.QuestionPatternId;
                    var duration = con.DurationTypes.Find(DurationTypeId);
                    var testdetails = con.Tests.Where(r => r.Id == sts.TestId).FirstOrDefault(); // pick LevelId and TopicId
                    var TquestionsM = con.TestQuestionMocks.Where(r => r.LevelId == testdetails.LevelId && r.TopicId == testdetails.TopicId && r.QuestionPatternId == quespatternid).ToList();
                    if (sts.JumbleTypeId != 0 && sts.FromYear != 0 && sts.ToYear != 0)
                    {
                       
                        TquestionsM = con.TestQuestionMocks.Where(r => r.LevelId == testdetails.LevelId && r.TopicId == testdetails.TopicId && r.QuestionPatternId == quespatternid && r.YearId >=sts.FromYear && r.YearId <= sts.ToYear).ToList();

                    }
                    else if (sts.JumbleTypeId != 0 && sts.FromYear == 0 && sts.ToYear == 0)
                    {
                        TquestionsM = con.TestQuestionMocks.Where(r => r.LevelId == testdetails.LevelId && r.TopicId == testdetails.TopicId && r.QuestionPatternId == quespatternid).ToList();
                    }
                    else
                         if (sts.QuestionSetId != 0 && sts.ParticularYearId != 0)
                    {
                       // int yearid = con.PreviousYears.Where(r => r.Year == sts.ParticularYearId).FirstOrDefault().Id;
                        TquestionsM = con.TestQuestionMocks.Where(r => r.LevelId == testdetails.LevelId && r.TopicId == testdetails.TopicId && r.QuestionPatternId == quespatternid && r.YearId == sts.ParticularYearId).ToList();
                    }

                    List<TestQuestionMock> total_questions = TquestionsM;
                    int studetestid = 0;
                    if (TquestionsM.Count > 0)
                    {
                        //if (Qid == 0)
                        //{
                        if (sts.TestId > 0)
                        {
                            studenttest.TestId = sts.TestId;
                            studenttest.Refno = Guid.NewGuid().ToString().Substring(0, 10).ToUpper();
                            studenttest.UserDetailsId = userDetailsId.Count() > 0 ? userDetailsId.FirstOrDefault().Id : 0;
                            studenttest.Status = true;
                            studenttest.Date = _currDateTime;
                            studenttest.Deleted = false;
                            studenttest.CreatedBy = userId;
                            studenttest.CreatedOn = _currDateTime;
                            studenttest.UpdatedBy = userId;
                            studenttest.UpdatedOn = _currDateTime;
                            studenttest.IsMockTest = true;
                            studenttest.OrgId = 1;
                            studenttest.SiteId = 1;
                            studenttest.IsTestChecked = false;
                            studenttest.TestCheckedBy = 0;
                            con.StudentTests.Add(studenttest);
                            con.SaveChanges();
                            studetestid = studenttest.Id;

                            //model = comm.onlineCategoryMock(model.StudentTestId, questionid);
                        }
                        else
                        {
                            model.message = "Questions not found";
                            model.status = false;
                            return null;
                            //model.message="Please Select th data properly...";
                        }
                       
                        model = comm.GetQuestionOptions(total_questions,     model.studenttest_id);
                        model.duration = Convert.ToInt32(duration.DurationMinute);
                        int cate = Convert.ToInt32(id);
                        model.studenttest_id= studetestid;
                        model.category_id = cate;
                    }

                    else
                    {
                        model.message = "Questions not found.";
                        model.status = false;
                        return model;
                    }

                    model.message = "Success";
                    model.status = true;
                    return model;
                }
                else
                {
                    model.message = "Not an authenticated user";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.message = "Please try again.";
                model.status = false;
                return model;
            }

        }

        /// <summary>
        /// Saving the answer of question during the test
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public Responsemodel SaveCategoryMockData(QSubmitedModel mod)
        {
            //ResponseModel model = new ResponseModel();
            Responsemodel model = new Responsemodel();
            try
            {

                if (IsValidUser())
                {
                    int Userid1 = mod.user_id;// int.Parse(Session["TUserId"].ToString());
                    var sts = con.ScheduleMockTests.Where(s => s.Id == mod.category_id).FirstOrDefault();
                    string type = sts != null ? sts.Type : null;
                    int stTestId = mod.studenttestid;// int.Parse(Session["studentTestId"].ToString());
                    int TQuestionId = mod.testquestionid;// int.Parse(Session["QuestionId"].ToString());
                    string sTime = "";
                    string eTime = "";
                    sTime = mod.quesread_time;// Session["STime"].ToString(); //Session["STime"] = DateTime.Now;
                    eTime = DateTime.Now.ToString(); //saveTestAnalytic(0, 1, 1, sTime, eTime);
                    var OVT = mod.ansread_time;// Session["ViewClickTime"];
                    int ansTime = 0;
                    int QReadTime = 0;
                    int OptnReadTime = 0;
                    ansTime = Convert.ToInt32(mod.ansread_time);
                    QReadTime = Convert.ToInt32(mod.quesread_time);
                    OptnReadTime = Convert.ToInt32(mod.ans_submittime);
                    comm.saveTestAnalytic(Userid1, TQuestionId, mod.qmaxmarks, stTestId, ansTime, QReadTime, OptnReadTime);
                    var stTestre = con.StudentTests.Where(r => r.Id == stTestId).ToList();
                    int testid = Convert.ToInt32(stTestre.FirstOrDefault().TestId);
                    //int Questionid = int.Parse(form["Quid"]);
                    var studenttestanswer = new StudentTestAnswer();
                    // TestContext.StudentTestId = int.Parse(Session["studentTestId"].ToString());
                    var Referencno = con.StudentTests.Find(stTestId);
                    string Refno = Referencno != null ? Referencno.Refno : "no refno generated.";
                    var Test = con.Tests.Find(testid).Name ?? "Unknow Test";
                    var i = con.StudentTestAnswers.Where(r => r.StudentTestsId == stTestId && r.TestQuestionId == TQuestionId).ToList();
                    string stestid = stTestId.ToString();
                    string uid = mod.user_id.ToString();
                    string catid = mod.category_id.ToString();
                    if (i.Count() > 0)
                    {
                        {
                            var studenttestanswers = con.StudentTestAnswers.Find(i.FirstOrDefault().Id);
                            studenttestanswers.StudentTestsId = stTestId;
                            studenttestanswers.TestQuestionId = TQuestionId;
                            studenttestanswers.MCQAnswersId = mod.mcq_answerid;
                            studenttestanswers.Answers = "NA";
                            studenttestanswers.IsSkip = false;
                            studenttestanswers.Status = true;
                            studenttestanswers.IsNegative = false;
                            if (type == "Topic")
                            {
                                studenttestanswers.IsTopicWise = true;
                            }
                            else
                            {
                                studenttestanswers.IsTopicWise = false;
                            }
                            studenttestanswers.Deleted = false;
                            studenttestanswer.MarksObtained = 0;
                            studenttestanswers.CreatedBy = mod.user_id;
                            studenttestanswers.CreatedOn = _currDateTime;
                            studenttestanswers.UpdatedBy = mod.user_id;
                            studenttestanswers.UpdatedOn = _currDateTime;
                            studenttestanswers.OrgId = 1;
                            studenttestanswers.SiteId = 1;
                            con.Entry(studenttestanswers).State = EntityState.Modified;
                            // _defContext.Entry(studenttestanswers).State = EntityState.Modified;
                            con.SaveChanges();
                            // TestContext.Qid++;
                        }
                        model.message = "Answer changed";
                        model.status = true;
                        model.refno = Refno;

                        return model;
                    }
                    else
                    {
                        {
                            studenttestanswer.StudentTestsId = stTestId;
                            studenttestanswer.TestQuestionId = TQuestionId;
                            studenttestanswer.MCQAnswersId = mod.mcq_answerid;
                            studenttestanswer.Answers = "NA";
                            studenttestanswer.MarksObtained = 0;
                            studenttestanswer.IsSkip = false;
                            studenttestanswer.Status = true;
                            studenttestanswer.IsNegative = false;
                            if (type == "Topic")
                            {
                                studenttestanswer.IsTopicWise = true;
                            }
                            else
                            {
                                studenttestanswer.IsTopicWise = false;
                            }
                            studenttestanswer.Deleted = false;
                            studenttestanswer.CreatedBy = mod.user_id;
                            studenttestanswer.CreatedOn = _currDateTime;
                            studenttestanswer.UpdatedBy = mod.user_id;
                            studenttestanswer.UpdatedOn = _currDateTime;
                            studenttestanswer.OrgId = 1;
                            studenttestanswer.SiteId = 1;
                            con.StudentTestAnswers.Add(studenttestanswer);
                            con.SaveChanges();
                            // TestContext.Qid++;

                        }
                    }
                    model.message = "Next Question";
                    model.status = true;
                    model.refno = Refno;
                    return model;
                }
                else
                {
                    model.message = "Not an authenticated user.";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.message = "Please try again.";
                model.status = false;
                return model;
            }

        }

        #endregion

        #region Live Test
        public ListMock GetCatLiveData()
        {
            ListMock model = new ListMock();
            try
            {
                if (IsValidUser())
                {
                    
                    var userDetailsId = con.UserDetails.Where(r => r.UserId == userId).ToList();
                    int UserId = userDetailsId.Count() > 0 ? userDetailsId.FirstOrDefault().Id : 0;
                    var dataTestScheduled = con.StudentTestScheduleViews.Where(r => r.Schedule == "Start Now" && r.Deleted == false && r.Status == true && r.UserDetailsId == UserId);
                    // var dataTestScheduled = con.StudentTestScheduleViews.Where(r => r.Schedule == "Time Expired" && r.Deleted == false && r.Status == true && r.UserDetailsId == UserId);
                    int count = 1;
                    model.schedule_mock_test = dataTestScheduled.OrderByDescending(r => r.Id).ToList().Select(x =>
                    {
                        var m = new CategoryMock();
                        comm.PrepareModelScheduleLiveTestTypes(m, x);

                        count = count + 1;
                        m.display_date = m.date.ToString();
                    //m.display_date = m.date.ToString("dd-MM-yyyy");//ToString("dd-MM-yyyy h:mm:ss tt");
                    return m;
                    }).ToList();
                    if (model.schedule_mock_test.Count() > 0)
                    {
                        model.message = "Success";
                    }
                    else
                    {
                        model.message = "Test was not scheduled successfully";
                    }

                    model.status = true;
                    return model;
                }
                else
                {
                    model.message = "Not an authenticated user";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.message = "Please try again";
                model.status = false;
                return model;
            }
        }

        public TestQuestionsListModel GetCategoryLiveData(string id)
        {
            var model = new TestQuestionsListModel();
            try
            {
                if (IsValidUser())
                {
                    var userDetailsId = con.UserDetails.Where(r => r.UserId == userId).ToList();
                    var sts = con.StudentTestSchedules.Where(s => s.Id.ToString() == id).FirstOrDefault();
                    int? DurationTypeId = con.Tests.Where(s => s.Id == sts.TestId).FirstOrDefault().DurationTypeId;
                    var studenttest = new StudentTest();
                    model.category_id = Convert.ToInt32(id);
                    int quespatternid = Convert.ToInt32(sts.QuestionPatternId);
                    var duration = con.DurationTypes.Find(DurationTypeId);
                    var testdetails = con.Tests.Where(r => r.Id == sts.TestId).FirstOrDefault(); // pick LevelId and TopicId
                    //TestContext.questions = _defContext.onlinecategorywisetestmodel.Where(r => r.TestId == sts.TestId && r.QuestionPatternId == sts.QuestionPatternId && r.QuestionStatus == true).ToList();
                    var Tquestions = con.OnlineCategoryWiseTestVws.Where(r => r.TestId == sts.TestId && r.QuestionPatternId == sts.QuestionPatternId && r.QuestionStatus == true).ToList();
                    var test = new TestContexts();
                    var randm = new List<OnlineCategoryWiseTestVw>();
                    test.questions = Tquestions;
                    for (int i = 0; i < Tquestions.Count; i++)
                    {
                        int randomId = comm.RetQID(Tquestions);
                        randm.Add(Tquestions[randomId]);

                    }
                    var questions= randm;
                   //List<TestQuestion> total_questions = (List<TestQuestion>)questions;
                    List < OnlineCategoryWiseTestVw> total_questions = questions;
                    int studetestid = 0;
                    if (Tquestions.Count > 0)
                    {
                        //if (Qid == 0)
                        //{
                        if (sts.TestId > 0)
                        {
                            studenttest.TestId = sts.TestId;
                            studenttest.Refno = Guid.NewGuid().ToString().Substring(0, 10).ToUpper();
                            studenttest.UserDetailsId = userDetailsId.Count() > 0 ? userDetailsId.FirstOrDefault().Id : 0;
                            studenttest.Status = true;
                            studenttest.Date = _currDateTime;
                            studenttest.Deleted = false;
                            studenttest.CreatedBy = userId;
                            studenttest.CreatedOn = _currDateTime;
                            studenttest.UpdatedBy = userId;
                            studenttest.UpdatedOn = _currDateTime;
                            studenttest.IsMockTest = false;
                            studenttest.OrgId = 1;
                            studenttest.SiteId = 1;
                            studenttest.IsTestChecked = false;
                            studenttest.TestCheckedBy = 0;
                            con.StudentTests.Add(studenttest);
                            con.SaveChanges();
                            studetestid = studenttest.Id;

                        }
                        else
                        {
                            model.message = "Questions not found";
                            model.status = false;
                            return null;
                        }
                       
                        model = comm.GetQuestionMCQOptions(total_questions, model.studenttest_id);
                        model.duration = Convert.ToInt32(duration.DurationMinute);
                        int cate = Convert.ToInt32(id);
                        model.studenttest_id = studetestid;
                        model.category_id = cate;
                    }

                    else
                    {
                        model.message = "Questions not found";
                        model.status = false;
                        return model;
                    }

                    model.message = "Success";
                    model.status = true;
                    return model;
                }
                else
                {
                    model.message = "Not an authenticated user";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.message = "Please try again";
                model.status = false;
                return model;
            }

        }

        public Responsemodel SaveCategoryLiveData(QSubmitedModel mod)
        {
            Responsemodel model = new Responsemodel();
            try
            {
                if (IsValidUser())
                {
                    var sts = con.ScheduleMockTests.Where(s => s.Id == mod.category_id).FirstOrDefault();
                    int Userid1 = mod.user_id;
                    int stTestId = mod.studenttestid;
                    int TQuestionId = mod.testquestionid;
                    string sTime = "";
                    string eTime = "";
                    sTime = mod.quesread_time;
                    eTime = DateTime.Now.ToString();
                    var OVT = mod.ansread_time;
                    int ansTime = 0;
                    int QReadTime = 0;
                    int OptnReadTime = 0;
                    ansTime = Convert.ToInt32(mod.ansread_time);
                    QReadTime = Convert.ToInt32(mod.quesread_time);
                    OptnReadTime = Convert.ToInt32(mod.ans_submittime);
                    comm.saveTestAnalytic(Userid1, TQuestionId, mod.qmaxmarks, stTestId, ansTime, QReadTime, OptnReadTime);
                    var stTestre = con.StudentTests.Where(r => r.Id == stTestId).ToList();

                    int testid = Convert.ToInt32(stTestre.FirstOrDefault().TestId);
                    var studenttestanswer = new StudentTestAnswer();
                    var Referencno = con.StudentTests.Find(stTestId);
                    string Refno = Referencno != null ? Referencno.Refno : "no refno generated.";
                    var Test = con.Tests.Find(testid).Name ?? "Unknow Test";
                    var i = con.StudentTestAnswers.Where(r => r.StudentTestsId == stTestId && r.TestQuestionId == TQuestionId).ToList();
                    string stestid = stTestId.ToString();
                    string uid = mod.user_id.ToString();
                    string catid = mod.category_id.ToString();
                    if (i.Count() > 0)
                    {
                        {
                            var studenttestanswers = con.StudentTestAnswers.Find(i.FirstOrDefault().Id);
                            studenttestanswers.StudentTestsId = stTestId;
                            studenttestanswers.TestQuestionId = TQuestionId;
                            studenttestanswers.MCQAnswersId = mod.mcq_answerid;
                            studenttestanswers.Answers = "NA";
                            studenttestanswers.IsSkip = false;
                            studenttestanswers.Status = true;
                            studenttestanswers.MarksObtained = 0;
                            studenttestanswers.IsNegative = false;
                            if (sts.Type == "Topic")
                            {
                                studenttestanswers.IsTopicWise = true;
                            }
                            else
                            {
                                studenttestanswers.IsTopicWise = false;
                            }

                            studenttestanswers.Deleted = false;
                            studenttestanswers.CreatedBy = mod.user_id;
                            studenttestanswers.CreatedOn = _currDateTime;
                            studenttestanswers.UpdatedBy = mod.user_id;
                            studenttestanswers.UpdatedOn = _currDateTime;
                            studenttestanswers.OrgId = 1;
                            studenttestanswers.SiteId = 1;
                            con.Entry(studenttestanswers).State = EntityState.Modified;
                            con.SaveChanges();
                        }
                        model.message = "Answer changed";
                        model.status = true;
                        model.refno = Refno;

                        return model;
                    }
                    else
                    {
                        {
                            studenttestanswer.StudentTestsId = stTestId;
                            studenttestanswer.TestQuestionId = TQuestionId;
                            studenttestanswer.MCQAnswersId = mod.mcq_answerid;
                            studenttestanswer.MarksObtained = 0;
                            studenttestanswer.Answers = "NA";
                            studenttestanswer.IsNegative = false;
                            studenttestanswer.IsSkip = false;
                            studenttestanswer.Status = true;
                            if (sts.Type == "Topic")
                            {
                                studenttestanswer.IsTopicWise = true;
                            }
                            else
                            {
                                studenttestanswer.IsTopicWise = false;
                            }
                            studenttestanswer.Deleted = false;
                            studenttestanswer.CreatedBy = mod.user_id;
                            studenttestanswer.CreatedOn = _currDateTime;
                            studenttestanswer.UpdatedBy = mod.user_id;
                            studenttestanswer.UpdatedOn = _currDateTime;
                            studenttestanswer.OrgId = 1;
                            studenttestanswer.SiteId = 1;
                            con.StudentTestAnswers.Add(studenttestanswer);
                            con.SaveChanges();

                        }
                    }
                    model.message = "Next Question";
                    model.status = true;
                    model.refno = Refno;
                    return model;
                }
                else
                {
                    model.message = "Not an authenticated user";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.message = "Please try again";
                model.status = false;
                return model;
            }

        }


        #endregion


        #region Login User
        public LoginEmailStudentModel LoginEmail(LoginEmailStudentModel log)
        {
            log.message = "User is not authenticated";
            var getemail = con.UserDetails.FirstOrDefault();
            if (log.is_email == true)
            {
                getemail = con.UserDetails.Where(r => r.Email == log.user_data).FirstOrDefault();
                if (getemail != null)
                {
                    log.success = true;
                    if (getemail.IsSetPass == true)
                    {
                        log.is_password = true;
                    }
                    else
                    {
                        Otp = comm.CreateRandomCode(6);
                        comm.SendEMailUser(log.user_data, Otp, getemail.Name);
                        log.otp = Otp;
                    }
                    log.message = "User is authenticated";

                }
            }
            else
            {
                getemail = con.UserDetails.Where(r => r.Mobile == log.user_data).FirstOrDefault();
                if (getemail != null)
                {
                    log.success = true;
                    if (getemail.IsSetPass == true)
                    {
                        log.is_password = true;
                    }
                    else
                    {
                        Otp = comm.CreateRandomCode(6);
                        comm.MobileLoginSms(log.user_data, Otp);
                        log.otp = Otp;
                    }
                    log.message = "User is authenticated";

                }
            }
            return log;
        }

        public async Task<LoginPasswrdStudentModel> LoginPassword(LoginPasswrdStudentModel logpass)
        {
            var getemail = con.AspNetUsers.FirstOrDefault();
            var getdata = con.AspNetUsers.FirstOrDefault();
            if (logpass.is_email == true)
            {
                getdata = con.AspNetUsers.Where(r => r.Email == logpass.email_id).FirstOrDefault();
            }
            else
            {
                getdata = con.AspNetUsers.Where(r => r.PhoneNumber == logpass.email_id).FirstOrDefault();
            }
            if (getdata != null)
            {
                bool result1 = comm.VerifyHashedPassword(getdata.PasswordHash, logpass.password);

                var result = result1;
                if (result == true)
                {
                    if (logpass.is_email == true)
                    {
                        var getuser = con.UserDetails.Where(r => r.Email == logpass.email_id).FirstOrDefault();
                        if (getuser != null)
                        {
                            logpass.success = true;
                            logpass.message = "Successfully logged in";
                            logpass.user_name = getuser.Name;
                            logpass.mobile = getuser.Mobile;
                            logpass.email_id = getuser.Email;
                            logpass.user_id = getuser.UserId;
                        }
                    }
                    else
                    {
                        var getuser = con.UserDetails.Where(r => r.Mobile == logpass.email_id).FirstOrDefault();
                        if (getuser != null)
                        {
                            logpass.success = true;
                            logpass.message = "Successfully logged in";
                            logpass.user_name = getuser.Name;
                            logpass.mobile = getuser.Mobile;
                            logpass.email_id = getuser.Email;
                            logpass.user_id = getuser.UserId;
                        }
                    }
                }
                else
                {
                    logpass.success = false;
                    logpass.message = "Please check the details entered";
                }
            }
            else
            {
                logpass.success = false;
                logpass.message = "Please check the details entered";
            }
            return logpass;
        }

        public LoginOtpStudentModel ResendOtp(LoginOtpStudentModel otps)
        {
            string getotp = comm.CreateRandomCode(6);
            var getrec = con.tbl_SMSConfig.Take(1).First();
            otps.message = "OTP is not generated successfully";


            if (otps.is_email == false)
            {
                var getuser = con.UserDetails.Where(r => r.Mobile == otps.data).FirstOrDefault();
                if (getuser != null)
                {
                    comm.MobileLoginSms(otps.data, getotp);
                    otps.success = true;
                    otps.message = "OTP is successfully sent";
                }
            }
            else
            {
                var getuser = con.UserDetails.Where(r => r.Email == otps.data).FirstOrDefault();
                if (getuser != null)
                {
                   
                    comm.SendEMailUser(otps.data, getotp ,getuser.Name);
                    otps.success = true;
                    otps.message = "OTP is successfully sent";
                }
            }

            return otps;
        }


        public async Task<LoginotpStudentModel> LoginOTP(LoginotpStudentModel logotp)
        {
            if (Otp == logotp.verification_code)
            {
                if (logotp.is_email == true)
                {

                    var getuser = con.UserDetails.Where(r => r.Email == logotp.email_id).FirstOrDefault();
                    if (getuser != null)
                    {
                        logotp.success = true;
                        logotp.message = "Successfully logged in";
                        logotp.user_name = getuser.Name;
                        logotp.mobile = getuser.Mobile;
                        logotp.email_id = getuser.Email;
                        logotp.user_id = getuser.UserId;
                    }
                }
                else
                {
                    var getuser = con.UserDetails.Where(r => r.Mobile == logotp.mobile).FirstOrDefault();
                    if (getuser != null)
                    {
                        logotp.success = true;
                        logotp.message = "Successfully logged in";
                        logotp.user_name = getuser.Name;
                        logotp.mobile = getuser.Mobile;
                        logotp.email_id = getuser.Email;
                        logotp.user_id = getuser.UserId;
                    }
                }
            }
            else
            {

                logotp.success = false;
                logotp.message = "Enter the correct OTP";
            }
            return logotp;
        }
        #endregion

        #region package selection
        public packageModel classlist()
        {
            packageModel model = new packageModel();
            if (IsValidUser())
            {

                var classes = con.ClassTypes.Where(r => r.Deleted == false && r.Status == true).ToList();
                foreach (var i in classes)
                {
                    model.pkgselection.Add(new packageSelectionModel()
                    {
                        class_name = i.Class,
                        id = i.Id,

                    });
                }
                if (classes.Count() > 0)
                {
                    model.message = "Sucessfully retrived the data";
                    model.status = true;

                }
                else
                {
                    model.message = "Failure in fetching information";
                    model.status = false;
                }


            }
            //}

            return model;

        }

        public PackageclassViewModel packagedetails(PackageclassModel class_id)
        {
            PackageclassViewModel mod = new PackageclassViewModel();

         
            mod.message = "Failure in fetching information";
            mod.status = false;
            if (IsValidUser())
            {
                var getpkgid = con.UserDetails.Where(r => r.UserId == userId).FirstOrDefault();
                var getrecords = con.PackageSelections.ToList();
                if (getpkgid != null)
                {
                    if (getpkgid.ClassId != 0 && getpkgid.PackageType != null && getpkgid.PackageType != "")
                    {
                        int pkageId =Convert.ToInt32(getpkgid.PackageType);
                        getrecords = con.PackageSelections.Where(r => r.ClassId == getpkgid.ClassId && r.Deleted == false && r.PackageId== pkageId).ToList();
                        mod.class_id = getpkgid.ClassId;
                        var getclssese = con.ClassTypes.Where(r => r.Id == mod.class_id).FirstOrDefault();
                        mod.class_name = getclssese.Class;
                    }
                    else
                    {
                        getrecords = con.PackageSelections.Where(r => r.ClassId == class_id.class_id && r.Deleted == false).ToList();

                    }
                }
                else
                {
                    getrecords = con.PackageSelections.Where(r => r.ClassId == class_id.class_id && r.Deleted == false).ToList();

                }
                foreach (var mm in getrecords)
                {
                    var getpkg = con.PackagesTypes.Where(r => r.Id == mm.PackageId && r.Deleted == false);
                    var getpkgamt = con.PackageSelections.Where(r => r.PackageId == mm.PackageId &&  r.Deleted == false);
                    int months = (getpkgamt.FirstOrDefault().ValidTill.Year - DateTime.Now.Year) * 12 + getpkgamt.FirstOrDefault().ValidTill.Month - DateTime.Now.Month;
                    int amts = Convert.ToInt32(getpkgamt.FirstOrDefault().Amount);
                    
                    var mod2 = new PackageclassListModel
                    {
                        package_name = getpkg.FirstOrDefault().Name,
                       // amtPer_month = Convert.ToInt32(getpkgamt.FirstOrDefault().Amount).ToString(),
                        valid_till = months.ToString(),
                        total_amount = amts.ToString(),
                        amount_per_month = (amts / 12).ToString(),
                      //  totlamt = (months * amts).ToString(),
                        logo_url = getpkg.FirstOrDefault().ImageUrl.ToString(),
                        back_color = getpkg.FirstOrDefault().BackGroundColor.ToString(),
                        options = getpkg.FirstOrDefault().options.ToString(),
                        id = getpkg.FirstOrDefault().Id,

                    };
                    mod.AvailableClass.Add(mod2);

                }
                var getclss = con.ClassTypes.Where(r => r.Id == class_id.class_id).FirstOrDefault();
                string classname = getclss.Class;
                var getsubjs = con.ClassMasters.Where(p => p.Class == classname).ToList();
                var getrecss = con.UserDetails.Where(r => r.UserId == userId).FirstOrDefault();
                if (getrecss.PackageType != null && getrecss.ClassId != 0)
                {
                    foreach (var itm in getsubjs)
                    {
                        var getdatapkg = con.StudentPackageSelections.Where(r => r.UserId == userId && r.SubjectId == itm.SubjectId).FirstOrDefault();
                        if (getdatapkg == null)
                        {
                            var getsubjname = con.Categories.Where(r => r.Id == itm.SubjectId).FirstOrDefault();
                            var mods = new PackageSubjectViewModel
                            {
                                subject_id = itm.SubjectId,
                                subject_name = getsubjname.CategoryName,
                                is_checked = false
                            };
                            mod.getsubjectlist.Add(mods);
                        }
                        else
                        {
                            var getsubjname = con.Categories.Where(r => r.Id == itm.SubjectId).FirstOrDefault();
                            var mods = new PackageSubjectViewModel
                            {
                                subject_id = itm.SubjectId,
                                subject_name = getsubjname.CategoryName,
                                is_checked = true
                            };
                            mod.getsubjectlist.Add(mods);
                        }
                    }
                   
                    mod.message = "Successfully retrived the data";
                    mod.status = true;
                }
                //if (getrecords.Count() > 0)
                //{
                  
               // }
                //else
                //{
                  
                //}
            }
            return mod;
        }

        public PackageupgradeViewModel GetClassDataUpgrade()
        {
            PackageupgradeViewModel mod = new PackageupgradeViewModel();
            var getmod = new PackageupgradeViewModel();
            if (IsValidUser())
            {

                var getdetails = con.UserDetails.Where(r => r.UserId == userId);
                var getrecords = con.PackageSelections.Where(r => r.ClassId == getdetails.FirstOrDefault().ClassId && r.Deleted == false).ToList();
                int pkgid = Convert.ToInt32(getdetails.FirstOrDefault().PackageType);
                var getprice = con.PackageSelections.Where(r => r.PackageId == pkgid && r.ClassId == getdetails.FirstOrDefault().ClassId);
                //diff
                int pkg = Convert.ToInt32(getdetails.FirstOrDefault().PackageType);
                int? clssid = getdetails.FirstOrDefault().ClassId;
                var getamt = con.PackageSelections.Where(r => r.PackageId == pkg && r.ClassId == clssid);
                int amounts = Convert.ToInt32(getamt.FirstOrDefault().Amount);
                var getdatapkgdetl = con.StudentPackageSelections.Where(r => r.UserId == userId).ToList();
                decimal amt = amounts * getdatapkgdetl.Count();

                foreach (var mm in getrecords)
                {
                    var getpkg = con.PackagesTypes.Where(r => r.Id == mm.PackageId && r.Deleted == false);
                    var getpkgamt = con.PackageSelections.Where(r => r.PackageId == mm.PackageId && r.Deleted == false);
                    int months = (getpkgamt.FirstOrDefault().ValidTill.Year - DateTime.Now.Year) * 12 + getpkgamt.FirstOrDefault().ValidTill.Month - DateTime.Now.Month;
                    int amts = Convert.ToInt32(getpkgamt.FirstOrDefault().Amount);

                    if (mm.Amount > getprice.FirstOrDefault().Amount)
                    {
                        var mod2 = new PackageupgradeListModel
                        {
                            package_name = getpkg.FirstOrDefault().Name,
                           // amtPer_month = Convert.ToInt32(getpkgamt.FirstOrDefault().Amount).ToString(),
                            valid_till = months.ToString(),
                          //  totlamt = (months * amts).ToString(),
                            total_amount = amts.ToString(),
                            amount_per_month = (amts / 12).ToString(),
                            logo_url = getpkg.FirstOrDefault().ImageUrl.ToString(),
                            back_color = getpkg.FirstOrDefault().BackGroundColor.ToString(),
                            options = getpkg.FirstOrDefault().options.ToString(),
                            difference_amount= amt.ToString(),
                            id = getpkg.FirstOrDefault().Id
                        };
                        mod.AvailableupgradeClass.Add(mod2);
                    }
                }
                var getclss = con.ClassTypes.Where(r => r.Id == getdetails.FirstOrDefault().ClassId).FirstOrDefault();
                string classname = getclss.Class;
                var getsubj = con.ClassMasters.Where(p => p.Class == classname).ToList();

                foreach (var itm in getsubj)
                {
                    var getdatapkg = con.StudentPackageSelections.Where(r => r.UserId == userId && r.SubjectId == itm.SubjectId).FirstOrDefault();
                    if (getdatapkg == null)
                    {
                        var getsubjname = con.Categories.Where(r => r.Id == itm.SubjectId).FirstOrDefault();
                        var mods = new PackageSubjectViewModel
                        {
                            subject_id = itm.SubjectId,
                            subject_name = getsubjname.CategoryName,
                            is_checked = false
                        };
                        mod.getsubjectlist.Add(mods);
                    }
                    else
                    {
                        var getsubjname = con.Categories.Where(r => r.Id == itm.SubjectId).FirstOrDefault();
                        var mods = new PackageSubjectViewModel
                        {
                            subject_id = itm.SubjectId,
                            subject_name = getsubjname.CategoryName,
                            is_checked = true
                        };
                        mod.getsubjectlist.Add(mods);
                    }
                    //   mod.Add(mods);
                }
                var getuser = con.UserDetails.Where(r => r.UserId == userId).FirstOrDefault();
                var getclass = con.ClassTypes.Where(r => r.Id == getuser.ClassId).FirstOrDefault();
                if (mod.AvailableupgradeClass.Count() > 0)
                {
                    if (mod.getsubjectlist.Count() > 0)
                    {
                        mod.message = "Successfully retrived the data";
                        mod.status = true;
                        mod.class_id = getclass.Id;
                        mod.class_name = getclass.Class;
                    }
                    else
                    {
                        mod.message = "Failure in fetching data";
                        mod.status = false;
                    }
                }
                else
                {
                    mod.message = "Already upgraded to maximum package";
                    mod.status = true;
                }
                var getclassdetails = con.UserDetails.Where(r => r.UserId == userId).FirstOrDefault();
                var getclassval = con.ClassTypes.Where(r => r.Id == getclassdetails.ClassId);
                return mod;
            }
            else
            {
                return mod;
            }

        }


        public statuspkg getdatadb(PackagegetModel getdata)
        {
            #region update table
            var getstatus = new statuspkg();
            if (IsValidUser())
            {
                    if (getdata != null)
                    {

                        var modeltys = new PackagegetModel();
                        //var getdetails = con.StudentPackageSelections.Where(r => r.UserId == userId).OrderByDescending(r => r.Id).FirstOrDefault();
                        modeltys.BillingPhone = getdata.BillingPhone;
                        modeltys.BillingName = getdata.BillingName;
                        modeltys.BillingEmail = getdata.BillingEmail;
                        modeltys.PackageId = Convert.ToInt32(getdata.PackageId);
                        modeltys.ClassId = Convert.ToInt32(getdata.ClassId);
                        modeltys.Deleted = false;
                        modeltys.Status = true;
                        modeltys.CreatedBy = userId;
                        modeltys.CreatedOn = _currDateTime;
                        modeltys.UpdatedBy = userId;
                        modeltys.UpdatedOn = _currDateTime;
                        modeltys.GetAmount = Convert.ToDecimal(getdata.Amount);
                        modeltys.UserId = userId;
                        modeltys.Mode = getdata.Mode;
                        modeltys.Description = getdata.Description;

                        modeltys.TransactionId = getdata.TransactionId;
                    // var mods = con.StudentPackageSelections.Where(r => r.UserId == userId).ToList();
                    string[] subjects = getdata.subject_list.Split(',');

                    if (getdata.PaymentStatus == "Authorized")
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandText = "sp_InvoiceDetails";
                        cmd.Connection = conn;

                        cmd.Parameters.AddWithValue("@UserId", modeltys.UserId);
                        cmd.Parameters.AddWithValue("@PackageId", modeltys.PackageId);
                        cmd.Parameters.AddWithValue("@Email", modeltys.BillingEmail);
                        cmd.Parameters.AddWithValue("@Name", modeltys.BillingName);
                        cmd.Parameters.AddWithValue("@MobileNumber", modeltys.BillingPhone);
                        cmd.Parameters.AddWithValue("@Amount", modeltys.GetAmount);
                        cmd.Parameters.AddWithValue("@ClassId", modeltys.ClassId);
                        cmd.Parameters.AddWithValue("@Date", modeltys.CreatedOn);
                        cmd.Parameters.AddWithValue("@Status", modeltys.Status);
                        cmd.Parameters.AddWithValue("@Deleted", modeltys.Deleted);
                        cmd.Parameters.AddWithValue("@CreatedBy", modeltys.CreatedBy);
                        cmd.Parameters.AddWithValue("@CreatedOn", modeltys.CreatedOn);
                        cmd.Parameters.AddWithValue("@UPDATED_BY", modeltys.UpdatedBy);
                        cmd.Parameters.AddWithValue("@UpdatedOn", modeltys.UpdatedOn);
                        cmd.Parameters.AddWithValue("@SiteId", modeltys.SiteId);
                        cmd.Parameters.AddWithValue("@OrgId", modeltys.OrgId);
                        cmd.Parameters.AddWithValue("@Mode", modeltys.Mode);
                        cmd.Parameters.AddWithValue("@Description", modeltys.Description);
                        cmd.Parameters.AddWithValue("@IsApp", true);
                        cmd.Parameters.AddWithValue("@ACTION", "INSERT");
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();

                        //insert of userdetails
                        var mod = con.UserDetails.Where(r => r.UserId == userId).FirstOrDefault();
                        mod.PackageType = modeltys.PackageId.ToString();
                        mod.ClassId = modeltys.ClassId;
                        con.Entry(mod).State = EntityState.Modified;
                        con.SaveChanges();


                        //insert student
                        foreach (var itms in subjects)
                        {
                            var model = new StudentPackageSelection();
                            model.MobileNumber = modeltys.BillingPhone;
                            model.Name = modeltys.BillingName;
                            model.Email = modeltys.BillingEmail;
                            model.PackageId = Convert.ToInt32(modeltys.PackageId);
                            model.ClassId = Convert.ToInt32(modeltys.ClassId);
                            model.Deleted = false;
                            model.Status = true;
                            model.CreatedBy = userId;
                            model.CreatedOn = _currDateTime;
                            model.UpdatedBy = userId;
                            model.UpdatedOn = _currDateTime;
                            model.Amount = Convert.ToDecimal(modeltys.GetAmount);
                            model.UserId = userId;
                            model.TxId = getdata.TransactionId;
                            model.Ispaid = true;
                            model.SiteId = 0;
                            model.OrgId = 0;
                            model.SubjectId = Convert.ToInt32(itms);

                            // insert to  student pkg details
                            con.StudentPackageSelections.Add(model);
                            con.SaveChanges();
                        }

                        var getdetails = con.StudentPackageSelections.Where(r => r.UserId == userId).OrderByDescending(r => r.Id).FirstOrDefault();

                        int classtypeid = Convert.ToInt32(getdetails.ClassId);
                        var classtype = con.ClassTypes.Where(r => r.Id == classtypeid).ToList();
                        string clas = classtype.Count() > 0 ? classtype.FirstOrDefault().Class : "";
                        var subjIds = con.StudentPackageSelections.Where(r => r.UserId == userId);
                        //  var subjIds = _defContext.classMasterContext.Where(r => r.Class == clas).ToList();
                        foreach (var sub in subjIds)
                        {
                            var modsub = con.UserDetails.Where(r => r.UserId == userId).FirstOrDefault();
                            int userdetailid = modsub != null ? modsub.Id : 0;
                            var getrec = con.StudentCategoryMappings.Where(r => r.UserDetailsId == userdetailid && r.CategoryId == sub.SubjectId && r.Deleted == false).ToList();
                            if (getrec.Count() == 0)
                            {
                                var m = new StudentCategoryMapping();
                                var modsubs = _defContexts.UserDetails.Where(r => r.UserId == userId).FirstOrDefault();
                                int userdetailids = modsubs != null ? modsubs.Id : 0;
                                m.UserDetailsId = userdetailids;
                                m.CategoryId = sub.SubjectId;
                                m.CreatedBy = userId;
                                m.CreatedOn = _currDateTime;
                                m.UpdatedOn = _currDateTime;
                                m.Status = true;
                                m.Deleted = false;
                                m.SiteId = 1;
                                m.OrgId = 1;
                                m.UpdatedBy = userId;
                                _defContexts.StudentCategoryMappings.Add(m);
                                _defContexts.SaveChanges();
                            }
                        }
                        getstatus.message = "success";
                        getstatus.status = true;
                    }
                    else
                    {
                        foreach (var itms in subjects)
                        {
                            //insert student
                            var model = new StudentPackageSelection();
                            model.MobileNumber = modeltys.BillingPhone;
                            model.Name = modeltys.BillingName;
                            model.Email = modeltys.BillingEmail;
                            model.PackageId = Convert.ToInt32(modeltys.PackageId);
                            model.ClassId = Convert.ToInt32(modeltys.ClassId);
                            model.Deleted = false;
                            model.Status = true;
                            model.CreatedBy = userId;
                            model.CreatedOn = _currDateTime;
                            model.UpdatedBy = userId;
                            model.UpdatedOn = _currDateTime;
                            model.Amount = Convert.ToDecimal(modeltys.GetAmount);
                            model.UserId = userId;
                            model.TxId = getdata.TransactionId;
                            model.Ispaid = false;
                            model.SiteId = 0;
                            model.OrgId = 0;
                            model.SubjectId = Convert.ToInt32(itms);

                            // insert to  student pkg details
                            con.StudentPackageSelections.Add(model);
                            con.SaveChanges();
                        }
                        getstatus.message = "failure";
                        getstatus.status = false;
                    }
                }
                else
                {
                    getstatus.message = "failure";
                    getstatus.status = false;
                }
            }
            else
            {
                getstatus.message = "failure";
                getstatus.status = false;
            }
            return getstatus;
            #endregion
        }

        #endregion
        #region Schedule Live Test

        public ResultScheduleLiveTest QuestionPatterns(string type)
        {
            string Type = type;
            ResultScheduleLiveTest model = new ResultScheduleLiveTest();

            var qpattern = con.QuestionPatterns.ToList();

            try
            {
                if (IsValidUser())
                {
                    foreach (var i in qpattern)
                    {
                        model.scheduleLiveTest.Add(new ScheduleLiveTest()
                        {
                            pattern_name = i.Name,
                            id = i.Id,
                        });
                    }

                    if (qpattern.Count() > 0)
                    {
                        model.message = "Success";
                        model.status = true;
                        return model;
                    }
                    else
                    {
                        model.message = "No Question Patterns found";
                        model.status = false;
                        return model;
                    }
                }
                else
                {
                    model.message = "Not an authenticated user";
                    model.status = false;
                    return model;
                }

            }
            catch (Exception ex)
            {
                model.message = "Not an authenticated user";
                model.status = false;
                return model;
            }

        }
        public ResultScheduleLiveTest GetLevels(ScheduleTestpassIds categoryid)
        {
            string Type = categoryid.type;
            ResultScheduleLiveTest model = new ResultScheduleLiveTest();
            var a = con.LevelMasters.Where(r => r.SubjectId == categoryid.category_id && r.Status == true && r.Deleted == false).ToList();
            try
            {
                if (IsValidUser())
                {
                    foreach (var i in a)
                    {
                        model.scheduleLiveTest.Add(new ScheduleLiveTest()
                        {
                            level_name = i.LevelName,
                            id = i.Id,

                        });
                    }
                    if (a.Count() > 0)
                    {
                        model.message = "Success";
                        model.status = true;
                        return model;
                    }
                    else
                    {
                        model.message = "No Levels found";
                        model.status = false;
                        return model;
                    }
                }
                else
                {

                    model.message = "Not an authenticated user";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.message = "Not an authenticated user";
                model.status = false;
                return model;
            }
        }

        public ResultScheduleLiveTest GetSubjectByLevel(string type)
        {
            string Type = type;
            ResultScheduleLiveTest model = new ResultScheduleLiveTest();
            try
            {
                if (IsValidUser())
                {
                    var userdetails = con.UserDetails.Where(r => r.UserId == userId).FirstOrDefault();
                    int userdetailsId = userdetails != null ? userdetails.Id : 0;
                    var subjectId = con.StudentCategoryMappings.Where(r => r.UserDetailsId == userdetailsId && r.Deleted==false).Select(r => r.CategoryId).ToList();
                    foreach (var subId in subjectId)
                    {
                        var a = con.Categories.Where(r => r.Id == subId && r.Status == true && r.Deleted == false).ToList();
                        foreach (var i in a)
                        {
                            model.scheduleLiveTest.Add(new ScheduleLiveTest()
                            {
                                subject_name = i.CategoryName,
                                id = i.Id,

                            });
                        }
                    }
                    if (subjectId.Count() > 0)
                    {
                        model.message = "Success";
                        model.status = true;
                        return model;
                    }
                    else
                    {
                        model.message = "No Subjects found";
                        model.status = false;
                        return model;
                    }
                }


                else
                {

                    model.message = "Not an authenticated user";
                    model.status = false;
                    return model;
                }

            }
            catch (Exception ex)
            {
                model.message = "Not an authenticated user";
                model.status = false;
                return model;
            }
        }



        public ResultScheduleLiveTest GetTopicByCategoryId(ScheduleTestpassIds categoryid)
        {
            string Type = categoryid.type;
            ResultScheduleLiveTest model = new ResultScheduleLiveTest();
            int Categoryid = categoryid.category_id;
            var a = con.Topics.Where(r => r.CategoryId == Categoryid && r.Status == true && r.Deleted == false).ToList();
            try
            {
                if (IsValidUser())
                {
                    foreach (var i in a)
                    {
                        model.scheduleLiveTest.Add(new ScheduleLiveTest()
                        {
                            topic_name = i.TopicName,
                            id = i.Id,

                        });
                    }

                }
                if (a.Count() > 0)
                {
                    model.message = "Success";
                    model.status = true;
                    return model;
                }

                else
                {

                    model.message = "No Topics found";
                    model.status = false;
                    return model;
                }
            }

            catch (Exception ex)
            {
                model.message = "Not an authenticated user";
                model.status = false;
                return model;
            }
        }

        public ResultScheduleLiveTest GetTestByTopicId(ScheduleTestpassIds topicId)
        {
            string Type = topicId.type;
            ResultScheduleLiveTest model = new ResultScheduleLiveTest();
            int topicid = topicId.topic_id;
            var a = con.Tests.Where(r => r.TopicId == topicid && r.IsTimeBased == false && r.Status == true && r.Deleted == false).ToList();
            try
            {
                if (IsValidUser())
                {
                    foreach (var i in a)
                    {
                        model.scheduleLiveTest.Add(new ScheduleLiveTest()
                        {
                            test_name = i.Name,
                            id = i.Id,

                        });
                    }
                }
                if (a.Count() > 0)
                {
                    model.message = "Success";
                    model.status = true;
                    return model;
                }
                else
                {

                    model.message = "No Tests found";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.message = "Not an authenticated user";
                model.status = false;
                return model;
            }
        }

        public ResultScheduleLiveTest GetDurationByTestId(ScheduleTestpassIds testid)
        {
            int testId = testid.test_id;
            string Type = testid.type;
            var duration = con.Tests.Where(r => r.Id == testId);
            ResultScheduleLiveTest model = new ResultScheduleLiveTest();
            var a = con.DurationTypes.Where(r => r.Id == duration.FirstOrDefault().DurationTypeId);
            try
            {
                if (IsValidUser())
                {
                    foreach (var i in a)
                    {
                        model.scheduleLiveTest.Add(new ScheduleLiveTest()
                        {
                            duration_minute = i.DurationMinute,
                            id = i.Id,

                        });
                    }
                }
                if (a.Count() > 0)
                {
                    model.message = "Success";
                    model.status = true;
                    return model;
                }
                else
                {

                    model.message = "No Durations found";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.message = "Not an authenticated user";
                model.status = false;
                return model;
            }
        }

        public ResultScheduleLiveTest GetTestBycategoryId(ScheduleTestpassIds testbysubid)
        {
            int testId = testbysubid.category_test_id;
            string Type = testbysubid.type;
            ResultScheduleLiveTest model = new ResultScheduleLiveTest();
            var a = con.OnlineCategoryWiseTestVws.Where(r => r.CategoryId == testId).GroupBy(r => r.TestId).Select(g => g.FirstOrDefault()).Distinct().ToList();
            try
            {
                if (IsValidUser())
                {
                    foreach (var i in a)
                    {
                        model.scheduleLiveTest.Add(new ScheduleLiveTest()
                        {
                            test_name = i.Name,
                            id = Convert.ToInt32(i.TestId),

                        });
                    }
                }
                if (a.Count() > 0)
                {
                    model.message = "Success";
                    model.status = true;
                    return model;
                }
                else
                {

                    model.message = "No Tests found";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.message = "Not an authenticated user";
                model.status = false;
                return model;
            }
        }

        public ScheduleLiveTests ScheduleLiveTest(ScheduleTestgetValues testdetails)
        {

            ScheduleLiveTests mod = new ScheduleLiveTests();
            try
            {
                if (IsValidUser())
                {

                    if (testdetails.test_id > 0)
                    {
                        comm.ScheduleLiveTest(testdetails);
                    }
                    mod.message = "Successfully Scheduled";
                    mod.status = true;
                    return mod;
                }
                else
                {
                    mod.message = "Not an authenticated user";
                    mod.status = false;
                    return mod;
                }
            }
            catch (Exception ex)
            {
                mod.message = ex.Message;
                mod.status = false;
                return mod;
            }

        }
        #endregion
        #region Schedule Mock Test
        public TestBytopic GetMockTestByTopicId(int topic_id, string type)
        {
            string Type = type;
            TestBytopic model = new TestBytopic();
            int topicid = topic_id;
            var a = con.Tests.Where(r => r.TopicId == topicid && r.IsTimeBased == true && r.Status == true && r.Deleted == false).ToList();
            try
            {
                if (IsValidUser())
                {
                    foreach (var i in a)
                    {
                        model.topicwisetest.Add(new MockTests()
                        {
                            test_name = i.Name,
                            id = i.Id,

                        });
                    }

                }
                if (a.Count() > 0)
                {
                    model.message = "Success";
                    model.status = true;
                    return model;
                }
                else
                {

                    model.message = "No Tests found";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.message = "Not an authenticated user";
                model.status = false;
                return model;
            }
        }

        public Result QuestionsetTypes(string type)
        {
            string Type = type;
            Result model = new Result();

            try
            {
                if (IsValidUser())
                {
                    var qsets = con.QuestionTypeSets.Where(r => r.Status == true && r.Deleted == false).ToList();
                    foreach (var i in qsets)
                    {
                        model.questionset.Add(new QuestionSet()
                        {
                            question_set_name = i.TypeName,
                            id = i.Id
                        });
                    }
                    if (model != null)
                    {
                        model.message = "Success";
                        model.status = true;
                    }
                    return model;
                }
                else
                {

                    model.message = "No Question Sets found";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.message = "Not an authenticated user";
                model.status = false;
                return model;
            }
        }

        public YearResult AvailableYears(string type)
        {
            string Type = type;
            YearResult model = new YearResult();

            try
            {
                if (IsValidUser())
                {
                    var qsets = con.PreviousYears.Where(r => r.Status == true && r.Deleted == false).ToList();
                    foreach (var i in qsets)
                    {
                        model.Year.Add(new Year()
                        {
                            year = i.Year,
                            id = i.Id
                        });
                    }
                    if (model != null)
                    {
                        model.message = "Success";
                        model.status = true;
                    }
                    return model;
                }
                else
                {

                    model.message = "No Years found";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.message = "Not an authenticated user";
                model.status = false;
                return model;
            }


        }

        public JumbleResult JumbleTypes(string type)
        {
            string Type = type;
            JumbleResult model = new JumbleResult();

            try
            {
                if (IsValidUser())
                {
                    var qsets = con.JumbleTypes.Where(r => r.Status == true && r.Deleted == false).ToList();
                    foreach (var i in qsets)
                    {
                        model.jumbleType.Add(new JumbleSet()
                        {
                            type_name = i.TypeName,
                            id = i.Id
                        });
                    }
                    if (model != null)
                    {
                        model.message = "Success";
                        model.status = true;
                    }
                    return model;
                }
                else
                {

                    model.message = "No Jumble Types found";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.message = "Not an authenticated user";
                model.status = false;
                return model;
            }

        }

        public TestByCategory GetMockTestByCategoryId(int category_id, string type)
        {
            TestByCategory model = new TestByCategory();
            try
            {
                if (IsValidUser())
                {
                    var topicIdsBycatgry = con.Topics.Where(r => r.CategoryId == category_id && r.Status == true && r.Deleted == false).Select(r => r.Id).ToList();
                    foreach (var tid in topicIdsBycatgry)
                    {
                        var a = con.Tests.Where(r => r.TopicId == tid && r.IsTimeBased == true && r.Status == true && r.Deleted == false).ToList();
                        // IsTimeBased==true  (For Mock test only)

                        foreach (var i in a)
                            model.subjectwisetest.Add(new MockCategorywiseTests()
                            {
                                test_name = i.Name,
                                id = i.Id,
                            });

                    }

                    model.message = "Success";
                    model.status = true;
                    return model;
                }


                else
                {
                    model.message = "No Tests found";
                    model.status = false;
                    return model;
                }

            }
            catch (Exception ex)
            {
                model.message = "Not an authenticated user";
                model.status = false;
                return model;
            }

        }

        public DurationResult GetMockDurations(string type)
        {
            DurationResult model = new DurationResult();
            var a = con.DurationTypes.Where(r => r.Status == true && r.Deleted == false);
            try
            {
                if (IsValidUser())
                {
                    foreach (var i in a)
                    {
                        model.durations.Add(new Duration()
                        {
                            duration = i.DurationMinute,
                            id = i.Id,

                        });
                    }
                    if (a.Count() > 0)
                    {
                        model.message = "Success";
                        model.status = true;
                        return model;
                    }
                    else
                    {
                        model.message = "No Durations found";
                        model.status = false;
                        return model;
                    }

                }
                else
                {
                    model.message = "Not an authenticated user";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.message = "Not an authenticated user";
                model.status = false;
                return model;
            }
        }

        public ScheduleMockTestModel ScheduleMockTest(ScheduleMockTestPassId MockValues)
        {
            ScheduleMockTestModel mod = new ScheduleMockTestModel();

            var studpackage = con.StudentPackageSelections.Where(r => r.UserId == MockValues.user_id).ToList();
            var TotalTests = con.ScheduleMockTests.Where(r => r.UserDetailsId == MockValues.user_id && r.Deleted == false && r.IsTrialTest == true).ToList();
            try
            {
                if (IsValidUser())
                {

                    if (MockValues.test_id > 0)
                    {
                        if (studpackage.Count() == 0 && TotalTests.Count() < 1)
                        {
                            comm.ScheduleTrialMockTest(MockValues);
                            mod.message = "Trial test scheduled successfully";
                            mod.status = true;
                            return mod;
                        }
                        else if (studpackage.Count() != 0)
                        {
                            comm.ScheduleMockTest(MockValues);
                            mod.message = "Scheduled successfully";
                            mod.status = true;
                            return mod;
                        }
                        else
                        {
                            mod.message = "Please buy the package to continue or Re-schedule test!";
                            mod.status = false;
                            return mod;
                        }
                    }

                }
                mod.message = "Not an authenticated user";
                mod.status = false;
                return mod;
            }
            catch (Exception ex)
            {
                mod.message = ex.Message;
                mod.status = false;
                return mod;
            }

        }


        #endregion
        #region Schedule Mock TYP Test


        public ScheduleMockTestModel ScheduleMockTYPTest(ScheduleMockTYPTest MockValues)
        {
            var tests = con.Tests.Where(r => r.Id == MockValues.test_id);
            int? topicid = tests.FirstOrDefault().TopicId;
            //int yearid = con.PreviousYears.Where(r => r.Year == MockValues.year).FirstOrDefault().Id;
            var MockQuestions = con.TestQuestionMocks.Where(r => r.TopicId ==topicid && r.YearId == MockValues.year).ToList();

            ScheduleMockTestModel mod = new ScheduleMockTestModel();
            try
            {
                if (IsValidUser())
                {

                    if (MockValues.test_id > 0 && MockQuestions.Count > 0)
                    {
                        comm.scheduleMockTYPtest(MockValues);

                        mod.message = "Successfully Scheduled";
                        mod.status = true;
                        return mod;
                    }
                    else
                    {
                        mod.message = "No Questions";
                        mod.status = false;
                        return mod;
                    }
                }
                else
                {
                    mod.message = "Not an authenticated user";
                    mod.status = false;
                    return mod;
                }
            }
            catch (Exception ex)
            {
                mod.message = ex.Message;
                mod.status = false;
                return mod;
            }

        }
        #endregion
        #region Student View Result

        string isnegative = "";
        public void GetViewResult(int StudentTestId)
        {
            var model = new TestEvaluationModel();
            try
            {
                    int StudeTestId = StudentTestId;                   //int.Parse(Session["StudTestId"].ToString());
                    var querystudenttestanswermodelContext = con.StudentTestAnswers.Where(r => r.StudentTestsId == StudeTestId && r.MCQAnswersId != 0 && r.Deleted == false).ToList();                //.studenttestanswermodelContext;
                    var queryquestionPatternContext = con.QuestionPatterns;
                    var querystudenttest = con.StudentTests.Find(StudeTestId);
                    string refNo = querystudenttest.Refno;
                    //int testid = querystudenttest.TestId;
                    //int tpic = _defContext.testContext.Find(testid).TopicId;
                    //TempData["topicId"] = tpic;
                    // var answerevaluation = querystudenttestanswermodelContext.Where(r => r.StudentTestsId == StudeTestId && r.Deleted == false).ToList();              
                    var querytestanswersevalution = con.TestAnswersEvaluations;
                    model.TotalAttempt = querystudenttestanswermodelContext.Count();
                    foreach (var i in querystudenttestanswermodelContext)
                    {
                        var mcqans = "";
                        var hintQ = "";
                        var writtenSol = "";
                        var videoLinkSol = "";
                        var modelAns = "";
                        var studtest = con.StudentTests.Where(r => r.Id == StudeTestId).ToList();

                        if (studtest.FirstOrDefault().IsMockTest == true)
                        {
                            #region Mock Test
                            var mcqansmock = con.MCQAnswersMocks.Where(r => r.Id == i.MCQAnswersId).ToList();
                            mcqans = mcqansmock.FirstOrDefault().Answer;

                            /// New - solution, Hint
                            var Qsolution = con.TestSolutions.Where(r => r.TestQuestionsId == i.TestQuestionId && r.IsMock == true).ToList();
                            hintQ = Qsolution.FirstOrDefault().Hint;
                            writtenSol = Qsolution.FirstOrDefault().WrittenSolution;
                            videoLinkSol = Qsolution.FirstOrDefault().VideoSolution;
                            modelAns = Qsolution.FirstOrDefault().ModelAnswer;

                            var querymcqanswerContext = con.MCQAnswersMocks;
                            var iscorrectanswered = querymcqanswerContext.Where(r => r.TestQuestionMockId == i.TestQuestionId && r.IsCorrect == true).ToList();
                            bool check = iscorrectanswered.Count() > 0 ? iscorrectanswered.FirstOrDefault().IsCorrect : false;
                            var iscorrectevaluation = querymcqanswerContext.Where(r => r.TestQuestionMockId == i.TestQuestionId && r.Id == i.MCQAnswersId && r.IsCorrect == check).ToList();
                            var iscorrect = iscorrectevaluation.Count() > 0 ? iscorrectevaluation.FirstOrDefault().IsCorrect : false;
                            var questionpattern = querymcqanswerContext.Where(r => r.TestQuestionMockId == i.TestQuestionId).ToList();
                            var question = con.TestQuestionMocks.Find(i.TestQuestionId).Question;
                            var querytestQuestionContext = con.TestQuestionMocks;
                            var studentusername = con.UserDetails.Where(r => r.UserId == i.CreatedBy);
                            int userDetailid = studentusername.FirstOrDefault().Id;
                            var answer = querystudenttestanswermodelContext.Where(r => r.StudentTestsId == i.StudentTestsId && r.TestQuestionId == i.TestQuestionId && r.Answers != "NA").ToList();
                            var negativeevaluation = querytestQuestionContext.Where(r => r.Id == i.TestQuestionId && r.NegativeMarks > 0).ToList();
                            var negativemark = negativeevaluation.Count() > 0 ? negativeevaluation.FirstOrDefault().NegativeMarks : 0;
                            var ss = querytestQuestionContext.Find(i.TestQuestionId).QuestionPatternId;
                            var name = queryquestionPatternContext.Find(ss).Name;
                            var maxmarks = querytestQuestionContext.Find(i.TestQuestionId);
                            model.StudentTestUsername = studentusername.Count() > 0 ? studentusername.FirstOrDefault().Name + "-[" + studentusername.FirstOrDefault().Email + "]" : "UnknownName";
                            model.StudentTestUserId = studentusername.Count() > 0 ? studentusername.FirstOrDefault().UserId : 0;
                            var studenttestanswersevaluation = querytestanswersevalution.Where(r => r.StudentTestAnswerId == i.Id);
                            if (questionpattern.Count() > 0 && negativeevaluation.Count() > 0 && negativeevaluation.FirstOrDefault().NegativeMarks > 0 && iscorrect == false)
                            {
                                string aa = "true";
                                isnegative = aa;
                                //ViewBag.objective = name;
                            }
                            else
                            {
                                string aa = "false";
                                isnegative = aa;
                                //ViewBag.subjective = name;
                            }
                            //  var IstestChecked = studenttestanswersevaluation.Count() > 0 ? true : false;
                            //  ViewBag.IstestChecked = IstestChecked;
                            model.AvailableAnswerEvaluation.Add(new TestEvaluationModel
                            {
                                Refno = refNo,
                                TestQuestionId = i.TestQuestionId,
                                //TestId = testid,
                                StudentTestsId = i.StudentTestsId,
                                StudentTestAnswerId = i.Id,
                                // QuestionPatternName = i.QuestionPatternName,
                                Question = question,              // _defContext.testQuestionContext.Find(i.TestQuestionId).Question,
                                MCQAnswersId = i.MCQAnswersId,
                                Answers = mcqans,                              //answer.Count() > 0 ? answer.FirstOrDefault().Answers : "",               //_defContext.mcqanswerContext.Find(studentanseweredid.FirstOrDefault().Id).Answer,
                                IsCorrectMCQAnswerId = iscorrectanswered.Count() > 0 ? iscorrectanswered.FirstOrDefault().Id : 0,
                                IsCorrectMCQAnswer = iscorrectanswered.Count() > 0 ? iscorrectanswered.FirstOrDefault().Answer : "",
                                IsCorrect = studenttestanswersevaluation.Count() > 0 ? studenttestanswersevaluation.FirstOrDefault().IsCorrect : iscorrect,
                                //IsTestChecked = IstestChecked,
                                IsNegative = bool.Parse(isnegative.ToString()),
                                //NegativeMarks = studenttestanswersevaluation.Count() > 0 ? studenttestanswersevaluation.FirstOrDefault().NegativeMarks : negativemark,
                                NegativeMarks = negativemark,
                                MaxMarks = maxmarks != null ? maxmarks.MaxMarks : 0,
                                MarksObtained = studenttestanswersevaluation.Count() > 0 ? studenttestanswersevaluation.FirstOrDefault().MarksObtain : 0,
                                StudentTestUserId = userDetailid,
                                HintQuestion = hintQ,
                                ModelAnswer = modelAns,
                                WrittenSol = writtenSol,
                                VideoLinkSol = videoLinkSol
                            });
                            #endregion
                        }
                        else
                        {
                            var mcqansmock = con.MCQAnswers.Where(r => r.Id == i.MCQAnswersId).ToList();
                            mcqans = mcqansmock.FirstOrDefault().Answer;
                            /// New - solution, Hint
                            var Qsolution = con.TestSolutions.Where(r => r.TestQuestionsId == i.TestQuestionId && r.IsMock == false).ToList();
                            hintQ = Qsolution.FirstOrDefault().Hint;
                            writtenSol = Qsolution.FirstOrDefault().WrittenSolution;
                            videoLinkSol = Qsolution.FirstOrDefault().VideoSolution;
                            modelAns = Qsolution.FirstOrDefault().ModelAnswer;
                            var querymcqanswerContext = con.MCQAnswers;
                            var iscorrectanswered = querymcqanswerContext.Where(r => r.TestQuestionId == i.TestQuestionId && r.IsCorrect == true).ToList();
                            bool check = iscorrectanswered.Count() > 0 ? iscorrectanswered.FirstOrDefault().IsCorrect : false;
                            var iscorrectevaluation = querymcqanswerContext.Where(r => r.TestQuestionId == i.TestQuestionId && r.Id == i.MCQAnswersId && r.IsCorrect == check).ToList();
                            var iscorrect = iscorrectevaluation.Count() > 0 ? iscorrectevaluation.FirstOrDefault().IsCorrect : false;
                            var questionpattern = querymcqanswerContext.Where(r => r.TestQuestionId == i.TestQuestionId).ToList();
                            var question = con.TestQuestions.Find(i.TestQuestionId).Question;
                            var querytestQuestionContext = con.TestQuestions;
                            var studentusername = con.UserDetails.Where(r => r.UserId == i.CreatedBy);
                            int userDetailid = studentusername.FirstOrDefault().Id;
                            var answer = querystudenttestanswermodelContext.Where(r => r.StudentTestsId == i.StudentTestsId && r.TestQuestionId == i.TestQuestionId && r.Answers != "NA").ToList();
                            var negativeevaluation = querytestQuestionContext.Where(r => r.Id == i.TestQuestionId && r.NegativeMarks > 0).ToList();
                            var negativemark = negativeevaluation.Count() > 0 ? negativeevaluation.FirstOrDefault().NegativeMarks : 0;
                            var ss = querytestQuestionContext.Find(i.TestQuestionId).QuestionPatternId;
                            var name = queryquestionPatternContext.Find(ss).Name;
                            var maxmarks = querytestQuestionContext.Find(i.TestQuestionId);
                            model.StudentTestUsername = studentusername.Count() > 0 ? studentusername.FirstOrDefault().Name + "-[" + studentusername.FirstOrDefault().Email + "]" : "UnknownName";
                            model.StudentTestUserId = studentusername.Count() > 0 ? studentusername.FirstOrDefault().UserId : 0;
                            var studenttestanswersevaluation = querytestanswersevalution.Where(r => r.StudentTestAnswerId == i.Id);
                            if (questionpattern.Count() > 0 && negativeevaluation.Count() > 0 && negativeevaluation.FirstOrDefault().NegativeMarks > 0 && iscorrect == false)
                            {
                                string aa = "true";
                                isnegative = aa;
                                //ViewBag.objective = name;
                            }
                            else
                            {
                                string aa = "false";
                                isnegative = aa;
                                //ViewBag.subjective = name;
                            }

                            model.AvailableAnswerEvaluation.Add(new TestEvaluationModel
                            {
                                Refno = refNo,
                                TestQuestionId = i.TestQuestionId,
                                //TestId = testid,
                                StudentTestsId = i.StudentTestsId,
                                StudentTestAnswerId = i.Id,
                                // QuestionPatternName = i.QuestionPatternName,
                                Question = question,
                                MCQAnswersId = i.MCQAnswersId,
                                Answers = mcqans,
                                IsCorrectMCQAnswerId = iscorrectanswered.Count() > 0 ? iscorrectanswered.FirstOrDefault().Id : 0,
                                IsCorrectMCQAnswer = iscorrectanswered.Count() > 0 ? iscorrectanswered.FirstOrDefault().Answer : "",
                                IsCorrect = studenttestanswersevaluation.Count() > 0 ? studenttestanswersevaluation.FirstOrDefault().IsCorrect : iscorrect,
                                IsNegative = bool.Parse(isnegative.ToString()),
                                NegativeMarks = negativemark,
                                MaxMarks = maxmarks != null ? maxmarks.MaxMarks : 0,
                                MarksObtained = studenttestanswersevaluation.Count() > 0 ? studenttestanswersevaluation.FirstOrDefault().MarksObtain : 0,
                                StudentTestUserId = userDetailid,
                                HintQuestion = hintQ,
                                ModelAnswer = modelAns,
                                WrittenSol = writtenSol,
                                VideoLinkSol = videoLinkSol

                            });
                        }
                    }
                    comm.ViewResultPost(model);
                    //model.message = "Success";
                    //model.status = true;                
            }
            catch (Exception ex)
            {
                model.message = "Failed";
                model.status = false;               
            }      
        }      
        public TestEvaluationModel StudentResultView(int StudentTestId)
        {

            var model = new TestEvaluationModel();
            try
            {
                if (IsValidUser())

                {
                    int StudeTestId = StudentTestId;                   //int.Parse(Session["StudTestId"].ToString());
                                                                       //TestEvaluationModel();
                    var querystudenttestanswermodelContext = con.StudentTestAnswers.Where(r => r.StudentTestsId == StudeTestId && r.MCQAnswersId != 0 && r.Deleted == false).ToList();                //.studenttestanswermodelContext;
                                                                                                                                                                                                      //--   var querytestQuestionContext = _defContext.testQuestionContext;                                                                                                                                                                                                  // var querymcqanswerContext = _defContext.mcqanswerContext;
                    var queryquestionPatternContext = con.QuestionPatterns;
                    var querystudenttest = con.StudentTests.Find(StudeTestId);
                    string refNo = querystudenttest.Refno;
                    //int testid = querystudenttest.TestId;
                    //int tpic = _defContext.testContext.Find(testid).TopicId;
                    //TempData["topicId"] = tpic;
                    // var answerevaluation = querystudenttestanswermodelContext.Where(r => r.StudentTestsId == StudeTestId && r.Deleted == false).ToList();              
                    var querytestanswersevalution = con.TestAnswersEvaluations;
                    model.TotalAttempt = querystudenttestanswermodelContext.Count();


                    foreach (var i in querystudenttestanswermodelContext)
                    {

                        var mcqans = "";
                        var hintQ = "";
                        var writtenSol = "";
                        var videoLinkSol = "";
                        var modelAns = "";
                        var studtest = con.StudentTests.Where(r => r.Id == StudeTestId).ToList();

                        if (studtest.FirstOrDefault().IsMockTest == true)
                        {
                            #region Mock Test
                            var mcqansmock = con.MCQAnswersMocks.Where(r => r.Id == i.MCQAnswersId).ToList();
                            mcqans = mcqansmock.FirstOrDefault().Answer;

                            /// New - solution, Hint
                            var Qsolution = con.TestSolutions.Where(r => r.TestQuestionsId == i.TestQuestionId && r.IsMock == true).ToList();
                            hintQ = Qsolution.FirstOrDefault().Hint;
                            writtenSol = Qsolution.FirstOrDefault().WrittenSolution;
                            videoLinkSol = Qsolution.FirstOrDefault().VideoSolution;
                            modelAns = Qsolution.FirstOrDefault().ModelAnswer;

                            var querymcqanswerContext = con.MCQAnswersMocks;
                            var iscorrectanswered = querymcqanswerContext.Where(r => r.TestQuestionMockId == i.TestQuestionId && r.IsCorrect == true).ToList();
                            bool check = iscorrectanswered.Count() > 0 ? iscorrectanswered.FirstOrDefault().IsCorrect : false;
                            var iscorrectevaluation = querymcqanswerContext.Where(r => r.TestQuestionMockId == i.TestQuestionId && r.Id == i.MCQAnswersId && r.IsCorrect == check).ToList();
                            var iscorrect = iscorrectevaluation.Count() > 0 ? iscorrectevaluation.FirstOrDefault().IsCorrect : false;
                            var questionpattern = querymcqanswerContext.Where(r => r.TestQuestionMockId == i.TestQuestionId).ToList();

                            var question = con.TestQuestionMocks.Find(i.TestQuestionId).Question;


                            var querytestQuestionContext = con.TestQuestionMocks;

                            var studentusername = con.UserDetails.Where(r => r.UserId == i.CreatedBy);
                            int userDetailid = studentusername.FirstOrDefault().Id;
                            var answer = querystudenttestanswermodelContext.Where(r => r.StudentTestsId == i.StudentTestsId && r.TestQuestionId == i.TestQuestionId && r.Answers != "NA").ToList();
                            var negativeevaluation = querytestQuestionContext.Where(r => r.Id == i.TestQuestionId && r.NegativeMarks > 0).ToList();
                            var negativemark = negativeevaluation.Count() > 0 ? negativeevaluation.FirstOrDefault().NegativeMarks : 0;
                            var ss = querytestQuestionContext.Find(i.TestQuestionId).QuestionPatternId;
                            var name = queryquestionPatternContext.Find(ss).Name;
                            var maxmarks = querytestQuestionContext.Find(i.TestQuestionId);
                            model.StudentTestUsername = studentusername.Count() > 0 ? studentusername.FirstOrDefault().Name + "-[" + studentusername.FirstOrDefault().Email + "]" : "UnknownName";
                            model.StudentTestUserId = studentusername.Count() > 0 ? studentusername.FirstOrDefault().UserId : 0;
                            var studenttestanswersevaluation = querytestanswersevalution.Where(r => r.StudentTestAnswerId == i.Id);


                            if (questionpattern.Count() > 0 && negativeevaluation.Count() > 0 && negativeevaluation.FirstOrDefault().NegativeMarks > 0 && iscorrect == false)
                            {
                                string aa = "true";
                                isnegative = aa;
                                //ViewBag.objective = name;
                            }
                            else
                            {
                                string aa = "false";
                                isnegative = aa;
                                //ViewBag.subjective = name;
                            }
                            //  var IstestChecked = studenttestanswersevaluation.Count() > 0 ? true : false;
                            //  ViewBag.IstestChecked = IstestChecked;
                            model.AvailableAnswerEvaluation.Add(new TestEvaluationModel
                            {
                                Refno = refNo,
                                TestQuestionId = i.TestQuestionId,
                                //TestId = testid,
                                StudentTestsId = i.StudentTestsId,
                                StudentTestAnswerId = i.Id,
                                // QuestionPatternName = i.QuestionPatternName,
                                Question = question,              // _defContext.testQuestionContext.Find(i.TestQuestionId).Question,
                                MCQAnswersId = i.MCQAnswersId,
                                Answers = mcqans,                              //answer.Count() > 0 ? answer.FirstOrDefault().Answers : "",               //_defContext.mcqanswerContext.Find(studentanseweredid.FirstOrDefault().Id).Answer,
                                IsCorrectMCQAnswerId = iscorrectanswered.Count() > 0 ? iscorrectanswered.FirstOrDefault().Id : 0,
                                IsCorrectMCQAnswer = iscorrectanswered.Count() > 0 ? iscorrectanswered.FirstOrDefault().Answer : "",
                                IsCorrect = studenttestanswersevaluation.Count() > 0 ? studenttestanswersevaluation.FirstOrDefault().IsCorrect : iscorrect,
                                //IsTestChecked = IstestChecked,
                                IsNegative = bool.Parse(isnegative.ToString()),
                                //NegativeMarks = studenttestanswersevaluation.Count() > 0 ? studenttestanswersevaluation.FirstOrDefault().NegativeMarks : negativemark,
                                NegativeMarks = negativemark,
                                MaxMarks = maxmarks != null ? maxmarks.MaxMarks : 0,
                                MarksObtained = studenttestanswersevaluation.Count() > 0 ? studenttestanswersevaluation.FirstOrDefault().MarksObtain : 0,
                                StudentTestUserId = userDetailid,
                                HintQuestion = hintQ,
                                ModelAnswer = modelAns,
                                WrittenSol = writtenSol,
                                VideoLinkSol = videoLinkSol

                            });
                            #endregion

                        }
                        else
                        {
                            var mcqansmock = con.MCQAnswers.Where(r => r.Id == i.MCQAnswersId).ToList();
                            mcqans = mcqansmock.FirstOrDefault().Answer;

                            /// New - solution, Hint
                            var Qsolution = con.TestSolutions.Where(r => r.TestQuestionsId == i.TestQuestionId && r.IsMock == false).ToList();
                            hintQ = Qsolution.FirstOrDefault().Hint;
                            writtenSol = Qsolution.FirstOrDefault().WrittenSolution;
                            videoLinkSol = Qsolution.FirstOrDefault().VideoSolution;
                            modelAns = Qsolution.FirstOrDefault().ModelAnswer;
                            var querymcqanswerContext = con.MCQAnswers;
                            var iscorrectanswered = querymcqanswerContext.Where(r => r.TestQuestionId == i.TestQuestionId && r.IsCorrect == true).ToList();
                            bool check = iscorrectanswered.Count() > 0 ? iscorrectanswered.FirstOrDefault().IsCorrect : false;
                            var iscorrectevaluation = querymcqanswerContext.Where(r => r.TestQuestionId == i.TestQuestionId && r.Id == i.MCQAnswersId && r.IsCorrect == check).ToList();
                            var iscorrect = iscorrectevaluation.Count() > 0 ? iscorrectevaluation.FirstOrDefault().IsCorrect : false;
                            var questionpattern = querymcqanswerContext.Where(r => r.TestQuestionId == i.TestQuestionId).ToList();
                            var question = con.TestQuestions.Find(i.TestQuestionId).Question;

                            var querytestQuestionContext = con.TestQuestions;

                            var studentusername = con.UserDetails.Where(r => r.UserId == i.CreatedBy);
                            int userDetailid = studentusername.FirstOrDefault().Id;
                            var answer = querystudenttestanswermodelContext.Where(r => r.StudentTestsId == i.StudentTestsId && r.TestQuestionId == i.TestQuestionId && r.Answers != "NA").ToList();
                            var negativeevaluation = querytestQuestionContext.Where(r => r.Id == i.TestQuestionId && r.NegativeMarks > 0).ToList();
                            var negativemark = negativeevaluation.Count() > 0 ? negativeevaluation.FirstOrDefault().NegativeMarks : 0;
                            var ss = querytestQuestionContext.Find(i.TestQuestionId).QuestionPatternId;
                            var name = queryquestionPatternContext.Find(ss).Name;
                            var maxmarks = querytestQuestionContext.Find(i.TestQuestionId);
                            model.StudentTestUsername = studentusername.Count() > 0 ? studentusername.FirstOrDefault().Name + "-[" + studentusername.FirstOrDefault().Email + "]" : "UnknownName";
                            model.StudentTestUserId = studentusername.Count() > 0 ? studentusername.FirstOrDefault().UserId : 0;
                            var studenttestanswersevaluation = querytestanswersevalution.Where(r => r.StudentTestAnswerId == i.Id);


                            if (questionpattern.Count() > 0 && negativeevaluation.Count() > 0 && negativeevaluation.FirstOrDefault().NegativeMarks > 0 && iscorrect == false)
                            {
                                string aa = "true";
                                isnegative = aa;
                                //ViewBag.objective = name;
                            }
                            else
                            {
                                string aa = "false";
                                isnegative = aa;
                                //ViewBag.subjective = name;
                            }

                            model.AvailableAnswerEvaluation.Add(new TestEvaluationModel
                            {
                                Refno = refNo,
                                TestQuestionId = i.TestQuestionId,
                                //TestId = testid,
                                StudentTestsId = i.StudentTestsId,
                                StudentTestAnswerId = i.Id,
                                // QuestionPatternName = i.QuestionPatternName,
                                Question = question,
                                MCQAnswersId = i.MCQAnswersId,
                                Answers = mcqans,
                                IsCorrectMCQAnswerId = iscorrectanswered.Count() > 0 ? iscorrectanswered.FirstOrDefault().Id : 0,
                                IsCorrectMCQAnswer = iscorrectanswered.Count() > 0 ? iscorrectanswered.FirstOrDefault().Answer : "",
                                IsCorrect = studenttestanswersevaluation.Count() > 0 ? studenttestanswersevaluation.FirstOrDefault().IsCorrect : iscorrect,
                                IsNegative = bool.Parse(isnegative.ToString()),
                                NegativeMarks = negativemark,
                                MaxMarks = maxmarks != null ? maxmarks.MaxMarks : 0,
                                MarksObtained = studenttestanswersevaluation.Count() > 0 ? studenttestanswersevaluation.FirstOrDefault().MarksObtain : 0,
                                StudentTestUserId = userDetailid,
                                HintQuestion = hintQ,
                                ModelAnswer = modelAns,
                                WrittenSol = writtenSol,
                                VideoLinkSol = videoLinkSol

                            });
                        }
                    }
                    comm.ViewResultPost(model);
                    model.message = "Success";
                    model.status = true;
                    return model;
                }
                else
                {
                    model.message = "No Authentication User";
                    model.status = false;
                    return model;
                }
            }
            catch(Exception ex)
            {
                model.message = "Failed";
                model.status = false;
                return model;
            }

            return model;
        }
        #endregion
    }
}
    

