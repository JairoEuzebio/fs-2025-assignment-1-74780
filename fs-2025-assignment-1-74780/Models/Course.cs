namespace fs_2025_assignment_1_74780.Models
{

    public class CourseModel
    {
        public int id { get; set; }
        public bool isPreorder { get; set; }
        public string courseUrl { get; set; }
        public string courseType { get; set; }
        public string courseName { get; set; }
        public int courseLessonCount { get; set; }
        public decimal courseLengthInHours { get; set; }
        public string shortDescription { get; set; }
        public string courseImage { get; set; }
        public int priceInUSD { get; set; }
        public string coursePreviewLink { get; set; }
    }

}
