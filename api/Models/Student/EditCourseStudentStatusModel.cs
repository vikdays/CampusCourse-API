using api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace api.Models.Student
{
    public class EditCourseStudentStatusModel
    {
        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        public StudentStatuses Status { get; set; }
    }
}
