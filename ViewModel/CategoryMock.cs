using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tys361.WCF.Model;

namespace tys361.WCF.ViewModel
{
    public class CategoryMock
    {
       // public IList<CategoryMock> schedule_mock_test { get; set; }
        public int id { get; set; }
        public int? test_id { get; set; }
        public string date { get; set; }
        //DisplayDate
        public string display_date { get; set; }
        public string type { get; set; }
        public string time { get; set; }
        public string topic_name { get; set; }
       // public int row_id { get; set; }
        //public string message { get; set; }
        //public bool status { get; set; }

    }
    public class ListMock
    {
        public IList<CategoryMock> schedule_mock_test { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
    }
    public class MockCat
    {
        public int id { get; set; }
        public bool is_topic { get; set; }
        public bool is_subject { get; set; }
    }
    public class StudentTests
    {
        public int Id { get; set; }
        public string Refno { get; set; }
        public int TestId { get; set; }
        public int UserDetailsId { get; set; }
        public string Date { get; set; }
        public bool IsTestChecked { get; set; }
        public int TestCheckedBy { get; set; }
        public bool Status { get; set; }
        public bool Deleted { get; set; }
        public string CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public int SiteId { get; set; }
        public int OrgId { get; set; }
        public bool IsMockTest { get; set; }
        public string message { get; set; }

    }
    public class TestContexts
    {
        public int UserId { get; set; }
        public List<OnlineCategoryWiseTestVw> questions { get; set; }
        public List<OnlineCategoryWiseTestVw> randomquestions { get; set; }
        public OnlineCategoryWiseTestVw testquestions { get; set; }
        public List<TestQuestionMock> questionsMk { get; set; }
        public int total_questions { get; set; }
        public static int qid { get; set; }
        public int Min { get; set; }
        public int studenttest_id { get; set; }
        public string event_date { get; set; }
        public int studentrevision_testid { get; set; }
        public ArrayList random_qid { get; set; }
        public static int index_id { get; set; }
    }
    public partial class TestEvaluationThanksModelForTestId
    {
        public int StudentTestId { get; set; }
        public int time { get; set; }

    }
    public class categorymockquestions
    {
        public string message { get; set; }
        public bool status { get; set; }
        public List<categorymockwise> ansque = new List<categorymockwise>();
        
    }
    public class categorymockwise
    {
       
        public int mcq_count { get; set; }      
        public string answer { get; set; }
        public string question { get; set; }
        public int  question_id { get; set; }
        //public string message { get; set; }
        public bool is_image { get; set; }
        public string refno { get; set; }  
        public string imageurl { get; set; }     
        public List<QMCQAnswers> available_mcqanswer = new List<QMCQAnswers>();
        //public List<TestQuestionMock> total_questions { get; set; }

    }
    public class Responsemodel
    {
        public string message { get; set; }
        public string refno { get; set; }
        public bool status { get; set; }
    }

    public class TestQuestionsListModel
    {
        public int duration { get; set; }
        public int studenttest_id { get; set; }
        public int category_id { get; set; }         
        public string message { get; set; }
        public bool status { get; set; }
        public List<categorymockwise> ansque = new List<categorymockwise>();
       // public  List<TestQuestionMock> total_questions { get; set; }
    }

    public class QSubmitedModel
    {
        public string quesread_time { get; set; }  // Q start time
        public string ansread_time { get; set; } // ans view time
        public string ans_submittime { get; set; } // ans submision time
        public int user_id { get; set; }    
        public int duration { get; set; }
        public int testquestionid { get; set; }
        public int studenttestid { get; set; }
        public decimal qmaxmarks { get; set; }
        public int mcq_answerid { get; set; }
        public int category_id { get; set; }
        //public IList<string> IsCorrects { get; set; }

    }
    public class QMCQAnswers
    {
        public int id { get; set; }
        public int testquestionmock_id { get; set; }
        public string answer { get; set; }
        public string image { get; set; }
        public bool is_image { get; set; }
        public bool is_correct { get; set; }
    }


    #region CategoryLive 
    public class LiveTestQuestionsListModel
    {
        public int duration { get; set; }
        public int studenttest_id { get; set; }
        public int category_id { get; set; }
        public int quid { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
        public List<LiveCategorywise> ansque = new List<LiveCategorywise>();
       // public List<TestQuestion> total_questions { get; set; }
    }

    public class LiveCategorywise
    {
        // public string Refno { get; set; }

        public int mcq_count { get; set; }
        public string answer { get; set; }
        public string question { get; set; }
        public int question_id { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
        public string refno { get; set; }
        public string imageurl { get; set; }
        public List<QMCQAnswers> available_mcqanswer = new List<QMCQAnswers>();
        //public List<TestQuestionMock> total_questions { get; set; }

    }

    #endregion

}