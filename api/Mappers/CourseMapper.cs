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
                RemainingSlotsCount = campusCourse.MaximumStudentsCount - campusCourse.Students.Count(s => s.Status == StudentStatuses.Accepted),
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
                StudentsEnrolledCount = campusCourse.Students.Count(s => s.Status == StudentStatuses.Accepted),
                Requirements = campusCourse.Requirements,
                Annotations = campusCourse.Annotation,
                Semester = campusCourse.Semester,
                Status = campusCourse.Status,
                Students = campusCourse.Students.Select(student => new CampusCourseStudentModel
                {
                    Id = student.UserId,
                    Name = student.User.Name,
                    Email = student.User.Email,
                    Status = student.Status,
                    MidtermResult = student.MidtermResult,
                    FinalResult = student.FinalResult
                }).ToList(),
                Teachers = campusCourse.Teachers.Select(teacher => new CampusCourseTeacherModel
                {
                    Name = teacher.User.Name,
                    Email = teacher.User.Email,
                    IsMain = teacher.IsMain
                }).ToList(),
                Notifications = campusCourse.Notifications.Select(notification => new CampusCourseNotificationModel
                {
                    Text = notification.Text,
                    IsImportant = notification.isImportant
                }).ToList()
            };
        }

        public static CampusCourse MapFromEditCampusCourseModelToCampusCourse(Guid courseId, EditCampusCourseModel editCampusCourseModel, CampusCourse campusCourse)
        {
            
            {
                campusCourse.Id = courseId;
                campusCourse.Name = editCampusCourseModel.Name;
                campusCourse.StartYear = editCampusCourseModel.StartYear;
                campusCourse.MaximumStudentsCount = editCampusCourseModel.MaximumStudentsCount;
                campusCourse.Requirements = editCampusCourseModel.Requirements;
                campusCourse.Annotation = editCampusCourseModel.Annotations;
                campusCourse.Semester = editCampusCourseModel.Semester;
                campusCourse.Status = CourseStatuses.Created;
                campusCourse.CampusGroupId = campusCourse.CampusGroupId;

            };
            return campusCourse;
        }


    }
        
}
