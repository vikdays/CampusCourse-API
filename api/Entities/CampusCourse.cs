using api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace api.Entities
{
    public class CampusCourse
    {
        [Key] public int Id { get; set; } 
        public string Name { get; set; }
        public int StartYear { get; set; }
        public int MaximumStudentsCount { get; set; }
        public string Requirements { get; set; }
        public string Annotation { get; set; }

        // Foreign Key and Navigation Property for Semester Enum
        public Semesters Semester { get; set; }

        // Foreign Key and Navigation Property for Status Enum
        public CourseStatuses Status { get; set; }

        // Relationships
       // public ICollection<CampusCourseStudent> Students { get; set; }
       // public ICollection<CampusCourseTeacher> Teachers { get; set; }
       // public ICollection<Notification> Notifications { get; set; }
    }
}
