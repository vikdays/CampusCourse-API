using api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Entities
{
    [Table("Courses")]
    public class CampusCourse
    {
        [Key] public Guid Id { get; set; }
        public Guid CampusGroupId { get; set; } //внешний ключ
        public string Name { get; set; }
        public int StartYear { get; set; }
        public int MaximumStudentsCount { get; set; }
        public string Requirements { get; set; }
        public string Annotation { get; set; }

        public Semesters Semester { get; set; }

        public CourseStatuses Status { get; set; }
        public CampusGroup CampusGroup { get; set; } 

        public ICollection<CampusCourseStudent> Students { get; set; } = new List<CampusCourseStudent>(); 
        public ICollection<CampusCourseTeacher> Teachers { get; set; } = new List<CampusCourseTeacher>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
