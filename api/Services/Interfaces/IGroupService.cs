using api.Models.CampusCourse;
using api.Models.CampusGroup;

namespace api.Services.Interfaces
{
    public interface IGroupService
    {
        public Task<CampusGroupModel> CreateCampusGroup(CreateCampusGroupModel createCampusGroupModel, string token, string userId);
        public Task<List<CampusGroupModel>> GetCampusGroups(string token);
        public Task<List<CampusCoursePreviewModel>> GetCampusGroup(Guid groupId, string userId);
        public Task<CampusGroupModel> EditCampusCourse(EditCampusGroupModel editCampusGroupModel, Guid groupId, string token, string userId);
        public Task DeleteCampusGroup(Guid groupId, string userId);

    }
}
