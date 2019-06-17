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
        User Authenticate(string username, string password);
    }
}
