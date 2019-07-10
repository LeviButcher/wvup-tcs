using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Models;
using tcs_service.Repos.Base;

namespace tcs_service.Repos.Interfaces
{
    public interface IReasonRepo : IRepo<Reason>
    {
        Task<Reason> Add(Reason reason);

        IEnumerable<Reason> GetActive();

        Task<Reason> Update(Reason reason);
    }
}
