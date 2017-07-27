using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;

namespace tys361.WCF.ViewModel
{
    public partial class TestEvaluationModel
    {
        public TestEvaluationModel()
        {
            AvailableQuestionPattern = new List<SelectListItem>();
            AvailableTopic = new List<SelectListItem>();
            AvailableCategory = new List<SelectListItem>();
            AvailableTest = new List<SelectListItem>();
            AvailableTeacherEvaluation = new List<TestEvaluationModel>();
            AvailableAnswerEvaluation = new List<TestEvaluationModel>();
        }

        public int userid { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int TopicId { get; set; }
        public string TopicName { get; set; }
        public int QuestionPatternId { get; set; }
        public string QuestionPatternName { get; set; }
        public int UserDetailsId { get; set; }
        public string Username { get; set; }
        public int ?StudentTestsId { get; set; }
        public int ?TestQuestionId { get; set; }
        public string Question { get; set; }
        public int ?MCQAnswersId { get; set; }
        public string MCQAnswer { get; set; }
        public string Answers { get; set; }
        public string Remark { get; set; }
        public bool IsTopicWise { get; set; }
        public bool IsSkip { get; set; }
        public decimal ?MarksObtained { get; set; }
        public bool IsNegative { get; set; }
        public DateTime Date { get; set; }
        public bool IsTestChecked { get; set; }
        public int TestCheckedBy { get; set; }
        public int StudentTestAnswerId { get; set; }
        public int IsCorrectMCQAnswerId { get; set; }
        public string IsCorrectMCQAnswer { get; set; }
        public bool ?IsCorrect { get; set; }
        public int TotalAttempt { get; set; }
        public int StudentTestUserId { get; set; }
        public string StudentTestUsername { get; set; }
        public decimal ?MaxMarks { get; set; }
        public decimal ?NegativeMarks { get; set; }
        public string Refno { get; set; }
        public bool IsQuestions { get; set; }
        public bool IsExpertise { get; set; }
        public string message { get; set; }
        public bool status { get; set; }


        /// <summary>
        /// Test Solution
        /// </summary>
        public string HintQuestion { get; set; }
        public string ModelAnswer { get; set; }
        public string VideoLinkSol { get; set; }
        public string WrittenSol { get; set; }
        public string QueryToexpert { get; set; }
        public string RemarkToexpert { get; set; }

        public IList<SelectListItem> AvailableQuestionPattern { get; set; }
        public IList<SelectListItem> AvailableTopic { get; set; }
        public IList<SelectListItem> AvailableCategory { get; set; }
        public IList<SelectListItem> AvailableTest { get; set; }
        public IList<TestEvaluationModel> AvailableTeacherEvaluation { get; set; }
        public IList<TestEvaluationModel> AvailableAnswerEvaluation { get; set; }

    }
}