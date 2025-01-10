using api.Models.CampusGroup;

namespace api.Services.Interfaces
{
    public interface IGroupService
    {
        public Task<CampusGroupModel> CreateCampusGroup(CreateCampusGroupModel createCampusGroupModel, string token);
        public Task<List<CampusGroupModel>> GetCampusGroup(string token);

    }
}
