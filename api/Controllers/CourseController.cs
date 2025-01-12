using api.Exceptions;
using api.Models.CampusCourse;
using api.Models.CampusGroup;
using api.Models.Notification;
using api.Models.Student;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace api.Controllers
{
    [ApiController]
    public class CourseController : ControllerBase

    {
        private readonly IGroupService _groupService;
        private readonly ITokenService _tokenService;
        private readonly ICourseService _courseService;

        public CourseController(IGroupService groupService, ITokenService tokenService, ICourseService courseService)
        {
            _groupService = groupService;
            _tokenService = tokenService;
            _courseService = courseService;
        }

        [Authorize]
        [HttpPost("groups/{groupId}")]
        public async Task<IActionResult> CreateCampusCourse([FromRoute] Guid groupId, CreateCampusCourseModel createCampusCourseModel)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var token = _tokenService.ExtractTokenFromHeader(authorizationHeader);
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;
            return Ok(await _courseService.CreateCampusCourse(groupId, createCampusCourseModel, token, userId));
        }

        [HttpDelete("courses/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCourse([FromRoute] Guid id)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var token = _tokenService.ExtractTokenFromHeader(authorizationHeader);
            if (await _tokenService.IsTokenBanned(token)) throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;
            await _courseService.DeleteCampusCourse(id, userId);
            return Ok();
        }

        [HttpPost("courses/{id}/sign-up")]
        [Authorize]
        public async Task<IActionResult> SignUpToCourse([FromRoute] Guid id)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var token = _tokenService.ExtractTokenFromHeader(authorizationHeader);
            if (await _tokenService.IsTokenBanned(token)) throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;
            await _courseService.SignUpToCourse(id, userId);
            return Ok();
        }

        [HttpPost("courses/{id}/status")]
        [Authorize]
        public async Task<IActionResult> EditCourseStatus([FromRoute] Guid id, EditCourseStatusModel editCourseStatusModel)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var token = _tokenService.ExtractTokenFromHeader(authorizationHeader);
            if (await _tokenService.IsTokenBanned(token)) throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;
            return Ok(await _courseService.EditCourseStatus(id, userId, editCourseStatusModel));
        }

        [HttpPost("courses/{id}/notifications")]
        [Authorize]
        public async Task<IActionResult> CreateCourseNotification([FromRoute] Guid id, AddCampusCourseNotificationModel addCampusCourseNotificationModel)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var token = _tokenService.ExtractTokenFromHeader(authorizationHeader);
            if (await _tokenService.IsTokenBanned(token)) throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;
            return Ok(await _courseService.CreateCourseNotification(id, userId, addCampusCourseNotificationModel));
        }

        [HttpGet("courses/{id}/details")]
        [Authorize]
        public async Task<IActionResult> GetCourseDetails([FromRoute] Guid id)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var token = _tokenService.ExtractTokenFromHeader(authorizationHeader);
            if (await _tokenService.IsTokenBanned(token)) throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;
            return Ok(await _courseService.GetCourseDetails(id, userId));
        }

        [HttpPost("courses/{id}/student-status/{studentId}")]
        [Authorize]
        public async Task<IActionResult> EditStudentStatus([FromRoute] Guid id, [FromRoute] Guid studentId, EditCourseStudentStatusModel editCourseStudentStatusModel)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var token = _tokenService.ExtractTokenFromHeader(authorizationHeader);
            if (await _tokenService.IsTokenBanned(token)) throw new UnauthorizedException(ErrorConstants.UnauthorizedError);
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;
            return Ok(await _courseService.EditStudentStatus(id, userId, studentId, editCourseStudentStatusModel));
        }
    }
}
