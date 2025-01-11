using api.Exceptions;
using api.Mappers;
using api.Models.CampusCourse;
using api.Models.CampusGroup;
using api.Models.Enums;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Services.Impls
{
    public class CourseService : ICourseService
    {
        private readonly DataContext _db;
        private readonly IAccountService _accountService;
        public CourseService(DataContext db, IAccountService accountService)
        {
            _db = db;
            _accountService = accountService;
        }

        public async Task<List<CampusCoursePreviewModel>> CreateCampusCourse(Guid groupId, CreateCampusCourseModel createCampusCourseModel, string token, string userId)
        {
            var group = await _db.CampusGroups.FindAsync(groupId);
            if (group == null) throw new NotFoundException(ErrorConstants.NotFoundGroupError);
            var user = await _accountService.GetUserByToken(token);
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id);
            if (role == null || !role.IsAdmin)
            {
                throw new ForbiddenException(ErrorConstants.ForbiddenError);
            }
            var course = CourseMapper.MapFromCreateCampusCourseModelToCampusCourse(groupId, createCampusCourseModel);
            var mainTeacher = await _db.Users.FirstOrDefaultAsync(u => u.Id == createCampusCourseModel.MainTeacherId);
            if (mainTeacher == null)
            {
                throw new NotFoundException($"Teacher with ID {createCampusCourseModel.MainTeacherId} not found.");
            }
            var courseTeacher = CourseTeacherMapper.MapUserToTeacher(mainTeacher, course);
            await _db.Courses.AddAsync(course);
            await _db.Teachers.AddAsync(courseTeacher);
            await _db.SaveChangesAsync();
            var courses = _db.Courses.Select(course => CourseMapper.MapFromCampusCourseToCampusCoursePreviewModel(course)).ToList();
            return courses;
        }

        public async Task DeleteCampusCourse(Guid id, string userId)
        {
            var user = await _accountService.GetUserById(userId);
            var course = await _db.Courses.FindAsync(id);
            if (course == null) throw new NotFoundException(ErrorConstants.NotFoundCourseError);
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id);
            if (role == null || !role.IsAdmin)
            {
                throw new ForbiddenException(ErrorConstants.ForbiddenError);
            }
            _db.Remove(course);
            await _db.SaveChangesAsync();
        }

        public async Task SignUpToCourse(Guid courseId, string userId)
        {
            var course = await _db.Courses.FindAsync(courseId);
            if (course == null) throw new NotFoundException(ErrorConstants.NotFoundCourseError);
            var user = await _accountService.GetUserById(userId);
            var student = await _db.Students.FirstOrDefaultAsync(s => s.UserId == user.Id && s.CampusCourseId == courseId);
            if (!(student == null))
            { 
                throw new BadRequestException(ErrorConstants.SignedUpError);
            }
            Console.WriteLine(course.Status.ToString(), course.Status.GetType().Name);
            if (!(course.Status == CourseStatuses.OpenForAssigning) || (course.RemainingSlotsCount == 0))
            {
                throw new BadRequestException(ErrorConstants.ClosedCourse);
            }
            var courseStudent = CourseStudentMapper.MapUserToStudent(user, course);
            await _db.Students.AddAsync(courseStudent);
            await _db.SaveChangesAsync();
        }
    }
}