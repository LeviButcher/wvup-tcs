using AutoMapper;
using System.Linq;
using tcs_service.Models;
using tcs_service.Models.DTOs;
using tcs_service.Models.ViewModels;

namespace tcs_service.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<Session, SessionCreateDTO>()
                .ForMember(dest => dest.SelectedClasses, opts =>
                opts.MapFrom(src => src.SessionClasses.Select(x => x.Class)));
            CreateMap<SessionCreateDTO, Session>();
            CreateMap<Session, SignInViewModel>()
                .ForMember(dest => dest.Classes, opts =>
                    opts.MapFrom(src => src.SessionClasses.Select(x => x.Class)));
            CreateMap<SignInViewModel, Session>()
                .ForMember(dest => dest.SessionClasses, opts =>
                    opts.MapFrom(src => src.Classes.Select(x => new SessionClass() { Class = x })))
                .ForMember(dest => dest.SessionReasons, opts =>
                    opts.MapFrom(src => src.Reasons.Select(x => new SessionReason() { Reason = x })));
        }
    }
}
