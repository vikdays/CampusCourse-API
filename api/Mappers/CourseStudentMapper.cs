using api.Entities;
using api.Models.Enums;

namespace api.Mappers
{
    public abstract class CourseStudentMapper
    {
        public static CampusCourseStudent MapUserToStudent(User user, CampusCourse campusCourse)
        {
            return new CampusCourseStudent
            {
                UserId = user.Id,
                CampusCourseId = campusCourse.Id,
                Name = user.Name,
                Email = user.Email,
                Status = StudentStatuses.InQueue
            };
        }
    }
}
