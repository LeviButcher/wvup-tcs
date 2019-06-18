using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.EF;

namespace tcs_service.Repos.Base
{
    public abstract class BaseRepo<T> : IRepo<T>,IDisposable where T : new()
    {
        protected TCSContext _db;
        private bool _disposed = false;

        protected BaseRepo()
        {
            _db = new TCSContext();
        }

        protected BaseRepo(TCSContext context)
        {
            _db = context;
        }

        protected BaseRepo(DbContextOptions options)
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

        public abstract Task<bool> Exist(int id);
        public abstract Task<T> Find(int id);
        public abstract IEnumerable<T> GetAll();
        public abstract Task<T> Remove(int id);
    }
}
