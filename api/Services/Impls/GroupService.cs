using api.Exceptions;
using api.Mappers;
using api.Models.CampusGroup;
using api.Services.Interfaces;

namespace api.Services.Impls
{
    public class GroupService : IGroupService
    {
        private readonly DataContext _db;
        private readonly ITokenService _tokenService;
        private readonly IAccountService _accountService;
        public GroupService(DataContext db, ITokenService tokenService, IAccountService accountService)
        {
            _db = db;
            _tokenService = tokenService;
            _accountService = accountService;
        }

        public async Task<CampusGroupModel> CreateCampusGroup(CreateCampusGroupModel createCampusGroupModel, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
            }

            if (await _tokenService.IsTokenBanned(token))
            {
                throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
            }
            var user = await _accountService.GetUserByToken(token);
            var group = GroupMapper.MapFromCreateCampusGroupModelToCampusGroup(createCampusGroupModel);
            await _db.CampusGroups.AddAsync(group);
            await _db.SaveChangesAsync();
            return GroupMapper.MapFromCampusGroupToCampusGroupModel(group);
        }

        public async Task<List<CampusGroupModel>> GetCampusGroup(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
            }

            if (await _tokenService.IsTokenBanned(token))
            {
                throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
            }
            var groups = _db.CampusGroups.Select(g => GroupMapper.MapFromCampusGroupToCampusGroupModel(g)).ToList();
            return groups;
        }
    }
}
