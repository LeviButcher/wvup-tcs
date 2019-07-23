using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            var signInsBetweenDate = _db.SignIns.Where(x => x.InTime >= start && x.InTime <= end)
                .Include(x => x.Courses).ThenInclude(x => x.Course)
                .Include(x => x.Reasons).ThenInclude(x => x.Reason);

            var totalDataCount = await signInsBetweenDate.CountAsync();
            var pageData = signInsBetweenDate
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

            return new PagingModel<SignInViewModel>(skip, take, totalDataCount, pageData);
        }
    }
}
