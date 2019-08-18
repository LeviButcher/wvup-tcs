using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Services.ScheduledTasks
{
    public class StudentSignOutJob : IJob
    {
        private readonly ISignInRepo _iRepo;


        public StudentSignOutJob(ISignInRepo iRepo)
        {
            _iRepo = iRepo;
        }
        
        public async Task Execute(IJobExecutionContext context)
        {
            await _iRepo.UpdateNullSignOuts();
          
            return;
        }
    }
}
