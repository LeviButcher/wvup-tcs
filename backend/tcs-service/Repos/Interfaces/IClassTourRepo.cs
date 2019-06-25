using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Models;
using tcs_service.Repos.Base;

namespace tcs_service.Repos.Interfaces
{
    public interface IClassTourRepo
    {
        Task<ClassTour> Add(ClassTour tour);

        Task<ClassTour> Update(ClassTour tour);

        Task<bool> Exist(int id);

        Task<ClassTour> Find(int id);

        IEnumerable<ClassTour> GetAll();

        Task<ClassTour> Remove(int id);
    }
}
