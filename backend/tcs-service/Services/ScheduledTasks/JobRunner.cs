using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace tcs_service.Services.ScheduledTasks {
    /// <summary>Job Runner</summary>
    [DisallowConcurrentExecution]
    public class JobRunner : IJob {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>Job Runner Constructor</summary>
        public JobRunner (IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        /// <summary>Execute a Job</summary>
        public async Task Execute (IJobExecutionContext context) {
            using (var scope = _serviceProvider.CreateScope ()) {
                var job = scope.ServiceProvider.GetRequiredService (context.JobDetail.JobType) as IJob;

                await job.Execute (context);
            }
        }
    }
}