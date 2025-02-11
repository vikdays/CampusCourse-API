﻿using api.Entities;
using api.Exceptions;
using api.Mappers;
using api.Models.CampusCourse;
using api.Models.CampusGroup;
using api.Models.Enums;
using api.Models.Notification;
using api.Models.Student;
using api.Models.Teacher;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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
            var courses = await _db.Courses.Where(c => c.CampusGroupId == groupId).Include(c => c.Students).Select(course => CourseMapper.MapFromCampusCourseToCampusCoursePreviewModel(course)).ToListAsync();
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
            var course = await _db.Courses.Include(c => c.Students).ThenInclude(s => s.User).Include(c => c.Teachers).ThenInclude(t => t.User).Include(c => c.Notifications).FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null) throw new NotFoundException(ErrorConstants.NotFoundCourseError);
            var user = await _accountService.GetUserById(userId);
            var student = course.Students.FirstOrDefault(s => s.UserId == user.Id);
            var teacher = course.Teachers.FirstOrDefault(s => s.UserId == user.Id);
            if (!(student == null)) throw new BadRequestException(ErrorConstants.SignedUpError);
            if (!(teacher == null)) throw new BadRequestException(ErrorConstants.TeacherSignUpError);
            if (!(course.Status == CourseStatuses.OpenForAssigning))
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
            var course = await _db.Courses.Include(c => c.Students).ThenInclude(s => s.User).Include(c => c.Teachers).ThenInclude(t => t.User).Include(c => c.Notifications).FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) throw new NotFoundException(ErrorConstants.NotFoundCourseError);
            if (course.Status == CourseStatuses.Finished && (editCourseStatusModel.Status == CourseStatuses.OpenForAssigning || editCourseStatusModel.Status == CourseStatuses.Started || editCourseStatusModel.Status == CourseStatuses.Created)) 
            {
                throw new BadRequestException(ErrorConstants.PreviousStatusError);
            }
            if (course.Status == CourseStatuses.Started && (editCourseStatusModel.Status == CourseStatuses.OpenForAssigning || editCourseStatusModel.Status == CourseStatuses.Created))
            {
                throw new BadRequestException(ErrorConstants.PreviousStatusError);
            }
            if (course.Status == CourseStatuses.OpenForAssigning && (editCourseStatusModel.Status == CourseStatuses.Created))
            {
                throw new BadRequestException(ErrorConstants.PreviousStatusError);
            }
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
            var course = await _db.Courses.Include(c => c.Students).ThenInclude(s => s.User).Include(c => c.Teachers).ThenInclude(t => t.User).Include(c => c.Notifications).FirstOrDefaultAsync(c => c.Id == id);
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
            var course = await _db.Courses.Include(c => c.Students).ThenInclude(s => s.User).Include(c => c.Teachers).ThenInclude(t => t.User).Include(c => c.Notifications).FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null) throw new NotFoundException(ErrorConstants.NotFoundCourseError);
            var isAdmin = (await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id))?.IsAdmin ?? false;
            var isTeacher = course.Teachers.Any(t => t.UserId == user.Id);
            var isStudent = course.Students.Any(s => s.UserId == user.Id);
            List<CampusCourseStudentModel> students;

            if (isAdmin || isTeacher)
            {
                students = course.Students.Select(s => new CampusCourseStudentModel
                {
                    Id = s.UserId,
                    Name = s.User.Name,
                    Email = s.User.Email,
                    Status = s.Status,
                    MidtermResult =  s.MidtermResult,
                    FinalResult =  s.FinalResult,
                }).ToList();
            }
            else if (isStudent)
            {
                students = course.Students
                    .Where(s => s.Status == StudentStatuses.Accepted)
                    .Select(s => new CampusCourseStudentModel
                    {
                        Id = s.UserId,
                        Name = s.User.Name,
                        Email = s.User.Email,
                        Status = s.Status,
                        MidtermResult = s.UserId == user.Id ? s.MidtermResult : null,
                        FinalResult = s.UserId == user.Id ? s.FinalResult : null
                    }).ToList();
            }
            else
            {
                students = course.Students
                    .Where(s => s.Status == StudentStatuses.Accepted)
                    .Select(s => new CampusCourseStudentModel
                    {
                        Id = s.UserId,
                        Name = s.User.Name,
                        Email = s.User.Email,
                        Status = s.Status,
                        MidtermResult = null,
                        FinalResult = null
                    }).ToList();
            }
            var courseDetails = new CampusCourseDetailsModel
            {
                Id = course.Id,
                Name = course.Name,
                StartYear = course.StartYear,
                MaximumStudentsCount = course.MaximumStudentsCount,
                StudentsInQueueCount = course.Students.Count(s => s.Status == StudentStatuses.InQueue),
                StudentsEnrolledCount = course.Students.Count(s => s.Status == StudentStatuses.Accepted),
                Requirements = course.Requirements,
                Annotations = course.Annotation,
                Semester = course.Semester,
                Status = course.Status,
                Students = students,
                Teachers = course.Teachers.Select(teacher => new CampusCourseTeacherModel
                {
                    Name = teacher.User.Name,
                    Email = teacher.User.Email,
                    IsMain = teacher.IsMain
                }).ToList(),
                Notifications = course.Notifications.Select(notification => new CampusCourseNotificationModel
                {
                    Text = notification.Text,
                    IsImportant = notification.isImportant
                }).ToList()
            };
            return courseDetails;
        }

        public async Task<CampusCourseDetailsModel> EditStudentStatus(Guid courseId, string userId, Guid studentId, EditCourseStudentStatusModel editCourseStudentStatusModel)
        {
            var user = await _accountService.GetUserById(userId);
            var course = await _db.Courses.Include(c => c.Students).ThenInclude(s => s.User).Include(c => c.Teachers).ThenInclude(t => t.User).Include(c => c.Notifications).FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null) throw new NotFoundException(ErrorConstants.NotFoundCourseError);
            var student = course.Students.FirstOrDefault(s => s.UserId == studentId);
            if (student == null) throw new BadRequestException($"User with id {studentId} is not signed up for this course.");
            if (editCourseStudentStatusModel == null) throw new BadRequestException(ErrorConstants.EmtyBodyError);
            
            if (!(student.Status == StudentStatuses.InQueue)) throw new BadRequestException(ErrorConstants.NotInQueueError);
      
            if (course == null) throw new NotFoundException(ErrorConstants.NotFoundCourseError);

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id);
            var teacher = course.Teachers.FirstOrDefault(t => t.UserId == user.Id);
            if (role == null || (!role.IsAdmin && teacher == null)) throw new ForbiddenException(ErrorConstants.ForbiddenError);
            if  ((course.MaximumStudentsCount - course.Students.Count(s => s.Status == StudentStatuses.Accepted)) > 0)
            {
                student.Status = editCourseStudentStatusModel.Status;
                if (student.Status == StudentStatuses.Accepted)
                {
                    student.MidtermResult = StudentMarks.NotDefined;
                    student.FinalResult = StudentMarks.NotDefined;
                }
                _db.Students.Update(student);
                await _db.SaveChangesAsync();
            }
            else
            {
                throw new BadRequestException(ErrorConstants.MaximumStudentError);
            }
            return CourseMapper.MapFromCampusCourseToCampusCourseDetailsModel(course);
        }

        public async Task<CampusCourseDetailsModel> EditStudentMark(Guid courseId, string userId, Guid studentId, EditCourseStudentMarkModel editCourseStudentMarkModel)
        {
            var user = await _accountService.GetUserById(userId);
            var course = await _db.Courses.Include(c => c.Students).ThenInclude(s => s.User).Include(c => c.Teachers).ThenInclude(t => t.User).Include(c => c.Notifications).FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null) throw new NotFoundException(ErrorConstants.NotFoundCourseError);
            var student = course.Students.FirstOrDefault(s => s.UserId == studentId);
            if (student == null) throw new BadRequestException($"User with id {studentId} is not signed up for this course.");
            if (editCourseStudentMarkModel == null) throw new BadRequestException(ErrorConstants.EmtyBodyError);

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id);
            var teacher = course.Teachers.FirstOrDefault(t => t.UserId == user.Id);
            if (role == null || (!role.IsAdmin && teacher == null)) throw new ForbiddenException(ErrorConstants.ForbiddenError);
            if (student.Status == StudentStatuses.Declined)
                throw new BadRequestException(ErrorConstants.DeclinedError);
            if (student.Status == StudentStatuses.InQueue)
                throw new BadRequestException(ErrorConstants.InQueueError);
            if (editCourseStudentMarkModel.MarkType == MarkType.Midterm)
            {
                student.MidtermResult = editCourseStudentMarkModel.Marks;
            }
            else if (editCourseStudentMarkModel.MarkType == MarkType.Final)
            {
                student.FinalResult = editCourseStudentMarkModel.Marks;
            }
            _db.Students.Update(student);
            await _db.SaveChangesAsync();
            return CourseMapper.MapFromCampusCourseToCampusCourseDetailsModel(course);
        }

        public async Task<CampusCourseDetailsModel> AddTeacherToCourse(Guid courseId, string userId, AddTeacherToCourseModel addTeacherToCourseModel)
        {
            var user = await _accountService.GetUserById(userId);
            var course = await _db.Courses.Include(c => c.Students).ThenInclude(s => s.User).Include(c => c.Teachers).ThenInclude(t => t.User).Include(c => c.Notifications).FirstOrDefaultAsync(c => c.Id == courseId);
            if (addTeacherToCourseModel == null) throw new BadRequestException(ErrorConstants.EmtyBodyError);

            if (course == null) throw new NotFoundException(ErrorConstants.NotFoundCourseError);

            var exTeacher = course.Teachers.FirstOrDefault(t => t.UserId == addTeacherToCourseModel.Id);
            if (!(exTeacher == null)) throw new BadRequestException(ErrorConstants.AlreadyTeacherError);

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id);
            var mainTeacher = course.Teachers.FirstOrDefault(t => t.UserId == user.Id);
            if (mainTeacher == null) throw new ForbiddenException(ErrorConstants.ForbiddenTeacherError);
            if (role == null || (!role.IsAdmin && (mainTeacher == null || !mainTeacher.IsMain))) throw new ForbiddenException(ErrorConstants.ForbiddenTeacherError);

            var student = course.Students.FirstOrDefault(s => s.UserId == addTeacherToCourseModel.Id);
            if (!(student == null))
            {
                throw new BadRequestException(ErrorConstants.StudentCannootBeTeacherError);
            }
            var teacher = _db.Users.FirstOrDefault(u => u.Id == addTeacherToCourseModel.Id);
            if (teacher == null) throw new NotFoundException($"User with ID {addTeacherToCourseModel.Id} not found.");
            var courseTeacher = CourseTeacherMapper.MapUserToNotMainTeacher(teacher, course);
            await _db.Teachers.AddAsync(courseTeacher);
            await _db.SaveChangesAsync();
            return CourseMapper.MapFromCampusCourseToCampusCourseDetailsModel(course);
        }

        public async Task<List<CampusCoursePreviewModel>> GetMyCourses(string userId)
        {
            var user = await _accountService.GetUserById(userId);
            var courses = await _db.Students.Include(s => s.CampusCourse).ThenInclude(c => c.Students).Where(s => s.UserId == user.Id).Select(s => s.CampusCourse).ToListAsync();

            var coursePreviews = courses.Select(course => CourseMapper.MapFromCampusCourseToCampusCoursePreviewModel(course)).ToList();
            return coursePreviews;
        }

        public async Task<List<CampusCoursePreviewModel>> GetTeachingCourses(string userId)
        {
            var user = await _accountService.GetUserById(userId);
            var courses = await _db.Teachers.Include(t => t.CampusCourse).ThenInclude(c => c.Students).Where(t => t.UserId == user.Id).Select(t => t.CampusCourse).ToListAsync();

            var coursePreviews = courses.Select(course => CourseMapper.MapFromCampusCourseToCampusCoursePreviewModel(course)).ToList();
            return coursePreviews;
        }

        public async Task<CampusCourseDetailsModel> EditCourseInfo(Guid courseId, string userId, EditCampusCourseRequirementsAndAnnotationModel editCampusCourseRequirementsAndAnnotationModel)
        {
            var user = await _accountService.GetUserById(userId);
            var course = await _db.Courses.Include(c => c.Students).ThenInclude(s => s.User).Include(c => c.Teachers).ThenInclude(t => t.User).Include(c => c.Notifications).FirstOrDefaultAsync(c => c.Id == courseId);
            if (editCampusCourseRequirementsAndAnnotationModel == null) throw new BadRequestException(ErrorConstants.EmtyBodyError);

            if (course == null) throw new NotFoundException(ErrorConstants.NotFoundCourseError);

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id);
            var teacher = course.Teachers.FirstOrDefault(t => t.UserId == user.Id);
            if (role == null || (!role.IsAdmin && teacher == null)) throw new ForbiddenException(ErrorConstants.ForbiddenError);

            course.Annotation = editCampusCourseRequirementsAndAnnotationModel.Annotations;
            course.Requirements = editCampusCourseRequirementsAndAnnotationModel.Requirements;
            _db.Courses.Update(course);
            await _db.SaveChangesAsync();
            return CourseMapper.MapFromCampusCourseToCampusCourseDetailsModel(course);
        }

        public async Task<List<CampusCoursePreviewModel>> GetCourses(SortList? sort, string? search, bool hasPlaceAndOpen, Semesters? semester, int page, int pageSize)
        {

            if (pageSize <= 0) throw new BadRequestException(ErrorConstants.SizeError);
            if (page <= 0) throw new BadRequestException(ErrorConstants.PageError);
            var courses = _db.Courses.Include(c => c.Students).Include(c => c.Teachers).Include(c => c.Notifications).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                courses = courses.Where(c => c.Name.ToLower().Contains(lowerSearch));
            }

            if (hasPlaceAndOpen)
            {
                courses = courses.Where(c => c.Status == CourseStatuses.OpenForAssigning && (c.MaximumStudentsCount - c.Students.Count(s => s.Status == StudentStatuses.Accepted)) > 0);
            }

            if (semester.HasValue)
            {
                courses = courses.Where(c => c.Semester == semester.Value);
            }
            if (sort.HasValue)
            {
                switch (sort.Value)
                {
                    case SortList.CreateAsc:
                        courses = courses.OrderBy(c => c.CreateTime);
                        break;
                    case SortList.CreateDesc:
                        courses = courses.OrderByDescending(c => c.CreateTime);
                        break;
                    default:
                        throw new BadRequestException(ErrorConstants.SortError);
                }
            }

            var skip = (page - 1) * pageSize;
            var coursesPage = await courses.Skip(skip).Take(pageSize).ToListAsync();
            return coursesPage.Select(c => new CampusCoursePreviewModel
            {
                Id = c.Id,
                Name = c.Name,
                StartYear = c.StartYear,
                MaximumStudentsCount = c.MaximumStudentsCount,
                RemainingSlotsCount = c.MaximumStudentsCount - c.Students.Count(s => s.Status == StudentStatuses.Accepted),
                Status = c.Status,
                Semester = c.Semester
            }).ToList();
        }

        public async Task<CampusCourseDetailsModel> EditCourse(Guid courseId, string userId, EditCampusCourseModel editCampusCourseModel)
        {
            var user = await _accountService.GetUserById(userId);
            var course = await _db.Courses.Include(c => c.Students).ThenInclude(s => s.User).Include(c => c.Teachers).ThenInclude(t => t.User).Include(c => c.Notifications).FirstOrDefaultAsync(c => c.Id == courseId);
            if (editCampusCourseModel == null) throw new BadRequestException(ErrorConstants.EmtyBodyError);

            if (course == null) throw new NotFoundException(ErrorConstants.NotFoundCourseError);

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id);
            if (role == null || !role.IsAdmin) throw new ForbiddenException(ErrorConstants.ForbiddenError);

            var enrolledStudentsCount = course.Students.Count(s => s.Status == StudentStatuses.Accepted);
            if (editCampusCourseModel.MaximumStudentsCount < enrolledStudentsCount)
            {
                throw new ConflictException($"Maximum students count cannot be less than the number of enrolled students ({enrolledStudentsCount}).");
            }
            course = CourseMapper.MapFromEditCampusCourseModelToCampusCourse(courseId, editCampusCourseModel, course);
            _db.Courses.Update(course);
            await _db.SaveChangesAsync();
            return CourseMapper.MapFromCampusCourseToCampusCourseDetailsModel(course);
        }
    }
}