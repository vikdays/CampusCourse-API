using api.Services.Interfaces;
using Quartz;

namespace api.Services.Email
{
    public class QuartzJobScheduler
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public QuartzJobScheduler(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        public async Task ScheduleEmailNotificationJob()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var job = JobBuilder.Create<EmailNotificationJob>()
                .WithIdentity("emailNotificationJob", "group1").Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("emailNotificationTrigger", "group1").StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInHours(24).RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
