using System.Linq;
using Microsoft.EntityFrameworkCore;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos {
    ///<summary>Repo for the SessionClass Table</summary>
    public class SessionClassRepo : BaseRepo<SessionClass>, ISessionClassRepo {
        ///<summary>SessionClass Repo</summary>
        public SessionClassRepo (DbContextOptions options) : base (options) { }
    }
}