using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using tys361.WCF.Model;
using System.ServiceModel.Web;
using System.Text;
using tys361.WCF.ViewModel;
using System.Threading.Tasks;


namespace tys361.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ItysService 
    {
        #region Register User

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetData", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        string GetData();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "MobileVerify", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        MobileVerificationMstModel MobileVerify(RegisterStudentModel regis);


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "MobileVerifyOTP", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        otp MobileVerifyOTP(otp otp);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "MobileVerifyresendOTP", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        resendotp MobileVerifyresendOTP(resendotp resendotp);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "set_password", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        pass set_password(setpassword set);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "change_password", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        pass change_password(changepassword set);

        #endregion

        #region Login User

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "LoginEmail", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        LoginEmailStudentModel LoginEmail(LoginEmailStudentModel log);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "LoginPassword", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Task<LoginPasswrdStudentModel> LoginPassword(LoginPasswrdStudentModel logpass);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "LoginOTP", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Task<LoginotpStudentModel> LoginOTP(LoginotpStudentModel logotp);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "ResendOtp", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        LoginOtpStudentModel ResendOtp(LoginOtpStudentModel otps);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "forgotPassword", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        pass forgotPassword(string email);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "setforgot_password", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        pass setforgot_password(forgotpass set);
        #endregion

        #region Category Mock 
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetCatMockData", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ListMock GetCatMockData();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetCatMockTYPData", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ListMock GetCatMockTYPData();
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetCategoryMockData", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        TestQuestionsListModel GetCategoryMockData(string id);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetCategoryTypeData", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        TestQuestionsListModel GetCategoryTypeData(string id);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SaveCategoryMockData", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Responsemodel SaveCategoryMockData(QSubmitedModel mod);

       
        #endregion

        #region Category Live
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetCatLiveData", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ListMock GetCatLiveData();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetCategoryLiveData", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        TestQuestionsListModel GetCategoryLiveData(string id);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SaveCategoryLiveData", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Responsemodel SaveCategoryLiveData(QSubmitedModel mod);
        #endregion

        #region Student Profile

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetProfile", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        GetUpdationModel GetProfile();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "ProfileUpdation", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ProfilesUpdation ProfileUpdation(ProfileUpdationModel profilemod);
        #endregion

        #region Student DashBoard 
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Dashboard", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        DashBoardViewModel Dashboard();
        #endregion

        #region profile Percentage
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "profilePer", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ProfilePercentageModel profilePer();
        //#region Category Mock 
        //[OperationContract]
        //[WebInvoke(Method = "GET", UriTemplate = "GetCatMockData", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        //CategoryMock GetCatMockData();

        //[OperationContract]
        //[WebInvoke(Method = "POST", UriTemplate = "GetCategoryMockData", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        // Task<StudentTests> GetCategoryMockData(string Id);
         #endregion


        #region All Marks Details
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StudentAllResult", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        StudnetAllResult StudentAllResult(int studenttestId1);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "StudentResultView", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        TestEvaluationModel StudentResultView(int StudentTestId);

        #endregion

        #region Schedule Live

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "QuestionPatterns", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ResultScheduleLiveTest QuestionPatterns(string type);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetLevels", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ResultScheduleLiveTest GetLevels(ScheduleTestpassIds categoryid);


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetSubjectByLevel", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ResultScheduleLiveTest GetSubjectByLevel(string type);


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetTopicByCategoryId", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ResultScheduleLiveTest GetTopicByCategoryId(ScheduleTestpassIds categoryid);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetTestByTopicId", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ResultScheduleLiveTest GetTestByTopicId(ScheduleTestpassIds topicId);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetTestBycategoryId", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ResultScheduleLiveTest GetTestBycategoryId(ScheduleTestpassIds testbysubid);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetDurationByTestId", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ResultScheduleLiveTest GetDurationByTestId(ScheduleTestpassIds testid);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "ScheduleLiveTest", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ScheduleLiveTests ScheduleLiveTest(ScheduleTestgetValues testdetails);

        #endregion

        #region Schedule Mock
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetMockTestByTopicId", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        TestBytopic GetMockTestByTopicId(int topic_id, string type);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetMockTestByCategoryId", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        TestByCategory GetMockTestByCategoryId(int category_id, string type);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "QuestionsetTypes", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Result QuestionsetTypes(string type);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "AvailableYears", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        YearResult AvailableYears(string type);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "JumbleTypes", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        JumbleResult JumbleTypes(string type);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetMockDurations", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        DurationResult GetMockDurations(string type);


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "ScheduleMockTest", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ScheduleMockTestModel ScheduleMockTest(ScheduleMockTestPassId MockValues);
        #endregion
        #region TYP-TEST

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "ScheduleMockTYPTest", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        ScheduleMockTestModel ScheduleMockTYPTest(ScheduleMockTYPTest MockValues);
        #endregion

        #region Package selection
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "classlist", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        packageModel classlist();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "packagedetails", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        PackageclassViewModel packagedetails(PackageclassModel class_id);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetClassDataUpgrade", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        PackageupgradeViewModel GetClassDataUpgrade();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "getdatadb", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        statuspkg getdatadb(PackagegetModel getdata);
        #endregion

    }

}
