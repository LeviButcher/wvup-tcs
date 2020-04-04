using System.Threading.Tasks;
using tcs_service.Models;
using tcs_service.Repos.Base;

namespace tcs_service.Repos.Interfaces {
    ///<summary>Repo for the User Table</summary>
    public interface IUserRepo : IRepo<User> {
        ///<summary>Authenticate a user and return a session token</summary>
        Task<User> Authenticate (string username, string password);

        ///<summary>Create a new User</summary>
        Task<User> Create (User user, string password);

        ///<summary>Update a new User</summary>
        Task<User> Update (User user, string password = null);
    }
}