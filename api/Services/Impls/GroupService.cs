using api.Services.Interfaces;

namespace api.Services.Impls
{
    public class GroupService : IGroupService
    {
        private readonly DataContext _db;
        private readonly ITokenService _tokenService;
        public GroupService(DataContext db, ITokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }
    }
}
