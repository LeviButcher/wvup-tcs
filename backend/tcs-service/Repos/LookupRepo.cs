using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.EF;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Models.ViewModels;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos
{
    public class LookupRepo : ILookupRepo
    {
        protected TCSContext _db;

        public LookupRepo()
        {
            _db = new TCSContext();
        }

        public LookupRepo(DbContextOptions options)
        {
            _db = new TCSContext(options);
        }

        public async Task<PagingModel<SignInViewModel>> Get(DateTime start, DateTime end, int? crn, string email, int skip, int take)
        {

            var sessionsBetweenDate = _db.Sessions.OrderBy(x => x.InTime)
               .Include(x => x.SessionClasses).ThenInclude(x => x.Class)
               .Include(x => x.SessionReasons).ThenInclude(x => x.Reason)
               .Where(x => x.InTime >= start && x.InTime <= end);

            if (crn != null)
            {
                sessionsBetweenDate = sessionsBetweenDate.Where(x => x.SessionClasses.Any(y => y.ClassId == crn));
            }

            if (email != null && !email.Equals(""))
            {
                sessionsBetweenDate = sessionsBetweenDate.Where(x => x.Person.Email.Equals(email));
            }

            var totalDataCount = await sessionsBetweenDate.CountAsync();
            var pageData = GetPageData(sessionsBetweenDate, skip, take);

            return new PagingModel<SignInViewModel>(skip, take, totalDataCount, pageData);
        }

        public async Task<PagingModel<SignInViewModel>> Daily(int skip, int take)
        {
            var dailySignIns = _db.Sessions
                .OrderByDescending(x => x.InTime)
                .Where(x => x.InTime.Value.Day == DateTime.UtcNow.Day
                && x.InTime.Value.Month == DateTime.UtcNow.Month
                && x.InTime.Value.Year == DateTime.UtcNow.Year)
               .Include(x => x.SessionClasses).ThenInclude(x => x.Class)
               .Include(x => x.SessionReasons).ThenInclude(x => x.Reason);

            var totalDataCount = await dailySignIns.CountAsync();
            var pageData = GetPageData(dailySignIns, skip, take);

            return new PagingModel<SignInViewModel>(skip, take, totalDataCount, pageData);
        }

        public async Task<PagingModel<SignInViewModel>> GetByCRN(int crn, DateTime start, DateTime end, int skip, int take)
        {
            var signInsBetweenDateByCRN = _db.Sessions.Where(x => x.InTime >= start && x.InTime <= end && x.SessionClasses.Any(y => y.ClassId == crn))
                .Include(x => x.SessionClasses).ThenInclude(x => x.Class)
                .Include(x => x.SessionReasons).ThenInclude(x => x.Reason);

            var totalDataCount = await signInsBetweenDateByCRN.CountAsync();
            var pageData = GetPageData(signInsBetweenDateByCRN, skip, take);

            return new PagingModel<SignInViewModel>(skip, take, totalDataCount, pageData);
        }

        public async Task<PagingModel<SignInViewModel>> GetByEmail(string email, DateTime start, DateTime end, int skip, int take)
        {
            var signInsBetweenDateByEmail = _db.Sessions.Where(x => x.InTime >= start && x.InTime <= end && x.Person.Email == email)
                .Include(x => x.SessionClasses).ThenInclude(x => x.Class)
                .Include(x => x.SessionReasons).ThenInclude(x => x.Reason);

            var totalDataCount = await signInsBetweenDateByEmail.CountAsync();
            var pageData = GetPageData(signInsBetweenDateByEmail, skip, take);

            return new PagingModel<SignInViewModel>(skip, take, totalDataCount, pageData);
        }

        private IQueryable<SignInViewModel> GetPageData(IQueryable<Session> signIns, int skip, int take)
        {
            return signIns
                .Skip(skip).Take(take)
                .Select(x => new SignInViewModel()
                {
                    Email = x.Person.Email,
                    FirstName = x.Person.FirstName,
                    FullName = x.Person.FirstName + " " + x.Person.LastName,
                    LastName = x.Person.LastName,
                    InTime = x.InTime,
                    OutTime = x.OutTime,
                    SemesterName = x.Semester.Name,
                    Tutoring = x.Tutoring,
                    SemesterId = x.Semester.Code,
                    PersonId = x.PersonId,
                    Id = x.Id,
                    Classes = x.SessionClasses.Select(signInCourse => signInCourse.Class).ToList(),
                    Reasons = x.SessionReasons.Select(signInReason => signInReason.Reason).ToList(),
                    Type = x.Person.PersonType
                });
        }

        public async Task<List<SignInSpreadSheetViewModel>> GetBySemester(int semesterId)
            => await _db.Sessions.Include(x => x.Person).Include(x => x.SessionClasses)
                        .ThenInclude(x => x.Class).Include(x => x.SessionReasons).ThenInclude(x => x.Reason)
                        .Where(x => x.SemesterCode == semesterId)
                        .Select(x => new SignInSpreadSheetViewModel(x.Person.PersonType, x.SessionClasses.Select(c => c.Class), x.SessionReasons.Select(r => r.Reason), x.Tutoring)
                        {
                            Email = x.Person.Email,
                            InTime = x.InTime,
                            LastName = x.Person.LastName,
                            FirstName = x.Person.FirstName,
                            OutTime = x.OutTime,
                            WVUPId = x.Person.Id,
                        })
                        .ToListAsync();
    }
}
