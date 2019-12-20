using System;
using System.Threading.Tasks;
using tcs_service.Models;
using tcs_service.Helpers;
using tcs_service.Models.ViewModels;
using tcs_service.Repos.Base;

namespace tcs_service.Repos.Interfaces
{
    public interface IClassTourRepo : IRepo<ClassTour>
    {
        Paging<ClassTourViewModel> GetBetweenDates(DateTime start, DateTime end, int skip, int take);
    }
}
