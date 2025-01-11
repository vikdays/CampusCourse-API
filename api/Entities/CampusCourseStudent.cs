using api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Entities
{
    [Table("Students")]
    public class CampusCourseStudent
    {
        [Key] public Guid UserId { get; set; }

        public Guid CampusCourseId { get; set; } //вненшний  ключ

        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = ErrorConstants.EmailNotValid)]
        public string? Email { get; set; }

        public StudentStatuses Status { get; set; }

        public StudentMarks MidtermResult { get; set; }

        public StudentMarks FinalResult { get; set; }

        public User User { get; set; } 
        public CampusCourse CampusCourse { get; set; }
    }
}
