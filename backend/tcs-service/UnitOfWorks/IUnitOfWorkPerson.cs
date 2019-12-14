using System;
using System.Threading.Tasks;
using tcs_service.Models.DTOs;

namespace tcs_service.UnitOfWorks
{
    public interface IUnitOfWorkPerson
    {
        Task<PersonInfoDTO> GetPersonInfo(string identifier, DateTime currentDate);
    }
}
