using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using tcs_service.Services.ScheduledTasks;

namespace tcs_service.Services {
    /// <summary>QuartzHostedService, part of running a job at a specific time</summary>
    public class QuartzHostedService : IHostedService {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly IEnumerable<JobSchedule> _jobSchedules;
        private IScheduler _scheduler;

        /// <summary>QuartzHostedService Constructor</summary>
        public QuartzHostedService (
            ISchedulerFactory schedulerFactory,
            IEnumerable<JobSchedule> jobSchedules,
            IJobFactory jobFactory) {
            _schedulerFactory = schedulerFactory;
            _jobSchedules = jobSchedules;
            _jobFactory = jobFactory;
        }

        /// <summary>StartAsync</summary>
        public async Task StartAsync (CancellationToken cancellationToken) {
            _scheduler = await _schedulerFactory.GetScheduler (cancellationToken);
            _scheduler.JobFactory = _jobFactory;

            foreach (var jobSchedule in _jobSchedules) {
                var job = CreateJob (jobSchedule);
                var trigger = CreateTrigger (jobSchedule);

                await _scheduler.ScheduleJob (job, trigger, cancellationToken);
            }

            await _scheduler.Start (cancellationToken);
        }

        /// <summary>StopAsync</summary>
        public async Task StopAsync (CancellationToken cancellationToken) {
            await _scheduler.Shutdown (cancellationToken);
        }

        /// <summary>Create Trigger</summary>
        private static ITrigger CreateTrigger (JobSchedule schedule) {
            return TriggerBuilder
                .Create ()
                .WithIdentity ($"{schedule.JobType.FullName}.trigger")
                .WithCronSchedule (schedule.CronExpression)
                .WithDescription (schedule.CronExpression)
                .Build ();
        }

        /// <summary>Create Job</summary>
        private static IJobDetail CreateJob (JobSchedule schedule) {
            var jobType = schedule.JobType;
            return JobBuilder
                .Create (jobType)
                .WithIdentity (jobType.FullName)
                .WithDescription (jobType.Name)
                .Build ();
        }
    }
}