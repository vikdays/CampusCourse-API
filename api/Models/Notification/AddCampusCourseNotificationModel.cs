using System.ComponentModel.DataAnnotations;

namespace api.Models.Notification
{
    public class AddCampusCourseNotificationModel
    {
        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = ErrorConstants.EmailLengthError)]
        public string Text { get; set; }

        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        public bool isImportant { get; set; }

    }
}
