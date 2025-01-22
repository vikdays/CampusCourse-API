using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace api.Services.Email
{
    public class EmailNotificationJob : IJob
    {
        private readonly IEmailSender _emailSender;
        private readonly DataContext _db;

        public EmailNotificationJob(IEmailSender emailSender, DataContext db)
        {
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var tomorrow = DateTime.UtcNow.Date.AddDays(1);
                var coursesWithStudents = await _db.Courses.Include(c => c.Students).ThenInclude(s => s.User)
                    .Where(c => c.StartDate.HasValue && c.StartDate.Value.Date == tomorrow)
                    .ToListAsync();

                foreach (var course in coursesWithStudents)
                {
                    foreach (var student in course.Students.Where(s => !s.IsSent))
                    {
                        try
                        {
                            var subject = $"Курс {course.Name} начинается завтра!";
                            var body = $"Здравствуйте, {student.User.Name}. Курс {course.Name} начинается завтра. Не забудьте подготовиться!";

                            await _emailSender.SendEmail(student.User.Email, subject, body);

                            student.IsSent = true;
                            Console.WriteLine($"Email sent to {student.User.Email} successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error sending email to {student.User.Email}: {ex.Message}");
                        }
                    }
                }
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in job execution: {ex.Message}");
                throw new JobExecutionException(ex, refireImmediately: true);
            }
        }
    }
}
