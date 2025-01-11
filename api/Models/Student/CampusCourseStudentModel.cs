using api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace api.Models.Student
{
    public class CampusCourseStudentModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = ErrorConstants.EmailNotValid)]
        public string? Email { get; set; }

        public StudentStatuses Status { get; set; }

        public StudentMarks MidtermResult { get; set; }

        public StudentMarks FinalResult { get; set; }
    }
}
