using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Services.ScheduledTasks
{
    public class JobSchedule
    {
        // Taken from https://andrewlock.net/creating-a-quartz-net-hosted-service-with-asp-net-core/
        public JobSchedule(Type jobType, string cronExpression)
        {
            JobType = jobType;
            CronExpression = cronExpression;
        }

        public Type JobType { get; }
        public string CronExpression { get; }
    }
}
