using api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace api.Models.CampusCourse
{
    public class CampusCoursePreviewModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public int StartYear { get; set; }

        public int MaximumStudentsCount { get; set; }

        public int RemainingSlotsCount { get; set; }

        public CourseStatuses Status { get; set; }

        public Semesters Semester { get; set; }
    }
}
