using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Models;
using tcs_service.Helpers;
using tcs_service.Repos.Base;
using tcs_service.Models.ViewModels;

namespace tcs_service.Repos.Interfaces
{
    public interface IClassTourRepo : IRepo<ClassTour>
    {
        Task<ClassTour> Add(ClassTour tour);

        Task<ClassTour> Update(ClassTour tour);

        Task<PagingModel<ClassTourViewModel>> GetBetweenDates(DateTime start, DateTime end, int skip, int take);
    }
}
