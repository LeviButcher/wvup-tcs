using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        }
    }
}
