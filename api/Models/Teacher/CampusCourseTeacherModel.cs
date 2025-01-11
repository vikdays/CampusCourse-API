using api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace api.Models.Teacher
{
    public class CampusCourseTeacherModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = ErrorConstants.EmailNotValid)]
        public string? Email { get; set; }

        public bool IsMain { get; set; } = true;
    }
}
