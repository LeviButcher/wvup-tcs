using System.Collections.Generic;
using System.Threading.Tasks;
using tcs_service.Helpers;

namespace tcs_service.UnitOfWorks.Interfaces
{
    public interface IUnitOfWorkSession
    {
        Task<int> UploadSessions(IEnumerable<CSVSessionUpload> sessions);
    }
}
