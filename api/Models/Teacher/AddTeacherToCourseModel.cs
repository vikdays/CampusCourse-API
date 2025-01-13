using System.ComponentModel.DataAnnotations;

namespace api.Models.Teacher
{
    public class AddTeacherToCourseModel
    {
        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        public Guid Id { get; set; }
    }
}
