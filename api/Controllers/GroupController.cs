using api.Models.CampusGroup;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("groups")]
    public class GroupController
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;

        }

        [HttpPost]
        public async Task<IActionResult> CreateCampusGroup(CreateCampusGroupModel createCampusGroupModel)
        {
            string token = HttpContext.Request.Headers["Authorization"];
            return Ok(await _accountService.Logout(token));
        }
    }
}
