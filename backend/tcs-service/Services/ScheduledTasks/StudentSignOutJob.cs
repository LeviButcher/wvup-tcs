using System.Threading.Tasks;
using Quartz;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Services.ScheduledTasks {

    ///<summary>Signs out all sessions that have not been signed out</summary>
    public class StudentSignOutJob : IJob {
        private readonly ISessionRepo _iRepo;

        /// <summary>StudentSignOutJob Constructor</summary>
        public StudentSignOutJob (ISessionRepo iRepo) {
            _iRepo = iRepo;
        }

        /// <summary>Execute signing out all session that have not been signed out</summary>
        /// signs them out 2 hours after their inTime
        public async Task Execute (IJobExecutionContext context) {
            var signIns = _iRepo.GetAll (x => x.OutTime == null && x.InTime != null);
            foreach (var signIn in signIns) {
                signIn.OutTime = signIn.InTime.AddHours (2);
                await _iRepo.Update (signIn);
            }
        }
    }
}