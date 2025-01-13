using api.Models.User;

namespace api.Services.Interfaces
{
    public interface IUserService
    {
        public Task<List<UserShortModel>> GetUsers(string token);
        public Task<UserRolesModel> GetRoles(string token);
    }
}
