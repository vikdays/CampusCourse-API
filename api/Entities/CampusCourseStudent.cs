using api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Entities
{
    [Table("Students")]
    public class CampusCourseStudent
    {
        [ForeignKey(nameof(CampusCourse))]
        public Guid CampusCourseId { get; set; } 
        public CampusCourse CampusCourse { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; } 
        public User User { get; set; }

        public StudentStatuses Status { get; set; }

        public StudentMarks? MidtermResult { get; set; }

        public StudentMarks? FinalResult { get; set; }

        public bool IsSent { get; set; }
    }
}
