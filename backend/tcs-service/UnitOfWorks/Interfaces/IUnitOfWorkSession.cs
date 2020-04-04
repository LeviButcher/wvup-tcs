using System.Collections.Generic;
using System.Threading.Tasks;
using tcs_service.Helpers;

namespace tcs_service.UnitOfWorks.Interfaces {

    /// <summary>IUnitOfWorkSession</summary>
    public interface IUnitOfWorkSession {

        /// <summary>Uploads a List of CSVSessionUpload to the database</summary>
        Task<int> UploadSessions (IEnumerable<CSVSessionUpload> sessions);
    }
}