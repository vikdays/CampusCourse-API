using api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace api.Models.CampusCourse
{
    public class EditCampusCourseModel
    {

        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = ErrorConstants.NameLengthError)]
        public string Name { get; set; }

        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        [Range(2000, 2029, ErrorMessage = ErrorConstants.StartYearError)]
        public int StartYear { get; set; }

        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        [Range(1, 200, ErrorMessage = ErrorConstants.MaximumStudentCount)]
        public int MaximumStudentsCount { get; set; }

        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        public Semesters Semester { get; set; }

        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = ErrorConstants.EmailLengthError)]
        public string Requirements { get; set; }

        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = ErrorConstants.PasswordLengthError)]
        public string Annotations { get; set; }

        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        public Guid MainTeacherId { get; set; }
    }
}
