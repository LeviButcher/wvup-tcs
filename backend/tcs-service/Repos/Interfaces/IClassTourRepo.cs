using System;
using System.Threading.Tasks;
using tcs_service.Models;
using System.Collections.Generic;
using tcs_service.Models.DTO;
using tcs_service.Repos.Base;

namespace tcs_service.Repos.Interfaces
{
    public interface IClassTourRepo : IRepo<ClassTour>
    {
        IEnumerable<ClassTour> GetBetweenDates(DateTime start, DateTime end);
    }
}
