using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Repos.Base
{
    public interface IRepo<T> where T : new()
    {
        Task<bool> Exist(int id);

        Task<T> Find(int id);

        IEnumerable<T> GetAll();

        Task<T> Remove(int id);
    }
}
