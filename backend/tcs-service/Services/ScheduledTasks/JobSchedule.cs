using System;

namespace tcs_service.Services.ScheduledTasks {
    ///<summary>Job Schedule</summary>
    public class JobSchedule {
        ///<summary>Job Schedule Constructor</summary>
        // Taken from https://andrewlock.net/creating-a-quartz-net-hosted-service-with-asp-net-core/
        public JobSchedule (Type jobType, string cronExpression) {
            JobType = jobType;
            CronExpression = cronExpression;
        }

        ///<summary>JobType</summary>
        public Type JobType { get; }

        ///<summary>CronExpression</summary>
        public string CronExpression { get; }
    }
}