using System.Linq;
using Microsoft.EntityFrameworkCore;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos {
    ///<summary>Repo for the Reason Table</summary>
    public class ReasonRepo : BaseRepo<Reason>, IReasonRepo {
        ///<summary>ReasonRepo Constructor</summary>
        public ReasonRepo (DbContextOptions options) : base (options) { }
    }
}