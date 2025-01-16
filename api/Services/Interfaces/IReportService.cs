using api.Models.Enums;
using api.Models.Teacher;

namespace api.Services.Interfaces
{
    public interface IReportService
    {
        public Task<List<TeacherReportRecordModel>>  GetReport(string userId, Semesters? semester, List<Guid> campusGroupIds);
    }
}
