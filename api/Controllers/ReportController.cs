using api.Exceptions;
using api.Models.Enums;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace api.Controllers
{
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly ITokenService _tokenService;
        private readonly IReportService _reportService;

        public ReportController(IGroupService groupService, ITokenService tokenService, IReportService reportService)
        {
            _groupService = groupService;
            _tokenService = tokenService;
            _reportService = reportService;
        }

        [HttpGet("report")]
        [Authorize]
        public async Task<IActionResult> GetReport([FromQuery] Semesters? semester, [FromQuery] List<Guid>? campusGroupIds)
        {
                var authorizationHeader = Request.Headers["Authorization"].ToString();
                var token = _tokenService.ExtractTokenFromHeader(authorizationHeader);
                if (await _tokenService.IsTokenBanned(token)) throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
                var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;
                return Ok(await _reportService.GetReport(userId, semester, campusGroupIds));

            
        }
    }
}
