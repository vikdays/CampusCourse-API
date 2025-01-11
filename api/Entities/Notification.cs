using System.ComponentModel.DataAnnotations;

namespace api.Entities
{
    public class Notification
    {
        [Key] public Guid Id { get; set; }
        public Guid CampusCourseId { get; set; }
        
        public string Text { get; set; }
        public bool isImportant { get; set; }

        public CampusCourse CampusCourse { get; set; }
    }
}
