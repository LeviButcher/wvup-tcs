using System.Collections.Generic;
using System.Threading.Tasks;

namespace tcs_service.UnitOfWorks.Interfaces
{
    public interface IUnitOfWorkSession
    {
        Task<int> UploadSessions(IEnumerable<CSVSessionUpload> sessions);
    }
}
