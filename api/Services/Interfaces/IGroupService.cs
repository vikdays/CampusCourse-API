using api.Models.CampusGroup;

namespace api.Services.Interfaces
{
    public interface IGroupService
    {
        public Task<CampusGroupModel> CreateCampusGroup(CreateCampusGroupModel createCampusGroupModel, string token, string userId);
        public Task<List<CampusGroupModel>> GetCampusGroup(string token);
        public Task<CampusGroupModel> EditCampusCourse(EditCampusGroupModel editCampusGroupModel, Guid groupId, string token, string userId);
        public Task DeleteCampusGroup(Guid groupId, string userId);

    }
}
