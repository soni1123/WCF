using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tys361.WCF.ViewModel
{
    public partial class DashBoardViewModel
    {
        public DashBoardViewModel()
        {

            pakages = new PackagesModel();
            StudentProgress = new StudentProgressModel();
        }
        public PackagesModel pakages { get; set; }
        public StudentProgressModel StudentProgress { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
        public bool is_buyed_enable { get; set; }
        public bool is_buy_first { get; set; }
        public List<SubjectSelection> subject_list = new List<SubjectSelection>();
    }

    public class SubjectSelection
    {
        public string subject_name { get; set; }
    }

    public class PackagesModel
    {
        public string Package { get; set; }
        public string color { get; set; }
        public int package_id { get; set; }
        public decimal amount { get; set; }
        public string valid_till { get; set; }
        public int remender_days { get; set; }
        public bool Is_upgrade { get; set; }
        public bool is_maximum_package { get; set; }


    }
    public partial class StudentProgressModel
    {
        public int totalcorrect_ans { get; set; }
        public int totalwrong_ans { get; set; }
        public Double speed { get; set; }
        public decimal accuracy { get; set; }
        public int time_practiced { get; set; }
        public decimal correctans_percentage { get; set; }
        public decimal wrongans_percentage { get; set; }
        public int test_schedule { get; set; }
        public int total_test { get; set; }
        public int total_attempt { get; set; }

    }

    public class StudentTestResultModel
    {
        public decimal marks_obtained { get; set; }
        public string topic_name { get; set; }
        public int total_attempt { get; set; }
        public decimal totalnegative_marks { get; set; }
        public decimal totalObtained_marks { get; set; }

    }


    public class DashBoardPassId
    {
        public int UserId { get; set; }
    }




}
