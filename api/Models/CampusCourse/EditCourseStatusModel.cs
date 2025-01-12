using api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace api.Models.CampusCourse
{
    public class EditCourseStatusModel
    {
        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        public CourseStatuses Status { get; set; }
    }
}
