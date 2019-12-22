using System.Runtime.InteropServices.ComTypes;
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
            CreateMap<Session, SessionPostOrPutDTO>()
                .ForMember(dest => dest.SelectedClasses, opts =>
                opts.MapFrom(src => src.SessionClasses.Select(x => x.Class)));
            CreateMap<SessionPostOrPutDTO, Session>();
            CreateMap<Session, SignInViewModel>()
                .ForMember(dest => dest.Classes, opts =>
                    opts.MapFrom(src => src.SessionClasses.Select(x => x.Class)));
            CreateMap<SignInViewModel, Session>()
                .ForMember(dest => dest.SessionClasses, opts =>
                    opts.MapFrom(src => src.Classes.Select(x => new SessionClass() { Class = x })))
                .ForMember(dest => dest.SessionReasons, opts =>
                    opts.MapFrom(src => src.Reasons.Select(x => new SessionReason() { Reason = x })));

            CreateMap<Session, SessionInfoDTO>()
                .ForMember(dest => dest.SelectedClasses, opts =>
                opts.MapFrom(src => src.SessionClasses.Select(x => x.ClassId)))
                .ForMember(dest => dest.SelectedReasons, opts =>
                opts.MapFrom(dest => dest.SessionReasons.Select(x => x.ReasonId)))
                .ForMember(dest => dest.PersonType, opts => opts.MapFrom(src => src.Person.PersonType))
                .ForMember(dest => dest.Email, opts => opts.MapFrom(src => src.Person.Email))
                .ForMember(dest => dest.FirstName, opts => opts.MapFrom(src => src.Person.FirstName))
                .ForMember(dest => dest.LastName, opts => opts.MapFrom(src => src.Person.LastName))
                .ForMember(dest => dest.Schedule, opts =>
                opts.MapFrom(src => src.Person.Schedules.Select(x => x.Class)));

            CreateMap<Session, SessionDisplayDTO>()
            .ForMember(dest => dest.SelectedClasses, opts =>
                opts.MapFrom(src => src.SessionClasses.Select(x => x.Class)))
            .ForMember(dest => dest.SelectedReasons, opts =>
                opts.MapFrom(src => src.SessionReasons.Select(x => x.Reason)));

            CreateMap<Person, PersonDisplayDTO>();
            CreateMap<Reason, ReasonDisplayDTO>();
            CreateMap<Class, ClassDisplayDTO>();
            CreateMap<Semester, SemesterDisplayDTO>();
        }
    }
}
