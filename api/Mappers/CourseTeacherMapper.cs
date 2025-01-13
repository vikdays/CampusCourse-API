using api.Entities;

namespace api.Mappers
{
    public abstract class CourseTeacherMapper
    {
        public static CampusCourseTeacher  MapUserToTeacher(User user, CampusCourse campusCourse)
        {
            return new CampusCourseTeacher
            {
                UserId = user.Id,
                CampusCourseId = campusCourse.Id,
                IsMain = true
            };
        }
        public static CampusCourseTeacher MapUserToNotMainTeacher(User user, CampusCourse campusCourse)
        {
            return new CampusCourseTeacher
            {
                UserId = user.Id,
                CampusCourseId = campusCourse.Id,
                IsMain = false
            };
        }
    }
}
