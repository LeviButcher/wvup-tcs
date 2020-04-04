using System;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace tcs_service.Services.ScheduledTasks {
    /// <summary>Creates a New Job that gets ran</summary>
    public class JobFactory : IJobFactory {
        // Taken from https://andrewlock.net/creating-a-quartz-net-hosted-service-with-asp-net-core/

        private readonly IServiceProvider _serviceProvider;

        /// <summary>JobFactory Constructor</summary>
        public JobFactory (IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        /// <summary>Trigger the job</summary>
        public IJob NewJob (TriggerFiredBundle bundle, IScheduler scheduler) {
            return _serviceProvider.GetRequiredService<JobRunner> ();
        }

        /// <summary>Return the Job</summary>
        public void ReturnJob (IJob job) { }
    }
}