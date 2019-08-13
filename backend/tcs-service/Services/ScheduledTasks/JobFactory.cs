using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Services.ScheduledTasks
{
    public class JobFactory : IJobFactory
    {
        // Taken from https://andrewlock.net/creating-a-quartz-net-hosted-service-with-asp-net-core/

        private readonly IServiceProvider _serviceProvider;
      
        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
           
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetRequiredService<JobRunner>();
        }

        public void ReturnJob(IJob job) { }
    }
}
