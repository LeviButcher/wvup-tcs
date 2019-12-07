using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Models;
using tcs_service.Models.ViewModels;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;
using tcs_service.Services.Interfaces;

namespace tcs_service.Repos
{
    public class SignInRepo : BaseRepo<SignIn>, ISignInRepo
    {
        public SignInRepo(DbContextOptions options) : base(options)
        {
        }

        protected override IQueryable<SignIn> Include(DbSet<SignIn> set) => set.Include(x => x.Courses).Include(x => x.Reasons);
    }
}
