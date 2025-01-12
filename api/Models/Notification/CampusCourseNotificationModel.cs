using System.ComponentModel.DataAnnotations;

namespace api.Models.Notification
{
    public class CampusCourseNotificationModel
    {
        public string? Text { get; set; }
        public bool IsImportant { get; set; }
    }
}
