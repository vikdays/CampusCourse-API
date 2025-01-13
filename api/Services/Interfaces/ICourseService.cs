using api.Models.CampusCourse;
using api.Models.Notification;
using api.Models.Student;
using api.Models.Teacher;

namespace api.Services.Interfaces
{
    public interface ICourseService
    {
        public Task<List<CampusCoursePreviewModel>> CreateCampusCourse(Guid groupId, CreateCampusCourseModel createCampusCourseModel, string token, string userId);
        public Task DeleteCampusCourse(Guid id, string userId);
        public Task SignUpToCourse(Guid courseId, string userId);
        public Task<CampusCourseDetailsModel> EditCourseStatus(Guid id, string userId, EditCourseStatusModel editCourseStatusModel);
        public Task<CampusCourseDetailsModel> CreateCourseNotification(Guid id, string userId, AddCampusCourseNotificationModel addCampusCourseNotificationModel);
        public Task<CampusCourseDetailsModel> GetCourseDetails(Guid courseId, string userId);
        public Task<CampusCourseDetailsModel> EditStudentStatus(Guid courseId, string userId, Guid studentId, EditCourseStudentStatusModel editCourseStudentStatusModel);
        public Task<CampusCourseDetailsModel> EditStudentMark(Guid courseId, string userId, Guid studentId, EditCourseStudentMarkModel editCourseStudentMarkModel);
        public Task<CampusCourseDetailsModel> AddTeacherToCourse(Guid courseId, string userId, AddTeacherToCourseModel addTeacherToCourseModel);
        public Task<List<CampusCoursePreviewModel>> GetMyCourses(string userId);
        public Task<List<CampusCoursePreviewModel>> GetTeachingCourses(string userId);
        //public Task<CampusCourseDetailsModel> EditCourse(Guid courseId, string userId, EditCampusCourseModel editCampusCourseModel);
    }
}
