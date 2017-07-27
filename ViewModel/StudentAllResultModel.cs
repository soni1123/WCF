using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tys361.WCF.ViewModel
{
    public class StudentAllResultModel
    {
        public StudentAllResultModel()
        {
            //AvailablestudentResult = new List<StudentAllResultModel>();
        }
        public int id { get; set; }
        public int studenttest_id { get; set; }
        public int totalcorrect_ans { get; set; }
        public int totalwrong_ans { get; set; }
        public decimal marks_obtained { get; set; }
        public decimal totalnegative_marks { get; set; }
        public decimal totalobtained_marks { get; set; }
        public int total_attempt { get; set; }
        public string topic_name { get; set; }
        public string question_patternName { get; set; }
       // public string question { get; set; }
        public string created_on { get; set; }
       // public string TestOn { get; set; }
        //public IList<StudentAllResultModel> AvailablestudentResult { get; set; }
    }

    public class StudnetAllResult
    {
        public StudnetAllResult()
        {
            AvailablestudentResult = new List<StudentAllResultModel>();
        }
        public IList<StudentAllResultModel> AvailablestudentResult { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
    }
}