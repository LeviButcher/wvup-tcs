using AutoMapper;
using System.Linq;
using tcs_service.Models;
using tcs_service.Models.ViewModels;

namespace tcs_service.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<SignIn, SignInViewModel>()
                .ForMember(dest => dest.Courses, opts =>
                    opts.MapFrom(src => src.Courses.Select(x => x.Course)));
            CreateMap<SignInViewModel, SignIn>()
                .ForMember(dest => dest.Courses, opts =>
                    opts.MapFrom(src => src.Courses.Select(x => new SignInCourse() { Course = x })))
                .ForMember(dest => dest.Reasons, opts =>
                    opts.MapFrom(src => src.Reasons.Select(x => new SignInReason() { Reason = x })));
            CreateMap<BannerInformation, StudentInfoViewModel>()
                .ForMember(dest => dest.classSchedule, opts => opts.MapFrom(src => src.Courses))
                .ForMember(dest => dest.studentID, opts => opts.MapFrom(src => src.WVUPID))
                .ForMember(dest => dest.semesterId, opts => opts.MapFrom(src => src.TermCode))
                .ForMember(dest => dest.studentEmail, opts => opts.MapFrom(src => src.EmailAddress));
            CreateMap<BannerInformation, TeacherInfoViewModel>()
                .ForMember(dest => dest.teacherID, opts => opts.MapFrom(src => src.WVUPID))
                .ForMember(dest => dest.semesterId, opts => opts.MapFrom(src => src.TermCode))
                .ForMember(dest => dest.teacherEmail, opts => opts.MapFrom(src => src.EmailAddress));
        }
    }
}
