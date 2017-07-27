using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tys361.WCF.ViewModel
{
    public class ScheduleMockTestModel
    {
        public string message { get; set; }
        public bool status { get; set; }
    }

    public class ScheduleMockTestPassId
    {
        public string type { get; set; }
        public int user_id { get; set; }
        public int question_pattern_id { get; set; }
        public int test_id { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public int year { get; set; }
        public int from_year { get; set; }
        public int to_year { get; set; }
        public int question_set { get; set; }
        public int jumble_type { get; set; }
        public int duration_id { get; set; }
        public bool is_trial { get; set; }
    }

    public class ScheduleMockTYPTest
    {
        public string type { get; set; }
        public int user_id { get; set; }
        public int question_pattern_id { get; set; }
        public int test_id { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public int year { get; set; }
        public int duration_id { get; set; }
    }
    public class QuestionSet
    {
        public int id { get; set; }
        public string question_set_name { get; set; }

    }

    public class Year
    {
        public int id { get; set; }
        public int? year { get; set; }

    }
    public class JumbleSet
    {
        public int id { get; set; }
        public string type_name { get; set; }

    }

    public class MockTests
    {
        public int id { get; set; }
        public string test_name { get; set; }

    }

    public class MockCategorywiseTests
    {
        public int id { get; set; }
        public string test_name { get; set; }

    }

    public class Duration
    {
        public int id { get; set; }
        public int? duration { get; set; }

    }
    public class Result
    {
        public Result()
        {
            questionset = new List<QuestionSet>();
           
        }

        public List<QuestionSet> questionset { get; set; }
        
      
        public string message { get; set; }
        public bool status { get; set; }
    }

    public class YearResult
    {
        public YearResult()
        {
            Year = new List<Year>();
        }
        public List<Year> Year { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
    }

    public class JumbleResult
    {
        public JumbleResult()
        {
            jumbleType = new List<JumbleSet>();
        }
        public List<JumbleSet> jumbleType { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
    }

    public class TestBytopic
    {
        public TestBytopic()
        {
            topicwisetest = new List<MockTests>();
        }
        public List<MockTests> topicwisetest { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
    }

    public class TestByCategory
    {
        public TestByCategory()
        {
              subjectwisetest = new List<MockCategorywiseTests>();
        }
         public List<MockCategorywiseTests> subjectwisetest { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
    }

    public class DurationResult
    {
        public DurationResult()
        {
            durations = new List<Duration>();
        }
        public List<Duration> durations { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
    }
}