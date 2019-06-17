using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.EF;

namespace tcs_service.Repos
{
    public class BaseRepo : IDisposable
    {
        protected TCSContext _db;
        private bool _disposed = false;

        public BaseRepo()
        {
            _db = new TCSContext();
        }

        public BaseRepo(TCSContext context)
        {
            _db = context;
        }

        public BaseRepo(DbContextOptions options)
        {
            _db = new TCSContext(options);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                //Free any other managed objects here
            }
            _db.Dispose();
            _disposed = true;
        }

        public int SaveChanges()
        {
            try
            {
                return _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //A concurrency error occurred
                throw;
            }
            catch (RetryLimitExceededException ex)
            {
                //_dbResiliency retry limit exceeded
                //logger.Error("Maximum retry limit reached.", ex);
                throw;
            }
            catch (Exception ex)
            {
                //logger.Error("Error occurred.", ex);
                throw;
            }
        }
        
    }
}
