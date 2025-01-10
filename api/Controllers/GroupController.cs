using api.Models.CampusGroup;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
            return Ok(await _groupService.CreateCampusGroup(createCampusGroupModel, token));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCampusGroup()
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var token = _tokenService.ExtractTokenFromHeader(authorizationHeader);
            return Ok(await _groupService.GetCampusGroup(token));
        }
    }
}
