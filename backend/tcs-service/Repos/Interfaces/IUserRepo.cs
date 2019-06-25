using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Models;
using tcs_service.Repos.Base;

namespace tcs_service.Repos.Interfaces
{
    public interface IUserRepo : IRepo<User>
    {
        Task<User> Authenticate(string username, string password);
        Task<User> Create(User user, string password);
        void Update(User user, string password = null);
        void Delete(int id);

    }
}
