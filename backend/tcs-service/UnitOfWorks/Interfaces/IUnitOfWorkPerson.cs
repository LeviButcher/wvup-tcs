using System.Threading.Tasks;
using tcs_service.Models.DTOs;

namespace tcs_service.UnitOfWorks.Interfaces {

    /// <summary>IUnitOfWorkPerson</summary>
    public interface IUnitOfWorkPerson {

        /// <summary>Returns back the student information from their WVUP Id and email</summary>
        Task<PersonInfoDTO> GetPersonInfo (string identifier);
    }
}