using System.ComponentModel.DataAnnotations;

namespace api.Models.CampusCourse
{
    public class EditCampusCourseRequirementsAndAnnotationModel
    {
        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = ErrorConstants.NameLengthError)]
        public string Requirements { get; set; }

        [Required(ErrorMessage = ErrorConstants.RequiredFieldError)]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = ErrorConstants.NameLengthError)]
        public string Annotations { get; set; }
    }
}
