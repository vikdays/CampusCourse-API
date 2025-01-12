using api.Entities;
using api.Models.CampusCourse;
using api.Models.Notification;

namespace api.Mappers
{
    public abstract class CourseNotificationMapper
    {
        public static Notification MapFromAddCampusCourseNotificationModelToNotification(AddCampusCourseNotificationModel addCampusCourseNotificationModel, CampusCourse campusCourse)
        {
            return new Notification
            {
                Id = Guid.NewGuid(),
                CampusCourseId = campusCourse.Id,
                Text = addCampusCourseNotificationModel.Text,
                isImportant = addCampusCourseNotificationModel.IsImportant
            };
        }
    }
}
