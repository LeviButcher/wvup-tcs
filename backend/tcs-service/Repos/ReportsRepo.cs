using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Models;
using tcs_service.Models.ViewModels;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos
{
    public class ReportsRepo : BaseRepo<SignIn>, IReportsRepo
    {
        public ReportsRepo(DbContextOptions options) : base(options)
        {

        }

        public override Task<bool> Exist(int id)
        {
            throw new NotImplementedException();
        }

        public override Task<SignIn> Find(int id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<SignIn> GetAll()
        {
            throw new NotImplementedException();
        }

        public override Task<SignIn> Remove(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ReportCountViewModel>> WeeklyVisits(DateTime startWeek, DateTime endWeek)
        {
            var result = new List<ReportCountViewModel>();
            var count = 1;
            while (startWeek <= endWeek)
            {
                result.Add(new ReportCountViewModel
                {
                    Item = count,
                    Count = await _db.SignIns.Where(x => x.InTime >= startWeek && x.InTime <= startWeek.AddDays(7)).CountAsync()
                });
                count++;
                startWeek = startWeek.AddDays(7);
            }
            return result;
        }
    }
}
