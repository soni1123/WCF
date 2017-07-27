using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tys361.WCF.ViewModel
{

    public class ResultScheduleLiveTest
    {
        public ResultScheduleLiveTest()
        {
            scheduleLiveTest = new List<ScheduleLiveTest>();
        }

        public List<ScheduleLiveTest> scheduleLiveTest { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
    }

    public class ScheduleLiveTest
    {
        public int id { get; set; }
        public string pattern_name { get; set; }
        public string level_name { get; set; }
        public string  subject_name { get; set; }
        public string topic_name { get; set; }
        public string test_name { get; set; }
        public int? duration_minute { get; set; }

    }

    public class ScheduleTestpassIds
    {
        public int level_id { get; set; }
        public int category_id { get; set; }
        public int test_id { get; set; }
        public int topic_id { get; set; }
        public int category_test_id { get; set; }
        public string type { get; set; }
    }

    public class ScheduleLiveTests
    {
        public string message { get; set; }
        public bool status { get; set;  }
    }

    public class ScheduleTestgetValues
    {
        public string type { get; set; }
        public int user_id { get; set; }
        public int question_pattern_id { get; set; }
        public int test_id { get; set; }
        public string date { get; set; }
        public string time { get; set; }
    }

    public class ScheduleTestgetMockValues
    {
        public string type { get; set; }
        public int user_id { get; set; }
        public int question_pattern_id { get; set; }
        public int test_id { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public bool is_trial { get; set; }
    }
    //public class Levels
    //{
    //    public int id { get; set; }
    //    public string name { get; set; }
    //}

    //#region Get all subjects by levelid
    //public class Subjects
    //{
    //    public int id { get; set; }
    //    public string name { get; set; }
    //}
    //public class LevelforSubID
    //{
    //    public int level_id { get; set; }
    //}
    //public class Topics
    //{
    //    public int id { get; set; }
    //    public string name { get; set; }
    //}

    //public class TopicBycategory
    //{
    //    public int category_id { get; set; }
    //}

    //public class Tests
    //{
    //    public int id { get; set; }
    //    public string name { get; set; }
    //    public int? duration_minute { get; set; }
    //}
    //public class TestBytopicId
    //{
    //    public int test_id { get; set; }
    //    public int topic_id { get; set; }
    //}
    //#endregion



}