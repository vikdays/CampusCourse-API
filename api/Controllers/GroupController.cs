using api.Models.CampusGroup;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using api.Exceptions;
using System.Security.Claims;

namespace api.Controllers
{
    [ApiController]
    [Route("groups")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly ITokenService _tokenService;

        public GroupController(IGroupService groupService, ITokenService tokenService)
        {
            _groupService = groupService;
            _tokenService = tokenService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCampusGroup(CreateCampusGroupModel createCampusGroupModel)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var token = _tokenService.ExtractTokenFromHeader(authorizationHeader);
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;
            return Ok(await _groupService.CreateCampusGroup(createCampusGroupModel, token, userId));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCampusGroup()
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var token = _tokenService.ExtractTokenFromHeader(authorizationHeader);
            return Ok(await _groupService.GetCampusGroup(token));
        }

        [HttpPut("groupe/{id}")]
        [Authorize]
        public async Task<IActionResult> EditGroup([FromRoute] Guid id, [FromBody] EditCampusGroupModel editCampusGroupModel)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var token = _tokenService.ExtractTokenFromHeader(authorizationHeader);
            if (await _tokenService.IsTokenBanned(token)) throw new CustomException("Token is banned", 401);
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;
            return Ok(await _groupService.EditCampusCourse(editCampusGroupModel, id, token, userId));
        }

        [HttpDelete("group/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment([FromRoute] Guid id)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var token = _tokenService.ExtractTokenFromHeader(authorizationHeader);
            if (await _tokenService.IsTokenBanned(token)) throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;
            await _groupService.DeleteCampusGroup(id, userId);
            return Ok();
        }
    }
}
