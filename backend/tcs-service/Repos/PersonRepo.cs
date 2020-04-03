using System.Linq;
using Microsoft.EntityFrameworkCore;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos {
    ///<summary>Repo for the Person Table</summary>
    public class PersonRepo : BaseRepo<Person>, IPersonRepo {
        ///<summary>PersonRepo constructor</summary>
        public PersonRepo (DbContextOptions options) : base (options) { }
    }
}