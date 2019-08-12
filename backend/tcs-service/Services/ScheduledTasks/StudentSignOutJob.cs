using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Services.ScheduledTasks
{
    [DisallowConcurrentExecution]
    public class StudentSignOutJob : IJob
    {
        private ISignInRepo _iRepo;

        public StudentSignOutJob(ISignInRepo iRepo)
        {
            _iRepo = iRepo;
        }
        
        async Task IJob.Execute(IJobExecutionContext context)
        {
            var signIns = await _iRepo.GetNullSignOuts();
            _iRepo.UpdateNullSignOuts(signIns);
        }
    }
}
