using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos
{
    public class ReasonRepo : BaseRepo<Reason>, IReasonRepo
    {

        public ReasonRepo(DbContextOptions options) : base(options)
        {

        }

        public async override Task<bool> Exist(int id)
        {
            return await _db.Reasons.AnyAsync(e => e.ID == id);
        }

        public async override Task<Reason> Find(int id)
        {
            return await _db.Reasons.SingleOrDefaultAsync(a => a.ID == id);
        }

        public override IEnumerable<Reason> GetAll()
        {
            return _db.Reasons;
        }

        public IEnumerable<Reason> GetActive()
        {
            return _db.Reasons.Where(x => x.Deleted == false);
        }

        public override Task<Reason> Remove(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Reason> Add(Reason reason)
        {
            await _db.AddAsync(reason);
            await _db.SaveChangesAsync();
            return reason;
        }
        public async Task<Reason> Update(Reason reason)
        {
            _db.Reasons.Update(reason);
            await _db.SaveChangesAsync();
            return reason;
        }

    }
}
