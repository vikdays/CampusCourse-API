using api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace api.Models.Student
{
    public class EditCourseStudentMarkModel
    {
        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        public MarkType MarkType { get; set; }

        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        public StudentMarks Marks { get; set; }
    }
}
