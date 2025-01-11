using api.Entities;
using api.Models.CampusCourse;
using api.Models.Enums;

namespace api.Mappers
{
    public abstract class CourseMapper
    {
        public static CampusCourse MapFromCreateCampusCourseModelToCampusCourse(Guid groupId, CreateCampusCourseModel createCampusCourseModel)
        {
            return new CampusCourse
            {
                Id = Guid.NewGuid(),
                CampusGroupId = groupId,
                Name = createCampusCourseModel.Name,
                StartYear = createCampusCourseModel.StartYear,
                MaximumStudentsCount = createCampusCourseModel.MaximumStudentsCount,
                RemainingSlotsCount = createCampusCourseModel.MaximumStudentsCount,
                Requirements = createCampusCourseModel.Requirements,
                Annotation = createCampusCourseModel.Annotations,
                Semester = createCampusCourseModel.Semester,
                Status = CourseStatuses.Created

            };
        }

        public static CampusCoursePreviewModel MapFromCampusCourseToCampusCoursePreviewModel(CampusCourse campusCourse)
        {
            return new CampusCoursePreviewModel
            {
                Id = campusCourse.Id,
                Name = campusCourse.Name,
                StartYear = campusCourse.StartYear,
                MaximumStudentsCount = campusCourse.MaximumStudentsCount,
                RemainingSlotsCount = campusCourse.RemainingSlotsCount,
                Semester = campusCourse.Semester,
                Status = campusCourse.Status

            };
        }
    }
        
}
