using System.Linq;
using Microsoft.EntityFrameworkCore;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos {
    ///<summary>Repo for the SessionReason Table</summary>
    public class SessionReasonRepo : BaseRepo<SessionReason>, ISessionReasonRepo {
        ///<summary>SessionRepo Constructor</summary>
        public SessionReasonRepo (DbContextOptions options) : base (options) { }
    }
}