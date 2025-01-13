using api.Services.Impls;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        public readonly IAccountService _accountService;
        public UserController(IUserService userService, IAccountService accountService, ITokenService tokenService)
        {
            _userService = userService;
            _accountService = accountService;
            _tokenService = tokenService;

        }

        [Authorize]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var token = _tokenService.ExtractTokenFromHeader(authorizationHeader);
            return Ok(await _userService.GetUsers(token));
        }
    }
}
