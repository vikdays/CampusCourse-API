using api.Exceptions;
using api.Mappers;
using api.Models.CampusCourse;
using api.Models.CampusGroup;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

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
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id);
            if (role == null || !role.IsAdmin)
            {
                throw new ForbiddenException(ErrorConstants.ForbiddenError);
            }
            var group = GroupMapper.MapFromCreateCampusGroupModelToCampusGroup(createCampusGroupModel);
            await _db.CampusGroups.AddAsync(group);
            await _db.SaveChangesAsync();
            return GroupMapper.MapFromCampusGroupToCampusGroupModel(group);
        }

        public async Task<List<CampusGroupModel>> GetCampusGroups(string token)
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
            var user = await _accountService.GetUserByToken(token);
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id);
            if (role == null || !role.IsAdmin)
            {
                throw new ForbiddenException(ErrorConstants.ForbiddenError);
            }
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
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id);
            if (role == null || !role.IsAdmin)
            {
                throw new ForbiddenException(ErrorConstants.ForbiddenError);
            }
            _db.Remove(group);
            await _db.SaveChangesAsync();
        }

        public async Task<List<CampusCoursePreviewModel>> GetCampusGroup(Guid groupId, string userId)
        {
            var user = await _accountService.GetUserById(userId);
            var group = await _db.CampusGroups.FindAsync(groupId);
            if (group == null) throw new NotFoundException(ErrorConstants.NotFoundGroupError);
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id);
            if (role == null || !role.IsAdmin)
            {
                throw new ForbiddenException(ErrorConstants.ForbiddenError);
            }
            var course = await _db.Courses.FirstOrDefaultAsync(c => c.CampusGroupId == groupId);
            var courses = _db.Courses.Select(course => CourseMapper.MapFromCampusCourseToCampusCoursePreviewModel(course)).ToList();
            return courses;
        }
    }
}
