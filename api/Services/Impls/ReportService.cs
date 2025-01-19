using api.Exceptions;
using api.Models.CampusGroup;
using api.Models.Enums;
using api.Models.Teacher;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Services.Impls
{
    public class ReportService : IReportService
    {
        private readonly DataContext _db;
        private readonly IAccountService _accountService;

        public ReportService(DataContext db, IAccountService accountService)
        {
            _db = db;
            _accountService = accountService;
        }
        public async Task<List<TeacherReportRecordModel>> GetReport(string userId, Semesters? semester, List<Guid>? campusGroupIds)
        {

            var user = await _accountService.GetUserById(userId);
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.UserId == user.Id);
            if (role == null || !role.IsAdmin)
            {
                throw new ForbiddenException(ErrorConstants.ForbiddenError);
            }
            var coursesQuery = _db.Courses.Include(c => c.Students).Include(c => c.Teachers).Include(c => c.CampusGroup).AsQueryable();
            Console.WriteLine("1 step", coursesQuery.ToList().Count);
            if (semester.HasValue)
            {
                coursesQuery = coursesQuery.Where(c => c.Semester == semester.Value);
                Console.WriteLine("2 step", coursesQuery.ToList().Count);
            }
            if (campusGroupIds != null && campusGroupIds.Any())
            {
                var existingGroupIds = await _db.CampusGroups.Where(g => campusGroupIds.Contains(g.Id)).Select(g => g.Id).ToListAsync();

                var nonExistentGroupIds = campusGroupIds.Except(existingGroupIds).ToList();
                if (nonExistentGroupIds.Any())
                {
                    throw new NotFoundException($"Groups with the IDs were not found");
                }
            }
            else
            {
                campusGroupIds = await _db.CampusGroups.Select(g => g.Id).ToListAsync();
            }
            coursesQuery = coursesQuery.Where(c => campusGroupIds.Contains(c.CampusGroupId));
            var courses = await coursesQuery.ToListAsync();
 
            Console.WriteLine("step 8");
            var reportData = courses.Where(c => c.Teachers.Any(t => t.IsMain)).GroupBy(c => c.Teachers.First(t => t.IsMain).UserId).Select(g => new TeacherReportRecordModel
            {
                Id = g.Key,
                FullName = _db.Users.Where(u => u.Id == g.Key).Select(u => u.Name).FirstOrDefault(),
                CampusGroupReports = campusGroupIds.Where(groupId => g.Any(c => c.CampusGroupId == groupId)).Select(groupId =>
                    {
                        var totalStudents = g.Where(c => c.CampusGroupId == groupId).SelectMany(c => c.Students).Count();
                        Console.WriteLine("students", groupId.ToString(), totalStudents.ToString());
                        return new CampusGroupReportModel
                        {
                            Id = groupId,
                            Name = _db.CampusGroups.Where(g => g.Id == groupId).Select(g => g.Name).FirstOrDefault(),
                            AveragePassed = totalStudents > 0
                                ? g.Where(c => c.CampusGroupId == groupId).SelectMany(c => c.Students).Count(s => s.FinalResult == StudentMarks.Passed) / (double)totalStudents : 0,
                            AverageFailed = totalStudents > 0
                                ? g.Where(c => c.CampusGroupId == groupId).SelectMany(c => c.Students).Count(s => s.FinalResult == StudentMarks.Failed) / (double)totalStudents : 0
                        };
                    })
                    .ToList()
            })
            .Where(report => report.CampusGroupReports.Any()) 
            .OrderBy(report => report.FullName)
            .ToList();
            Console.WriteLine("step 9",reportData.Count);
            return reportData;
        }
    }
}
