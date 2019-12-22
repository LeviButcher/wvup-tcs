using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Models;
using tcs_service.Models.ViewModels;

namespace tcs_service.Repos.Interfaces
{
    public interface IReportsRepo
    {
        Task<List<WeeklyVisitsViewModel>> WeeklyVisits(DateTime startWeek, DateTime endWeek);

        Task<List<PeakHoursViewModel>> PeakHours(DateTime startWeek, DateTime endWeek);

        Task<List<ClassTourReportViewModel>> ClassTours(DateTime startWeek, DateTime endWeek);

        Task<List<TeacherSignInTimeViewModel>> Volunteers(DateTime startWeek, DateTime endWeek);

        Task<List<ReasonWithClassVisitsViewModel>> Reasons(DateTime startWeek, DateTime endWeek);

        Task<List<CourseWithGradeViewModel>> SuccessReport(int semesterId);

    }
}
