using api.Entities;
using api.Exceptions;
using api.Mappers;
using api.Models.CampusCourse;
using api.Models.CampusGroup;
using api.Models.Enums;
using api.Models.Notification;
using api.Models.Student;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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
            if (createCampusCourseModel == null)
            {
                throw new BadRequestException(ErrorConstants.EmtyBodyError);
            }
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
            var course = await _db.Courses.Include(c => c.Students).ThenInclude(s => s.User).Include(c => c.Teachers).Include(c => c.Notifications).FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null) throw new NotFoundException(ErrorConstants.NotFoundCourseError);
            var user = await _accountService.GetUserById(userId);
            var student = course.Students.FirstOrDefault(s => s.UserId == user.Id);
            if (!(student == null))
            { 
                throw new BadRequestException(ErrorConstants.SignedUpError);
            }
            if (!(course.Status == CourseStatuses.OpenForAssigning) || ((course.MaximumStudentsCount - course.Students.Count(s => s.Status == StudentStatuses.Accepted)) == 0))
            {
                throw new BadRequestException(ErrorConstants.ClosedCourse);
            }
            var courseStudent = CourseStudentMapper.MapUserToStudent(user, course);
            await _db.Students.AddAsync(courseStudent);
            await _db.SaveChangesAsync();
        }

        public async Task<CampusCourseDetailsModel> EditCourseStatus(Guid id, string userId, EditCourseStatusModel editCourseStatusModel)
        {
            if (editCourseStatusModel == null)
            {
                throw new BadRequestException(ErrorConstants.EmtyBodyError);
            }
            var user = await _accountService.GetUserById(userId);
            var course = await _db.Courses.Include(c => c.Students).ThenInclude(s => s.User).Include(c => c.Teachers).Include(c => c.Notifications).FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) throw new NotFoundException(ErrorConstants.NotFoundCourseError);
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id);
            var teacher = course.Teachers.FirstOrDefault(t => t.UserId == user.Id);
            if (role == null || (!role.IsAdmin && teacher == null))
            {
                throw new ForbiddenException(ErrorConstants.ForbiddenError);
            }
            course.Status = editCourseStatusModel.Status;
            _db.Courses.Update(course);
            await _db.SaveChangesAsync();
            return CourseMapper.MapFromCampusCourseToCampusCourseDetailsModel(course);
        }

        public async Task<CampusCourseDetailsModel> CreateCourseNotification(Guid id, string userId, AddCampusCourseNotificationModel addCampusCourseNotificationModel)
        {
            if (addCampusCourseNotificationModel == null)
            {
                throw new BadRequestException(ErrorConstants.EmtyBodyError);
            }
            var user = await _accountService.GetUserById(userId);
            var course = await _db.Courses.Include(c => c.Students).ThenInclude(s => s.User).Include(c => c.Teachers).Include(c => c.Notifications).FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) throw new NotFoundException(ErrorConstants.NotFoundCourseError);
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id);
            var teacher = course.Teachers.FirstOrDefault(t => t.UserId == user.Id);
            if (role == null || (!role.IsAdmin && teacher == null))
            {
                throw new ForbiddenException(ErrorConstants.ForbiddenError);
            }
            var notification = CourseNotificationMapper.MapFromAddCampusCourseNotificationModelToNotification(addCampusCourseNotificationModel, course);
            await _db.Notifications.AddAsync(notification);
            await _db.SaveChangesAsync();
            return CourseMapper.MapFromCampusCourseToCampusCourseDetailsModel(course);
        }

        public async Task<CampusCourseDetailsModel> GetCourseDetails(Guid courseId, string userId)
        {
            var user = await _accountService.GetUserById(userId);
            var course = await _db.Courses.Include(c => c.Students).Include(c => c.Teachers).Include(c => c.Notifications).FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null) throw new NotFoundException(ErrorConstants.NotFoundCourseError);
            return CourseMapper.MapFromCampusCourseToCampusCourseDetailsModel(course);
        }

        public async Task<CampusCourseDetailsModel> EditStudentStatus(Guid courseId, string userId, Guid studentId, EditCourseStudentStatusModel editCourseStudentStatusModel)
        {
            if (editCourseStudentStatusModel == null)
            {
                throw new BadRequestException(ErrorConstants.EmtyBodyError);
            }
            var user = await _accountService.GetUserById(userId);
            var course = await _db.Courses.Include(c => c.Students).ThenInclude(s => s.User).Include(c => c.Teachers).Include(c => c.Notifications).FirstOrDefaultAsync(c => c.Id == courseId);
            var student = course.Students.FirstOrDefault(s => s.UserId == studentId);
            if (course == null) throw new NotFoundException(ErrorConstants.NotFoundCourseError);
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id);
            var teacher = course.Teachers.FirstOrDefault(t => t.UserId == user.Id);
            if (role == null || (!role.IsAdmin && teacher == null))
            {
                throw new ForbiddenException(ErrorConstants.ForbiddenError);
            }
            student.Status = editCourseStudentStatusModel.Status;
            _db.Students.Update(student);
            await _db.SaveChangesAsync();
            return CourseMapper.MapFromCampusCourseToCampusCourseDetailsModel(course);
        }
    }
}