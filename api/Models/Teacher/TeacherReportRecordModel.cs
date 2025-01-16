using api.Models.CampusGroup;

namespace api.Models.Teacher
{
    public class TeacherReportRecordModel
    {
        public string FullName { get; set; }
        public Guid Id { get; set; }
        public List<CampusGroupReportModel>  CampusGroupReports { get; set; }
    }
}
