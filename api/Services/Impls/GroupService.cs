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

        public async Task<CampusGroupModel> CreateCampusGroup(CreateCampusGroupModel createCampusGroupModel, string token, string userId)
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

        public async Task<CampusGroupModel> EditCampusCourse(EditCampusGroupModel editCampusGroupModel, Guid groupId, string token, string userId)
        {
            var group = GroupMapper.MapFromEditCampusGroupModelToCampusGroup(editCampusGroupModel, groupId);
            _db.CampusGroups.Update(group);
            await _db.SaveChangesAsync();
            return GroupMapper.MapFromCampusGroupToCampusGroupModel(group);
        }

        public async Task DeleteCampusGroup(Guid id, string userId)
        {
            var user = await _accountService.GetUserById(userId);
            var group = await _db.CampusGroups.FindAsync(id);
            if (group == null) throw new NotFoundException(ErrorConstants.NotFoundGroupError);
            // if (user.Id != comment.AuthorId) throw new CustomException("You are not the author of this comment", 403);
            _db.Remove(group);
            await _db.SaveChangesAsync();
        }
    }
}
