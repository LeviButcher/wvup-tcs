using System;
using System.Threading.Tasks;
using tcs_service.Models.DTOs;

namespace tcs_service.UnitOfWorks.Interfaces
{
    public interface IUnitOfWorkPerson
    {
        Task<PersonInfoDTO> GetPersonInfo(string identifier, DateTime currentDate);
    }
}
