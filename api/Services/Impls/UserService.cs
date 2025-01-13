using api.Exceptions;
using api.Mappers;
using api.Models.User;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Services.Impls
{
    public class UserService : IUserService
    {
        private readonly DataContext _db;
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;
        public UserService(DataContext db, ITokenService tokenService, IAccountService accountService)
        {
            _db = db;
            _tokenService = tokenService;
            _accountService = accountService;
        }

        public async Task<List<UserShortModel>> GetUsers(string token)
        {
            var user = await _accountService.GetUserByToken(token);
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id);
            if (role == null || !role.IsAdmin)
            {
                throw new ForbiddenException(ErrorConstants.ForbiddenError);
            }
            var users = _db.Users.Select(u => UserMapper.MapFromEntityToUserShortModel(u)).ToList();
            return users;
        }

    }
}
