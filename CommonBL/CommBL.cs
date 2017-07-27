using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using tys361.WCF.Model;
using tys361.WCF.SMS;
using tys361.WCF.ViewModel;
//using TchrArea = eLearning.Areas.Teacher.ViewModels;

namespace tys361.WCF.CommonBL
{
    public class CommBL
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConn"].ConnectionString);
        DBContextEntities con = new DBContextEntities();
        string SessFrm;
        string SessTo;
        string veryCode;
        public string STime; public string ETime; public int StestId = 0; public int QuestionId = 0;
        #region Register User
        public string CreateRandomCode(int codeCount)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;

            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(10);
                if (temp != -1 && temp == t)
                {
                    return CreateRandomCode(codeCount);
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }
        public void MobileSms(string mobileNumber)
        {
            string verifyCode = veryCode;
            if (verifyCode == null)
            {
                //verifyCode = veryCode;
            }
            try
            {
                String sendToPhoneNumber = mobileNumber;
                ISMSSender sms = new SMSSender();
                //     MobileSMS sms = new MobileSMS();

                tbl_SMSConfig config = con.tbl_SMSConfig.Take(1).First();
                sms.sendTSMS(config.TR_URL, config.TR_USER, config.TR_PWD, config.TR_SID, "N", sendToPhoneNumber, "Your TYS361 Verification Code " + verifyCode);
                //sms.sendTSMS(config.TR_URL, config.TR_USER, config.TR_PWD, config.TR_SID, "N", sendToPhoneNumber, "Your XIPHIAS - TYS Verification code is " + verifyCode);
                //   sms.sendTSMS(sendToPhoneNumber, "Your XIPHIAS - ATSI Verification code is " + verifyCode, "N");
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.ToString());
            }
        }
        public int RegisteralerdyUser(RegisterStudentModel regis)
        {
            veryCode = this.CreateRandomCode(6);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "SP_updateeMobileVerificationMst";
            cmd.Connection = conn;

            cmd.Parameters.AddWithValue("@MobileNo", regis.mobile_no);
            cmd.Parameters.AddWithValue("@VerificationCode", veryCode);
            cmd.Parameters.AddWithValue("@IsRegistered", 0);
            cmd.Parameters.AddWithValue("@CurrentDate", DateTime.Now.ToString("dd-MMM-yyyy,hh:mm:ss"));
         

            conn.Open();
            int j = cmd.ExecuteNonQuery();
            conn.Close();
            if (j > 0)
            {
                MobileSms(regis.mobile_no);
                SendEMailUser(regis.email, veryCode ,regis.full_name);
            }
            return j;
        }
        public int RegisterUser(RegisterStudentModel regis)
        {
            veryCode = this.CreateRandomCode(6);

            SessFrm = DateTime.Now.Year.ToString();
            SessTo = (Convert.ToInt32(SessFrm) + 1).ToString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "SP_MobileVerificationMst";
            cmd.Connection = conn;
            cmd.Parameters.AddWithValue("@MobileNo", regis.mobile_no);
            cmd.Parameters.AddWithValue("@VerificationCode", veryCode);
            cmd.Parameters.AddWithValue("@CurrentDate", DateTime.Now.ToString("dd-MMM-yyyy,hh:mm:ss"));
            cmd.Parameters.AddWithValue("@SessFrom", SessFrm);
            cmd.Parameters.AddWithValue("@SessTo", SessTo);
            cmd.Parameters.AddWithValue("@IsRegistered", 0);
            cmd.Parameters.AddWithValue("@FullName", regis.full_name);
            cmd.Parameters.AddWithValue("@Email", regis.email);
            conn.Open();
            int j = cmd.ExecuteNonQuery();
            conn.Close();

            if (j > 0)
            {
                MobileSms(regis.mobile_no);
                SendEMailUser(regis.email, veryCode ,regis.full_name);
            }// var data = con.MobileVerificationMsts.Where(r => r.MobileNo == regis.MobileNo && r.IsRegistered == false).FirstOrDefault();
            return j;
        }
        public int OTP_verify(otp otp, string name, string email)
        {

            SessFrm = DateTime.Now.Year.ToString();
            SessTo = (Convert.ToInt32(SessFrm) + 1).ToString();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "SP_UpdateMobileVerificationMst";
            cmd.Connection = conn;

            cmd.Parameters.AddWithValue("@MobileNo", otp.mobile_no);
            cmd.Parameters.AddWithValue("@VerificationCode", otp.verification_code);
            cmd.Parameters.AddWithValue("@IsRegistered", 1);
            cmd.Parameters.AddWithValue("@CurrentDate", DateTime.Now.ToString("dd-MMM-yyyy,hh:mm:ss"));
            cmd.Parameters.AddWithValue("@SessFrom", SessFrm);
            cmd.Parameters.AddWithValue("@SessTo", SessTo);

            conn.Open();
            int j = cmd.ExecuteNonQuery();
            conn.Close();

            //aspnet users
            var getnetuserdeatils = new AspNetUser();
            getnetuserdeatils.Email = email;
            getnetuserdeatils.PhoneNumber = otp.mobile_no;
            int getcount = con.AspNetUsers.Count();
            getnetuserdeatils.Id = (getcount + 1).ToString();
            getnetuserdeatils.EmailConfirmed = true;
            var getpaswrdhash = HashPassword(email);
            getnetuserdeatils.PasswordHash = getpaswrdhash;
            string securitystmp = email + (getcount + 2);
            var getsecuritystmp = HashPassword(securitystmp);
            getnetuserdeatils.SecurityStamp = getsecuritystmp;
            getnetuserdeatils.PhoneNumberConfirmed = true;
            getnetuserdeatils.TwoFactorEnabled = false;
            getnetuserdeatils.LockoutEnabled = true;
            getnetuserdeatils.AccessFailedCount = 0;
            getnetuserdeatils.UserName = email;
            con.AspNetUsers.Add(getnetuserdeatils);
            con.SaveChanges();


            //user role map
            var usrRole = new UserRoleMap();
            var roleMaster = con.RoleMasters.Where(r => r.RoleName.Contains("Student"));
            var roleId = roleMaster.Count() > 0 ? roleMaster.FirstOrDefault().Id : 0;
            usrRole.UserId = con.AspNetUsers.Where(r => r.UserName == otp.email).FirstOrDefault().UId;
            usrRole.RoleId = roleId;
            usrRole.Status = true;
            usrRole.Deleted = false;
            usrRole.CreatedOn = DateTime.Now;
            usrRole.CreatedBy = con.AspNetUsers.Where(r => r.UserName == otp.email).FirstOrDefault().UId;
            usrRole.UpdatedOn = DateTime.Now;
            usrRole.UpdatedBy = con.AspNetUsers.Where(r => r.UserName == otp.email).FirstOrDefault().UId;
            usrRole.OrgId = 1;
            usrRole.SiteId = 1;
            con.UserRoleMaps.Add(usrRole);
            con.SaveChanges();

            ///UserDetails
            var Usrdetail = new UserDetail();
            var cnf = new MobileVerificationMst();
            Usrdetail.Email = email;
            Usrdetail.Mobile = otp.mobile_no;
            Usrdetail.Name = name;
            Usrdetail.Gender = "";
            Usrdetail.CityMasterId = 0;
            Usrdetail.SchoolMasterId = 0;
            Usrdetail.ClassMasterId = 0;
            Usrdetail.ClassId = 0;
            Usrdetail.IsSetPass = false;
            Usrdetail.PackageType = "";
            Usrdetail.Status = true;
            Usrdetail.Deleted = false;
            Usrdetail.CreatedOn = DateTime.Now;
            Usrdetail.CreatedBy = con.AspNetUsers.Where(r => r.UserName == otp.email).FirstOrDefault().UId;
            Usrdetail.UpdatedOn = DateTime.Now;
            Usrdetail.UpdatedBy = con.AspNetUsers.Where(r => r.UserName == otp.email).FirstOrDefault().UId;
            Usrdetail.OrgId = 1;
            Usrdetail.SiteId = 1;
            Usrdetail.UserId = usrRole.UserId.Value;
            con.UserDetails.Add(Usrdetail);
            con.SaveChanges();


            return j;
        }
        public int Resend_OTP(resendotp resendotp, long dataId)
        {
            SessFrm = DateTime.Now.Year.ToString();
            SessTo = (Convert.ToInt32(SessFrm) + 1).ToString();
            veryCode = this.CreateRandomCode(6);

            MobileSms(resendotp.mobile_no);

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "SP_UpdateMobileVerificationMst";
            cmd.Connection = conn;

            cmd.Parameters.AddWithValue("@MobileNo", resendotp.mobile_no);
            cmd.Parameters.AddWithValue("@VerificationCode", veryCode);
            cmd.Parameters.AddWithValue("@IsRegistered", 0);
            cmd.Parameters.AddWithValue("@CurrentDate", DateTime.Now.ToString("dd-MMM-yyyy,hh:mm:ss")
                );
            cmd.Parameters.AddWithValue("@SessFrom", SessFrm);
            cmd.Parameters.AddWithValue("@SessTo", SessTo);

            conn.Open();
            int j = cmd.ExecuteNonQuery();
            conn.Close();
            SendMailUser(resendotp.email, veryCode ,resendotp.full_name);
            //var data = con.MobileVerificationMsts.Where(r => r.MobileNo == resendotp.mobile_no && r.IsRegistered == false).FirstOrDefault();


            return j;
        }
        public void SendMailUser(string email, string otp ,string name  )
        {


            MailMessage mail = new MailMessage();
            mail.To.Add(email);
          mail.From = new MailAddress("<postmaster@xiphiasconnect.com>");
            mail.Subject = "Your OTP for TYS 361";
            mail.Body = "<table align ='center'width ='600'style ='text-align:center;color:rgba(0, 117, 187, 0.99); border:3px solid rgba(165, 165, 165, 0.76)'><tbody> <tr> <td> <table align ='center'width ='580'><tbody> <tr><td> <div style = 'border:4px solid rgba(0, 101, 25, 0.76)'>";
            mail.Body = mail.Body + "<h1 style ='text-align:center;color:rgba(0, 117, 187, 0.99);font-family: sans-serif;font-size: 20px;'> Your TYS361 One-Time Password </h1></div></td></tr></tbody></table></td></tr><tr> <td style ='padding: 10px;'>";
            mail.Body = mail.Body + "<p style ='text-align:justify; color:#000;font-family: sans-serif;font-size: 16px;'>Hi <strong>" + name + "</strong>,</p> <p style='text-align:justify; color:#000;font-family: sans-serif;font-size: 16px;'>Welcome to TYS361, We're so excited that you have chosen us.</p>";
            mail.Body = mail.Body + "<p style ='text-align:justify; color:#000;font-family: sans-serif;font-size: 16px;'> <strong>" + otp + "</strong > is your One-Time Password. It is valid for 15 minutes to log into your account. If you did not attempt this action which has triggered this One Time Password please ignore.</p>";
            mail.Body = mail.Body + "<p style ='text-align:justify; color:#000;font-family: sans-serif;font-size: 16px;'> Welcome once again to the TYS 361 Family.</p> ";
            mail.Body = mail.Body + "<p style ='text-align:justify;color:#000;font-family: sans-serif;font-size: 16px;'> Regards,</p>";
            mail.Body = mail.Body + "<p style ='text-align:justify;color:#000;font-family: sans-serif;font-size: 16px;margin-top:-12px;'> TYS361 Team.</p>";
            mail.Body = mail.Body + "<p style ='text-align:justify;color:#000;font-family: sans-serif;font-size: 16px;margin-top:-12px;'> www.tys361.com</p> ";
            mail.Body = mail.Body + "<p style ='text-align:justify; color:#000;font-family: sans-serif;font-size: 16px;'> To Contact Us: Email : info@tys361.com,Phone : +91 80-67601000</p>";
            mail.Body = mail.Body + "</td ></tr></tbody></table> ";
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.mailgun.org";
            smtp.Port = 587;
            //   smtp.Port = 25;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("postmaster@xiphiasconnect.com", "#xiphias0101");
            smtp.EnableSsl = true;
            smtp.Send(mail);
           

        }
        public string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            string abc = Convert.ToBase64String(dst);
            return abc;
        }


        #endregion

        #region Profile Updation
        public void Profileupdate(ProfileUpdationModel profilemodelRe)
        {
            var data = con.UserDetails.Where(r => r.UserId == profilemodelRe.userid).FirstOrDefault();
            var email = data != null ? data.Email : null;
            var mod = con.UserDetails.Find(profilemodelRe.userid);
            mod.Email = profilemodelRe.email;
            //mod.Id = profilemodelRe.id;
            mod.Mobile = profilemodelRe.mobile;
            mod.Name = profilemodelRe.name;
            mod.Gender = profilemodelRe.gender;
            mod.Address = profilemodelRe.address;
            if (profilemodelRe.gender == "MALE")
            {
                mod.ImageUrl = "~/Storage/UserImage/boy.png";
            }
            else if (profilemodelRe.gender == "FEMALE")
            {
                mod.ImageUrl = "~/Storage/UserImage/Lady.png";
            }
            else
            {
                mod.ImageUrl = "~/Storage/UserImage/";
            }

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "SP_UpdateASPNetUsers";
            cmd.Connection = conn;

            cmd.Parameters.AddWithValue("@UId", profilemodelRe.userid);
            cmd.Parameters.AddWithValue("@Email", profilemodelRe.email);
            cmd.Parameters.AddWithValue("@PhoneNumber", profilemodelRe.mobile);
            cmd.Parameters.AddWithValue("@UserName", profilemodelRe.email);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            var userid = data != null ? data.UserId : 0;
            var parentdata = con.ParentDetails.Where(r => r.UserId == userid).FirstOrDefault();
            if (parentdata == null)
            {
                var parent = new ParentDetail();
                parent.FatherName = profilemodelRe.father_name;
                parent.MotherName = profilemodelRe.mother_name;
                parent.ParentAlternativeEmail = profilemodelRe.parentalternative_email;
                parent.ParentAlternativeMobile = profilemodelRe.parentalternative_mobile;
                parent.ParentEmail = profilemodelRe.parent_email;
                parent.ParentMobile = profilemodelRe.parent_mobile;
                parent.UserId = userid;
                con.ParentDetails.Add(parent);
                con.SaveChanges();

            }
            else
            {
                var parent = con.ParentDetails.Find(parentdata.Id);
                parent.FatherName = profilemodelRe.father_name;
                parent.MotherName = profilemodelRe.mother_name;
                parent.ParentAlternativeEmail = profilemodelRe.parentalternative_email;
                parent.ParentAlternativeMobile = profilemodelRe.parentalternative_mobile;
                parent.ParentEmail = profilemodelRe.parent_email;
                parent.ParentMobile = profilemodelRe.parent_mobile;

                con.Entry(parent).State = EntityState.Modified;
                con.SaveChanges();
            }
            
        }

        public int saveTestAnalytic(int userId, int QuestionId, decimal QuestionMarks, int StudentTestId, int ansTime, int QReadTime, int OptnReadTime)
        {
            var model = new TestAnalytic();
            model.UserId = userId;
            model.QuestionId = QuestionId;
            model.QuestionMarks = QuestionMarks;
            model.StudentTestId = StudentTestId;
            model.AnsTime = ansTime;
            model.QReadTime = QReadTime;
            model.OptnReadTime = OptnReadTime;
            model.Accurecy = "";
            model.Status = true;
            model.CreatedBy = userId;
            model.CreatedOn = DateTime.Now;
            model.UpdatedBy = userId;
            model.UpdatedOn = DateTime.Now;
            con.TestAnalytics.Add(model);
            con.SaveChanges();

            int d = 0;

            //var time = con.TestAnalytics.Where(t => t.UserId == userId && t.StudentTestId == StudentTestId && t.CreatedOn.Year == DateTime.Now.Year && t.CreatedOn.Month == DateTime.Now.Month && t.CreatedOn.Day == DateTime.Now.Day).ToList();

            //foreach (var i in time)
            //{

            //    //TimeSpan varTime = Convert.ToDateTime(i.AnsClickTime) - Convert.ToDateTime(i.AnsViewTime);
            //    //double fractionalMinutes = varTime.TotalSeconds;
            //    //int wholeSeconds = (int)fractionalMinutes;

            //    //d = d + wholeSeconds;

            //    d =Convert.ToInt32(d + i.AnsTime);
            //}

            return d;
        }

        public void PrepareModelScheduleMockTestTypes(CategoryMock model, StudentMockTestScheduleView test)
        {
            if (test != null)
            {
                model.id = test.Id;
                var subjectdetail = con.StudentTestSchedules.Where(r => r.TestId == test.TestId).ToList();
                model.type = test.Type;
                model.test_id = test.TestId;
                int TopicId = Convert.ToInt32(con.Tests.Where(t => t.Id == test.TestId).FirstOrDefault().TopicId);
                string TopicName = con.Topics.Where(t => t.Id == TopicId).FirstOrDefault().TopicName;
                model.topic_name = TopicName;
                model.date = Convert.ToDateTime(test.Date).Date.ToString("dd-MM-yyyy");
                // model.date = test.Date.ToString("dd-MM-yyyy");           
                model.time = test.Time;
            }
        }
        public TestQuestionsListModel GetQuestionOptions(List<TestQuestionMock> total_questions, int studenttest_id)
        {
            TestContexts.qid = 0;
            List<categorymockwise> question_answers = new List<categorymockwise>();
            List<categorymockwise> tot_questions = new List<categorymockwise>();
            List<QMCQAnswers> answers = new List<QMCQAnswers>();
            TestQuestionsListModel modss = new TestQuestionsListModel();
            categorymockquestions model1 = new categorymockquestions();
            var questions = total_questions;
            int quecount = questions.Count();
            try
            {
                var ans = (dynamic)null;
                foreach (var itm in questions)
                {
                    if (quecount >= TestContexts.qid)
                    {

                        string que = "";
                        int quid = questions[TestContexts.qid].Id;
                        string imag = questions[TestContexts.qid].ImageUrl;
                        if (imag == "" || imag == null)
                        {
                            imag = null;
                            que = questions[TestContexts.qid].Question;

                        }
                        else
                        {
                            que = "<p>" + questions[TestContexts.qid].Question + "<br/><img style=width:100 %; src=http://www.tys361.com/tys/Content/images/" + questions[TestContexts.qid].ImageUrl + " /></p>";
                            imag = questions[TestContexts.qid].ImageUrl;
                        }
                        int StudentTestId = Convert.ToInt32(studenttest_id);
                        var onlinetest = con.MCQAnswersMocks.Where(r => r.TestQuestionMockId == quid && r.Deleted == false && r.Status == true).OrderBy(r => r.OrderNo).ToList();
                        var answerview = con.StudentTestAnswers.Where(r => r.StudentTestsId == StudentTestId && r.TestQuestionId == quid).ToList();
                        List<QMCQAnswers> AvailableMCQAnswer = new List<QMCQAnswers>();
                        string answerss = ""; int mcq_count = 0;
                        if (onlinetest.Count() > 0)
                        {
                            if (answerview.Count <= 0)
                            {
                                foreach (var i in onlinetest)
                                {
                                    //var mcq = _defContext.studenttestanswersContext.Where(r => r.MCQAnswersId == i.Id).ToList();
                                    var answer = con.StudentTestAnswers.Where(r => r.TestQuestionId == i.TestQuestionMockId && r.StudentTestsId == StudentTestId).ToList();
                                    if (i.Image != null)
                                    {
                                        string d = i.Answer ?? answer.FirstOrDefault().Answers;
                                        string img = i.Image ?? null;
                                        ans = "<p>" + d + "<br/><img style=width:100 %; src=http://www.tys361.com/tys/Content/images/" + img + " /></p>";
                                       
                                    }
                                    else
                                    {
                                        ans = i.Answer ?? answer.FirstOrDefault().Answers;
                                    }
                                    AvailableMCQAnswer.Add(new QMCQAnswers
                                    {
                                        id = i.Id,
                                        answer = ans,
                                        image = i.Image ?? null,
                                        is_image = (i.Image != null),
                                        //IsCorrect = false
                                    });
                                }
                            }
                            if (answerview.Count > 0)
                            {

                                foreach (var i in onlinetest)
                                {
                                    var mcq = con.MCQAnswersMocks.Where(r => r.Id == i.Id).ToList();
                                    var answer = con.StudentTestAnswers.Where(r => r.TestQuestionId == i.TestQuestionMockId && r.StudentTestsId == StudentTestId).ToList();
                                    if (i.Image != null)
                                    {
                                        string d = i.Answer ?? answer.FirstOrDefault().Answers;
                                        string img = i.Image ?? null;
                                        ans = "<p>" + d + "<br/><img style=width:100 %; src=http://www.tys361.com/tys/Content/images/" + img + " /></p>";
                                    }
                                    else
                                    {
                                        ans = i.Answer ?? answer.FirstOrDefault().Answers;
                                    }
                                    AvailableMCQAnswer.Add(new QMCQAnswers
                                    {
                                        id = i.Id,
                                        answer = ans,
                                        image = i.Image ?? null,
                                        is_image = (i.Image != null),
                                        is_correct = (answer.FirstOrDefault().MCQAnswersId == mcq.FirstOrDefault().Id ? true : false)               //(mcq.Count()>0? true:false)
                                    });
                                }
                            }
                            answers = AvailableMCQAnswer;

                        }
                        else
                        {
                            foreach (var i in answerview)
                            {
                                AvailableMCQAnswer.Add(new QMCQAnswers
                                {
                                    id = i.Id,
                                    answer = i.Answers,

                                    is_correct = true,
                                });
                                answerss = i.Answers;
                            }
                            answers = AvailableMCQAnswer;
                            mcq_count = onlinetest.Count();
                        }

                        var mod1 = new categorymockwise
                        {
                            question = que,
                            available_mcqanswer = answers,
                            question_id = quid,
                            answer = answerss,
                            mcq_count = mcq_count,
                            imageurl = imag,
                            is_image = (imag != null)

                        };
                        question_answers.Add(mod1);
                        modss.ansque = question_answers;
                        TestContexts.qid++;
                        //return model1;
                    }
                    else
                    {
                        var Referencno = con.StudentTests.Find(Convert.ToInt32(studenttest_id));
                        string Refno = Referencno != null ? Referencno.Refno : "no refno generated.";
                        model1.message = "End of the Test";
                        model1.status = false;
                        //model1.refno = Refno;
                        // return model1;
                    }
                }
                modss.status = true;
                modss.message = "Qusetions and answers are in list";
                return modss;
            }
            catch (Exception ex)
            {
                modss.status = false;
                modss.message = "Failed";
                return modss;
            }
        }

        #endregion

        #region Login region
        public void SendEMailUser(string email, string otp ,string name )
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(email);
          mail.From = new MailAddress("<postmaster@xiphiasconnect.com>");
            mail.Subject = "Your OTP for TYS 361";
            mail.Body = "<table align ='center'width ='600'style ='text-align:center;color:rgba(0, 117, 187, 0.99); border:3px solid rgba(165, 165, 165, 0.76)'><tbody> <tr> <td> <table align ='center'width ='580'><tbody> <tr><td> <div style = 'border:4px solid rgba(0, 101, 25, 0.76)'>";
            mail.Body = mail.Body + "<h1 style ='text-align:center;color:rgba(0, 117, 187, 0.99);font-family: sans-serif;font-size: 20px;'> Your TYS361 One-Time Password </h1></div></td></tr></tbody></table></td></tr><tr> <td style ='padding: 10px;'>";
            mail.Body = mail.Body + "<p style ='text-align:justify; color:#000;font-family: sans-serif;font-size: 16px;'>Hi <strong>" + name + "</strong>,</p> <p style='text-align:justify; color:#000;font-family: sans-serif;font-size: 16px;'>Welcome to TYS361, We're so excited that you have chosen us.</p>";
            mail.Body = mail.Body + "<p style ='text-align:justify; color:#000;font-family: sans-serif;font-size: 16px;'> <strong>" + otp + "</strong > is your One-Time Password. It is valid for 15 minutes to log into your account. If you did not attempt this action which has triggered this One Time Password please ignore.</p>";
            mail.Body = mail.Body + "<p style ='text-align:justify; color:#000;font-family: sans-serif;font-size: 16px;'> Welcome once again to the TYS 361 Family.</p> ";
            mail.Body = mail.Body + "<p style ='text-align:justify;color:#000;font-family: sans-serif;font-size: 16px;'> Regards,</p>";
            mail.Body = mail.Body + "<p style ='text-align:justify;color:#000;font-family: sans-serif;font-size: 16px;margin-top:-12px;'> TYS361 Team.</p>";
            mail.Body = mail.Body + "<p style ='text-align:justify;color:#000;font-family: sans-serif;font-size: 16px;margin-top:-12px;'> www.tys361.com</p> ";
            mail.Body = mail.Body + "<p style ='text-align:justify; color:#000;font-family: sans-serif;font-size: 16px;'> To Contact Us: Email : info@tys361.com,Phone : +91 80-67601000</p>";
            mail.Body = mail.Body + "</td ></tr></tbody></table> ";
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.mailgun.org";
            smtp.Port = 587;
            //   smtp.Port = 25;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("postmaster@xiphiasconnect.com", "#xiphias0101");
            smtp.EnableSsl = true;
            smtp.Send(mail);
         
            //Dispose();
        }
        public void MobileLoginSms(string mobileNumber, string otp)
        {
            try
            {
                String sendToPhoneNumber = mobileNumber;
                ISMSSender sms = new SMSSender();
                //     MobileSMS sms = new MobileSMS();

                tbl_SMSConfig config = con.tbl_SMSConfig.Take(1).First();
                sms.sendTSMS(config.TR_URL, config.TR_USER, config.TR_PWD, config.TR_SID, "N", sendToPhoneNumber, "Your TYS361 Verification Code" + otp);
                //sms.sendTSMS(config.TR_URL, config.TR_USER, config.TR_PWD, config.TR_SID, "N", sendToPhoneNumber, "Your XIPHIAS - TYS Verification code is " + verifyCode);
                //   sms.sendTSMS(sendToPhoneNumber, "Your XIPHIAS - ATSI Verification code is " + verifyCode, "N");
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.ToString());
            }
        }

        public bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            bool areEqual = buffer3.SequenceEqual(buffer4);
            return areEqual;
        }


        #endregion

        #region Live test
        public void PrepareModelScheduleLiveTestTypes(CategoryMock model, StudentTestScheduleView test)
        {
            if (test != null)
            {              
                model.id = test.Id;
                //  var subjectdetail = con.StudentTestSchedules.Where(r => r.TestId == test.TestId).ToList();
                model.type = test.Type;
                model.test_id = test.TestId;
                int TopicId = Convert.ToInt32(con.Tests.Where(t => t.Id == test.TestId).FirstOrDefault().TopicId);
                string TopicName = con.Topics.Where(t => t.Id == TopicId).FirstOrDefault().TopicName;
                model.topic_name = TopicName;
                model.date = Convert.ToDateTime(test.Date).Date.ToString("dd-MM-yyyy");
                model.time = test.Time;
            }
        }
        ArrayList ListArray = new ArrayList();

        public int RetQID(List<OnlineCategoryWiseTestVw> questions) // Generate Random Question Method.
        {

            int questioncount = questions.Count;
            //-------------------
            TestContexts t1 = new TestContexts();
            Random rnd = new Random();
            if (t1.random_qid == null)
                t1.random_qid = new ArrayList();
            if (ListArray != null)
            {
                t1.random_qid = new ArrayList();
                for (int k = 0; k < (ListArray).Count; k++)
                {
                    t1.random_qid.Add((ListArray)[k]);
                }
            }
            int RandomQuesId = rnd.Next(0, (questioncount - 1));
            if (t1.random_qid.Count <= 0)
            {
                t1.random_qid.Add(RandomQuesId);
            }
            else
            {
                if (questioncount == t1.random_qid.Count)
                {
                }
                else
                {
                    int chkQid = 0;
                    DoAgain:
                    RandomQuesId = rnd.Next(0, (questioncount));
                    for (int j = 0; j < t1.random_qid.Count; j++)
                    {
                        if (RandomQuesId == (Int32)t1.random_qid[j])
                        {
                            chkQid = 1;
                        }
                    }
                    if (chkQid == 1)
                    {
                        chkQid = 0;
                        goto DoAgain;
                    }
                    t1.random_qid.Add(RandomQuesId);
                }
            }
            ListArray = t1.random_qid;
            return RandomQuesId;
            
           
            //Session["ListArray"] = t1.random_qid;
            //---------------

        }

        public TestQuestionsListModel GetQuestionMCQOptions(List<OnlineCategoryWiseTestVw> total_questions, int studenttest_id)
        {
            TestContexts.qid = 0;
            List<categorymockwise> question_answers = new List<categorymockwise>();
            List<categorymockwise> tot_questions = new List<categorymockwise>();
            List<QMCQAnswers> answers = new List<QMCQAnswers>();
            TestQuestionsListModel modss = new TestQuestionsListModel();
            categorymockquestions model1 = new categorymockquestions();

            var questions = total_questions;
            int quecount = questions.Count();
            try
            {
                foreach (var itm in questions)
                {
                    if (quecount >= TestContexts.qid)
                    {
                        string que = "";
                        int quid = questions[TestContexts.qid].TestQuestionId;
                        string imag = questions[TestContexts.qid].ImageUrl;
                        if (imag == "" || imag == null)
                        {
                            que = questions[TestContexts.qid].Question;
                            imag = null;
                        }
                        else
                        {
                           
                            que = "<p>" + questions[TestContexts.qid].Question + "<br/><img style=width:100 %; src=http://www.tys361.com/tys/Content/images/" + questions[TestContexts.qid].ImageUrl + " /></p>"; 
                            imag = questions[TestContexts.qid].ImageUrl;
                        }
                        int StudentTestId = Convert.ToInt32(studenttest_id);
                        var onlinetest = con.MCQAnswers.Where(r => r.TestQuestionId == quid && r.Deleted == false && r.Status == true).OrderBy(r => r.OrderNo).ToList();
                        var answerview = con.StudentTestAnswers.Where(r => r.StudentTestsId == StudentTestId && r.TestQuestionId == quid).ToList();
                        List<QMCQAnswers> AvailableMCQAnswer = new List<QMCQAnswers>();
                        string answerss = ""; int mcq_count = 0;
                        var ans = (dynamic)null;
                        if (onlinetest.Count() > 0)
                        {
                            if (answerview.Count <= 0)
                            {
                                foreach (var i in onlinetest)
                                {
                                    //var mcq = _defContext.studenttestanswersContext.Where(r => r.MCQAnswersId == i.Id).ToList();
                                    var answer = con.StudentTestAnswers.Where(r => r.TestQuestionId == i.TestQuestionId && r.StudentTestsId == StudentTestId).ToList();
                                   if(i.Image!=null)
                                    {
                                        string d = i.Answer ?? answer.FirstOrDefault().Answers;
                                        string img = i.Image ?? null;
                                    ans = "<p>"+d+ "<br/><img style=width:100 %; src=http://www.tys361.com/tys/Content/images/" +img+ " /></p>";
                                    }
                                   else
                                    {
                                        ans = i.Answer ?? answer.FirstOrDefault().Answers;
                                    }

                                    AvailableMCQAnswer.Add(new QMCQAnswers
                                    {
                                        id = i.Id,
                                        //answer = i.Answer ?? answer.FirstOrDefault().Answers,
                                        answer = ans,
                                        image = i.Image ?? null,
                                        is_image = (i.Image != null),
                                        is_correct = i.IsCorrect
                                    });
                                }
                            }
                            if (answerview.Count > 0)
                            {

                                foreach (var i in onlinetest)
                                {
                                    var mcq = con.MCQAnswersMocks.Where(r => r.Id == i.Id).ToList();
                                    var answer = con.StudentTestAnswers.Where(r => r.TestQuestionId == i.TestQuestionId && r.StudentTestsId == StudentTestId).ToList();
                                    if (i.Image != null)
                                    {
                                        string d = i.Answer ?? answer.FirstOrDefault().Answers;
                                        string img = i.Image ?? null;
                                        ans = "<p>" + d + "<br/><img style=width:100 %; src=http://www.tys361.com/tys/Content/images/" + img + " /></p>";
                                    }
                                    else
                                    {
                                        ans = i.Answer ?? answer.FirstOrDefault().Answers;
                                    }
                                    AvailableMCQAnswer.Add(new QMCQAnswers
                                    {
                                        id = i.Id,
                                        answer = ans,
                                        image = i.Image ?? null,
                                        is_image=(i.Image!=null),
                                        is_correct = (answer.FirstOrDefault().MCQAnswersId == mcq.FirstOrDefault().Id ? true : false)               //(mcq.Count()>0? true:false)
                                    });
                                }
                            }
                            answers = AvailableMCQAnswer;

                        }
                        else
                        {
                            foreach (var i in answerview)
                            {
                                AvailableMCQAnswer.Add(new QMCQAnswers
                                {
                                    id = i.Id,
                                    answer = i.Answers,
                                    is_correct = true,
                                });
                                answerss = i.Answers;
                            }
                            answers = AvailableMCQAnswer;
                            mcq_count = onlinetest.Count();
                        }

                        var mod1 = new categorymockwise
                        {
                            question = que,
                            available_mcqanswer = answers,
                            question_id = quid,
                            answer = answerss,
                            mcq_count = mcq_count,
                            imageurl = imag,
                            is_image=(imag!=null)
                          
                        };
                        question_answers.Add(mod1);
                        modss.ansque = question_answers;
                        TestContexts.qid++;
                        //return model1;
                    }
                    else
                    {
                        var Referencno = con.StudentTests.Find(Convert.ToInt32(studenttest_id));
                        string Refno = Referencno != null ? Referencno.Refno : "no refno generated.";
                        model1.message = "End of the Test";
                        model1.status = false;
                        //model1.refno = Refno;
                        // return model1;
                    }
                }
                modss.status = true;
                modss.message = "Qusetions and answers are in list";
                return modss;
            }
            catch (Exception ex)
            {
                modss.status = false;
                modss.message = "Failed";
                return modss;
            }
        }


        #endregion
        public void ScheduleLiveTest(ScheduleTestgetValues testdetails)
        {
            string d = testdetails.date;
            var model = new StudentTestSchedule();
            model.UserDetailsId = testdetails.user_id;
            model.Type = testdetails.type;
            model.TestId = testdetails.test_id;
            model.QuestionPatternId = testdetails.question_pattern_id;
            model.Date=Convert.ToDateTime(d);
            model.Time = testdetails.time;
            model.Refno= Guid.NewGuid().ToString().Substring(0, 10).ToUpper();
            model.IsTestChecked = false;
            model.TestCheckedBy = 0;
            model.NoOfTimesReschedule = 0;
            model.Deleted = false;
            model.CreatedBy = testdetails.user_id; 
            model.CreatedOn = DateTime.Now;
            model.UpdatedBy = testdetails.user_id;
            model.UpdatedOn = DateTime.Now; 
            model.OrgId = 1;
            model.SiteId = 1;
            model.Status = true;
            con.StudentTestSchedules.Add(model);
            con.SaveChanges();
        }
        public void ScheduleMockTest(ScheduleMockTestPassId MockValues)
        {
            string d = MockValues.date;
            var model = new ScheduleMockTest();
            var studpackage = con.StudentPackageSelections.Where(r => r.UserId == MockValues.user_id).ToList();
                model.UserDetailsId = MockValues.user_id;
                model.Type = MockValues.type;
                model.TestId = MockValues.test_id;
                model.QuestionPatternId = MockValues.question_pattern_id;
                model.QuestionSetId = MockValues.question_set;
                model.JumbleTypeId = MockValues.jumble_type;
                model.ParticularYearId = MockValues.year;
                model.FromYear = MockValues.from_year;
                model.ToYear = MockValues.to_year;
                model.DurationId = MockValues.duration_id;
                model.IsTrialTest = false;
                model.Date = Convert.ToDateTime(d);
                model.Time = MockValues.time;
                model.Refno = Guid.NewGuid().ToString().Substring(0, 10).ToUpper();
                model.IsTestChecked = false;
                model.TestCheckedBy = 0;
                model.Deleted = false;
                model.CreatedBy = MockValues.user_id;
                model.CreatedOn = DateTime.Now;
                model.UpdatedBy = MockValues.user_id;
                model.UpdatedOn = DateTime.Now;
                model.OrgId = 1;
                model.SiteId = 1;
                model.Status = true;
            model.IsTYPTest = false;
                con.ScheduleMockTests.Add(model);
                con.SaveChanges();           
        }
        public void ScheduleTrialMockTest(ScheduleMockTestPassId MockValues)
        {
                string d = MockValues.date;
                var model = new ScheduleMockTest();
                model.UserDetailsId = MockValues.user_id;
                model.Type = MockValues.type;
                model.TestId = MockValues.test_id;
                model.QuestionPatternId = MockValues.question_pattern_id;
                model.QuestionSetId = MockValues.question_set;
                model.JumbleTypeId = MockValues.jumble_type;
                model.ParticularYearId = MockValues.year;
                model.FromYear = MockValues.from_year;
                model.ToYear = MockValues.to_year;
                model.DurationId = MockValues.duration_id;
                model.DurationId = MockValues.duration_id;
                model.IsTrialTest = true;
                model.Date = Convert.ToDateTime(d);
                model.Time = MockValues.time;
                model.Refno = Guid.NewGuid().ToString().Substring(0, 10).ToUpper();
                model.IsTestChecked = false;
                model.TestCheckedBy = 0;
                model.Deleted = false;
                model.CreatedBy = MockValues.user_id;
                model.CreatedOn = DateTime.Now;
                model.UpdatedBy = MockValues.user_id;
                model.UpdatedOn = DateTime.Now;
                model.OrgId = 1;
                model.SiteId = 1;
                model.Status = true;
            model.IsTYPTest = false;
                con.ScheduleMockTests.Add(model);
                con.SaveChanges();
        }

        public void scheduleMockTYPtest(ScheduleMockTYPTest MockValues)
        {

            string d = MockValues.date;
            var model = new ScheduleMockTest();
            model.UserDetailsId = MockValues.user_id;
            model.Type = MockValues.type;
            model.TestId = MockValues.test_id;
            model.QuestionPatternId = MockValues.question_pattern_id;
            model.QuestionSetId =1;
            model.JumbleTypeId = 0;
            model.ParticularYearId = MockValues.year;
            model.FromYear = 0;
            model.ToYear = 0;
            model.DurationId = MockValues.duration_id;
            model.IsTrialTest = false;
            model.Date = Convert.ToDateTime(d);
            model.Time = MockValues.time;
            model.Refno = Guid.NewGuid().ToString().Substring(0, 10).ToUpper();
            model.IsTestChecked = false;
            model.TestCheckedBy = 0;
            model.Deleted = false;
            model.CreatedBy = MockValues.user_id;
            model.CreatedOn = DateTime.Now;
            model.UpdatedBy = MockValues.user_id;
            model.UpdatedOn = DateTime.Now;
            model.IsTYPTest = true;
            model.OrgId = 1;
            model.SiteId = 1;
            model.Status = true;
            con.ScheduleMockTests.Add(model);
            con.SaveChanges();
        }
        
        public void ViewResultPost(TestEvaluationModel model)
        {
            var answerevaluation = new TestAnswersEvaluation();
            var totaltestmarks = new TestTotalMark();         
            var userid = con.UserDetails.Where(r => r.UserId == model.StudentTestUserId);
            decimal totalCorrectMarks = 0;
            decimal totalNegativemarks = 0;
            int totalcorrectans = 0;
            int totalnegativeans = 0;
            int nid = 0;
            int totmarkid = 0;
            int StudentTestId = 0;
            int checkedby = con.UserDetails.Where(r => r.UserId ==model.StudentTestUserId).FirstOrDefault().Id;
            for (int i = 0; i < model.AvailableAnswerEvaluation.Count; i++)
            {
                int count = model.AvailableAnswerEvaluation.Count;
                var correct = model.AvailableAnswerEvaluation.Where(r => r.IsCorrect == true).Count();
                nid = count - int.Parse(correct.ToString());
                if (model.AvailableAnswerEvaluation[i].IsCorrect == true)
                {
                    var aa = model.AvailableAnswerEvaluation[i].MarksObtained;
                    var bb = aa != 0 ? aa : model.AvailableAnswerEvaluation[i].MaxMarks;
                    totalCorrectMarks +=Convert.ToDecimal( bb);

                    totalcorrectans = int.Parse(correct.ToString());
                }
                if (model.AvailableAnswerEvaluation[i].IsCorrect == false)
                {
                    var aa = model.AvailableAnswerEvaluation[i].NegativeMarks;
                    var bb = aa != 0 ? aa : 0;
                    totalNegativemarks +=Convert.ToDecimal(bb);
                    totalnegativeans = nid;
                }
            }
            foreach (var i in model.AvailableAnswerEvaluation)
            {
                var stansid = con.StudentTestAnswers.Where(r => r.StudentTestsId == i.StudentTestsId && r.TestQuestionId == i.TestQuestionId);
                var stid = stansid.Count() > 0 ? stansid.FirstOrDefault().Id : 0;
                var studentanswer = con.StudentTestAnswers.Find(stid);
                var studenttest = con.StudentTests.Find(i.StudentTestsId);
                var studenttesteval = con.TestAnswersEvaluations.Where(r => r.StudentTestId == i.StudentTestsId && r.StudentTestAnswerId == i.StudentTestAnswerId).ToList();
                var studenttesteavlId = studenttesteval.Count() > 0 ? studenttesteval.FirstOrDefault().Id : 0;
                var totmark = con.TestTotalMarks.Where(r => r.StudentTestId == i.StudentTestsId).ToList();
                totmarkid = totmark.Count() > 0 ? totmark.FirstOrDefault().Id : 0;
                StudentTestId =Convert.ToInt32(i.StudentTestsId);
                // if (i.IsTestChecked == true)
                {
                    if (studenttesteavlId > 0)
                    {
                        var answerevaluations = con.TestAnswersEvaluations.Find(studenttesteavlId);
                        answerevaluations.CandidateId = checkedby;        //model.StudentTestUserId;
                        answerevaluations.StudentTestId = i.StudentTestsId;
                        answerevaluations.StudentTestAnswerId = i.StudentTestAnswerId;
                        answerevaluations.IsCorrect = i.IsCorrect;
                        if (i.IsCorrect == false)
                        {
                            answerevaluations.IsNegative = true;
                            answerevaluations.NegativeMarks = i.NegativeMarks;
                        }
                        else
                        {
                            answerevaluations.IsNegative = false;
                            answerevaluations.NegativeMarks = 0;
                        }
                        answerevaluations.MarksObtain = i.MarksObtained;
                        answerevaluations.MaxMarks = i.MaxMarks;

                        answerevaluations.CheckedBy = checkedby;
                        answerevaluations.CheckedOn = DateTime.UtcNow;
                        con.Entry(answerevaluations).State = EntityState.Modified;
                        con.SaveChanges();
                        //SuccessNotification("Record updated successfully", "Success");
                    }
                    else
                    {
                        answerevaluation.CandidateId = checkedby;     //model.StudentTestUserId;
                        answerevaluation.StudentTestId = i.StudentTestsId;
                        answerevaluation.StudentTestAnswerId = i.StudentTestAnswerId;
                        answerevaluation.IsCorrect = i.IsCorrect;
                        if (i.IsCorrect == false)
                        {
                            answerevaluation.IsNegative = true;
                            answerevaluation.NegativeMarks = i.NegativeMarks;
                        }
                        else
                        {
                            answerevaluation.IsNegative = false;
                            answerevaluation.NegativeMarks = 0;
                        }
                        answerevaluation.MarksObtain = i.MaxMarks;                 //i.MarksObtained;
                        answerevaluation.MaxMarks = i.MaxMarks;
                        answerevaluation.CheckedBy = checkedby;
                        answerevaluation.CheckedOn = DateTime.UtcNow;
                        con.TestAnswersEvaluations.Add(answerevaluation);
                        con.SaveChanges();
                    }
                    //decimal mrkObtd = i.MaxMarks;
                    studentanswer.MarksObtained = (decimal)i.MaxMarks;      //i.MarksObtained != 0 ? i.MarksObtained : i.MaxMarks;
                    studentanswer.IsNegative = i.IsNegative;
                    con.Entry(studentanswer).State = EntityState.Modified;
                    con.SaveChanges();

                    studenttest.IsTestChecked = true;
                    studenttest.TestCheckedBy = checkedby;
                    con.Entry(studenttest).State = EntityState.Modified;
                    con.SaveChanges();
                }
            }
            if (totmarkid > 0)
            {
                var totaltestmark = con.TestTotalMarks.Find(totmarkid);
                totaltestmark.TotalCorrectAns = totalcorrectans;
                totaltestmark.TotalWrongAns = totalnegativeans;
                totaltestmark.TotalObtainedMarks = totalCorrectMarks;
                totaltestmark.TotalNegativeMarks = totalNegativemarks;
                totaltestmark.MarksObtained = totalCorrectMarks - totalNegativemarks;
                totaltestmark.StudentTestId = StudentTestId;
                totaltestmark.Status = true;
                totaltestmark.Deleted = false;
                totaltestmark.CreatedBy = checkedby;
                totaltestmark.CreatedOn = DateTime.UtcNow;
                con.Entry(totaltestmark).State = EntityState.Modified;
                con.SaveChanges();
            }
            else
            {
                totaltestmarks.TotalCorrectAns = totalcorrectans;
                totaltestmarks.TotalWrongAns = totalnegativeans;
                totaltestmarks.TotalObtainedMarks = totalCorrectMarks;
                totaltestmarks.TotalNegativeMarks = totalNegativemarks;
                totaltestmarks.MarksObtained = totalCorrectMarks - totalNegativemarks;
                totaltestmarks.StudentTestId =Convert.ToInt32(model.AvailableAnswerEvaluation.FirstOrDefault().StudentTestsId);
                totaltestmarks.Status = true;
                totaltestmarks.Deleted = false;
                totaltestmarks.CreatedBy =Convert.ToInt32(answerevaluation.CheckedBy);
                totaltestmarks.CreatedOn = DateTime.UtcNow;
                con.TestTotalMarks.Add(totaltestmarks);
                con.SaveChanges();
            }
        }
    }
}