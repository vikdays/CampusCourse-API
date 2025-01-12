using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ITokenService _tokenService;
    public AccountController(IAccountService accountService, ITokenService tokenService)
    {
        _accountService = accountService;
        _tokenService = tokenService;

    }
    [HttpPost("registration")]
    public async Task<IActionResult> Register(UserRegisterModel userRegisterModel)
    {
        var token = await _accountService.Register(userRegisterModel);
        return Ok(token);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginModel userLoginModel)
    {
        var token = await _accountService.Login(userLoginModel);
        return Ok(token);

    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var authorizationHeader = Request.Headers["Authorization"].ToString();
        var token = _tokenService.ExtractTokenFromHeader(authorizationHeader);


        return Ok(await _accountService.GetProfile(token));
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> EditProfile(UserProfileModel userProfileModel)
    {
        var authorizationHeader = Request.Headers["Authorization"].ToString();
        var token = _tokenService.ExtractTokenFromHeader(authorizationHeader);
        return Ok(await _accountService.EditProfile(userProfileModel, token));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        string token = HttpContext.Request.Headers["Authorization"];

        return Ok(await _accountService.Logout(token));
    }



}