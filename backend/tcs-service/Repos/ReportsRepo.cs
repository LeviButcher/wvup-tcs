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

        public async Task<List<ReportCountViewModel>> PeakHours(DateTime startWeek, DateTime endWeek)
        {
            var result = new List<ReportCountViewModel>();
            var realResult = new List<ReportCountViewModel>();
            var count = 0;

            while(startWeek <= endWeek)
            {
                while (count <= 23 )
                {
                    result.Add(new ReportCountViewModel
                    {
                        Item = count,
                        Count = await _db.SignIns.Where(x => x.InTime >= startWeek && x.InTime <= startWeek.AddDays(7) && x.InTime.Value.Hour == count).CountAsync()
                    });
                    count++;
                }

                count = 0;
                startWeek = startWeek.AddDays(7);
            }

            var hourCount = 0;
            var inCount = 0;
            while(hourCount < 24)
            {
                foreach (ReportCountViewModel rc in result)
                {
                    if (rc.Item == hourCount)
                    {
                        inCount += rc.Count;
                    }
                }

                realResult.Add(new ReportCountViewModel
                {
                    Item = hourCount,
                    Count = inCount
                });
                hourCount++;
                inCount = 0;
            }
            
            return realResult;
       }

        public async Task<List<ClassTourReportViewModel>> ClassTours(DateTime startWeek, DateTime endWeek)
        {
            var result = new List<ClassTourReportViewModel>();
            var temp = new List<ClassTour>();

            while (startWeek <= endWeek)
            {
                var tours = _db.ClassTours.Where(x => x.DayVisited >= startWeek && x.DayVisited <= startWeek.AddDays(7)).ToList();
                
                foreach(ClassTour t in tours)
                {
                   foreach(ClassTour tour in temp)
                    {
                        if (tour != null)
                        {
                            if(t.Name == tour.Name)
                            {
                                tour.NumberOfStudents += t.NumberOfStudents;
                                t.Name = null;
                            }
                        }
                    }
                   if(t.Name != null)
                    {
                        temp.Add(t);
                    }
                }

                startWeek = startWeek.AddDays(7);
            }
            
            foreach(ClassTour t in temp)
            {                
                result.Add(new ClassTourReportViewModel {
                    Name = t.Name,
                    Students = t.NumberOfStudents
                });
            }

            return result;
        }
    }
}
