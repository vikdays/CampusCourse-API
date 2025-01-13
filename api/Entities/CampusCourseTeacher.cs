using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Entities
{
    [Table("Teachers")]
    public class CampusCourseTeacher
    {
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public User User { get; set; }

        [ForeignKey(nameof(CampusCourse))]
        public Guid CampusCourseId { get; set; }
        public CampusCourse CampusCourse { get; set; }

        public bool IsMain { get; set; } = true;
    }
}
