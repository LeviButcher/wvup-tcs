using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tcs_service.Models.ViewModels;

namespace tcs_service.Repos.Interfaces
{
    public interface ILookupRepo
    {
        Task<IEnumerable<SignInViewModel>> Get(DateTime start, DateTime end, int skip, int take);
    }
}
