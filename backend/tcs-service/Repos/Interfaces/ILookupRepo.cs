using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tcs_service.Helpers;
using tcs_service.Models.ViewModels;

namespace tcs_service.Repos.Interfaces
{
    public interface ILookupRepo
    {
        Task<PagingModel<SignInViewModel>> Get(DateTime start, DateTime end, int? crn, string email, int skip, int take);

        Task<PagingModel<SignInViewModel>> Daily(int skip, int take);

        Task<PagingModel<SignInViewModel>> GetByCRN(int crn, DateTime start, DateTime end, int skip, int take);

        Task<PagingModel<SignInViewModel>> GetByEmail(string email, DateTime start, DateTime end, int skip, int take);

        Task<List<SignInSpreadSheetViewModel>> GetBySemester(int semesterId);
    }
}
