using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.EF;
using tcs_service.Helpers;
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

        public async Task<PagingModel<SignInViewModel>> Get(DateTime start, DateTime end, int skip, int take)
        {
            var signInsBetweenDate = _db.SignIns.OrderBy(x => x.InTime)
                .Where(x => x.InTime >= start && x.InTime <= end)
                .Include(x => x.Courses).ThenInclude(x => x.Course)
                .Include(x => x.Reasons).ThenInclude(x => x.Reason);

            var totalDataCount = await signInsBetweenDate.CountAsync();
            var pageData = GetPageData(signInsBetweenDate, skip, take);

            return new PagingModel<SignInViewModel>(skip, take, totalDataCount, pageData);
        }

        public async Task<PagingModel<SignInViewModel>> Daily(int skip, int take)
        {
            var dailySignIns = _db.SignIns
                .OrderByDescending(x => x.InTime)
                .Where(x => x.InTime.Value.Day == DateTime.Now.Day
                && x.InTime.Value.Month == DateTime.Now.Month
                && x.InTime.Value.Year == DateTime.Now.Year)
               .Include(x => x.Courses).ThenInclude(x => x.Course)
               .Include(x => x.Reasons).ThenInclude(x => x.Reason);

            var totalDataCount = await dailySignIns.CountAsync();
            var pageData = GetPageData(dailySignIns, skip, take);

            return new PagingModel<SignInViewModel>(skip, take, totalDataCount, pageData);
        }

        public async Task<PagingModel<SignInViewModel>> GetByCRN(int crn, DateTime start, DateTime end, int skip, int take)
        {
            var signInsBetweenDateByCRN = _db.SignIns.Where(x => x.InTime >= start && x.InTime <= end && x.Courses.Any(y => y.CourseID == crn))
                .Include(x => x.Courses).ThenInclude(x => x.Course)
                .Include(x => x.Reasons).ThenInclude(x => x.Reason);

            var totalDataCount = await signInsBetweenDateByCRN.CountAsync();
            var pageData = GetPageData(signInsBetweenDateByCRN, skip, take);

            return new PagingModel<SignInViewModel>(skip, take, totalDataCount, pageData);
        }

        public async Task<PagingModel<SignInViewModel>> GetByEmail(string email, DateTime start, DateTime end, int skip, int take)
        {
            var signInsBetweenDateByEmail = _db.SignIns.Where(x => x.InTime >= start && x.InTime <= end && x.Person.Email == email)
                .Include(x => x.Courses).ThenInclude(x => x.Course)
                .Include(x => x.Reasons).ThenInclude(x => x.Reason);

            var totalDataCount = await signInsBetweenDateByEmail.CountAsync();
            var pageData = GetPageData(signInsBetweenDateByEmail, skip, take);

            return new PagingModel<SignInViewModel>(skip, take, totalDataCount, pageData);
        }

        private IQueryable<SignInViewModel> GetPageData(IIncludableQueryable<Models.SignIn, Models.Reason> signIns, int skip, int take)
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
                    SemesterId = x.SemesterId,
                    PersonId = x.PersonId,
                    Id = x.ID,
                    Courses = x.Courses.Select(signInCourse => signInCourse.Course).ToList(),
                    Reasons = x.Reasons.Select(signInReason => signInReason.Reason).ToList()
                });
        }
    }
}
