using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tcs_service.Models.DTO;

namespace tcs_service.Repos.Interfaces
{
    public interface IReportsRepo
    {
        Task<List<WeeklyVisitsDTO>> WeeklyVisits(DateTime startWeek, DateTime endWeek);

        Task<List<PeakHoursDTO>> PeakHours(DateTime startWeek, DateTime endWeek);

        Task<List<ClassTourReportDTO>> ClassTours(DateTime startWeek, DateTime endWeek);

        Task<List<TeacherSignInTimeDTO>> Volunteers(DateTime startWeek, DateTime endWeek);

        Task<List<ReasonWithClassVisitsDTO>> Reasons(DateTime startWeek, DateTime endWeek);

        Task<List<ClassWithGradeDTO>> SuccessReport(int semesterId);

    }
}
