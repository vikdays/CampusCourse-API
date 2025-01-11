using api.Models.CampusCourse;

namespace api.Services.Interfaces
{
    public interface ICourseService
    {
        public Task<List<CampusCoursePreviewModel>> CreateCampusCourse(Guid groupId, CreateCampusCourseModel createCampusCourseModel, string token, string userId);
        public Task DeleteCampusCourse(Guid id, string userId);
        public Task SignUpToCourse(Guid courseId, string userId);
    }
}
