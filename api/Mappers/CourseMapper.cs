using api.Entities;
using api.Models.CampusCourse;
using api.Models.Enums;
using api.Models.Notification;
using api.Models.Student;
using api.Models.Teacher;

namespace api.Mappers
{
    public abstract class CourseMapper
    {
        public static CampusCourse MapFromCreateCampusCourseModelToCampusCourse(Guid groupId, CreateCampusCourseModel createCampusCourseModel)
        {
            return new CampusCourse
            {
                Id = Guid.NewGuid(),
                CampusGroupId = groupId,
                Name = createCampusCourseModel.Name,
                StartYear = createCampusCourseModel.StartYear,
                MaximumStudentsCount = createCampusCourseModel.MaximumStudentsCount,
                RemainingSlotsCount = createCampusCourseModel.MaximumStudentsCount,
                Requirements = createCampusCourseModel.Requirements,
                Annotation = createCampusCourseModel.Annotations,
                Semester = createCampusCourseModel.Semester,
                Status = CourseStatuses.Created

            };
        }

        public static CampusCoursePreviewModel MapFromCampusCourseToCampusCoursePreviewModel(CampusCourse campusCourse)
        {
            return new CampusCoursePreviewModel
            {
                Id = campusCourse.Id,
                Name = campusCourse.Name,
                StartYear = campusCourse.StartYear,
                MaximumStudentsCount = campusCourse.MaximumStudentsCount,
                RemainingSlotsCount = campusCourse.RemainingSlotsCount,
                Semester = campusCourse.Semester,
                Status = campusCourse.Status

            };
        }
        public static CampusCourseDetailsModel MapFromCampusCourseToCampusCourseDetailsModel(CampusCourse campusCourse)
        {
            return new CampusCourseDetailsModel
            {
                Id = campusCourse.Id,
                Name = campusCourse.Name,
                StartYear = campusCourse.StartYear,
                MaximumStudentsCount = campusCourse.MaximumStudentsCount,
                StudentsInQueueCount = campusCourse.Students.Count(s => s.Status == StudentStatuses.InQueue),
                StudentsEnrolledCount = campusCourse.MaximumStudentsCount - campusCourse.Students.Count(s => s.Status == StudentStatuses.Accepted),
                Requirements = campusCourse.Requirements,
                Annotations = campusCourse.Annotation,
                Semester = campusCourse.Semester,
                Status = campusCourse.Status,
                Students = campusCourse.Students.Select(student => new CampusCourseStudentModel
                {
                    Id = student.UserId,
                    Name = student.Name,
                    Email = student.Email,
                    Status = student.Status,
                    MidtermResult = student.MidtermResult,
                    FinalResult = student.FinalResult
                }).ToList(),
                Teachers = campusCourse.Teachers.Select(teacher => new CampusCourseTeacherModel
                {
                    Name = teacher.Name,
                    Email = teacher.Email,
                    IsMain = teacher.IsMain
                }).ToList(),
                Notifications = campusCourse.Notifications.Select(notification => new CampusCourseNotificationModel
                {
                    Text = notification.Text,
                    IsImportant = notification.isImportant
                }).ToList()
            };
        }
    }
        
}
