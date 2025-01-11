using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Entities
{
    [Table("Teachers")]
    public class CampusCourseTeacher
    {
        [Key] public Guid UserId { get; set; }
        public Guid CampusCourseId { get; set; } 

        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = ErrorConstants.EmailNotValid)]
        public string? Email { get; set; }

        public bool IsMain { get; set; } = true;

        public User User { get; set; } 
        public CampusCourse CampusCourse { get; set; }
    }
}
