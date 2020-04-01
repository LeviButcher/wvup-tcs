using System.Linq;
using AutoMapper;
using tcs_service.Models;
using tcs_service.Models.DTO;
using tcs_service.Models.DTOs;

namespace tcs_service.Helpers
{
    ///<summary>AutoMapperProfile</summary>
    /// AutoMapperProfiles sets up AutoMapper to automatically Convert One type to another type
    /// in a specific way
    public class AutoMapperProfile : Profile
    {
        ///<summary>AutoMapperProfile Constructor</summary>
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<Session, SessionPostOrPutDTO>()
                .ForMember(dest => dest.SelectedClasses, opts =>
                   opts.MapFrom(src => src.SessionClasses.Select(x => x.Class)));
            CreateMap<SessionPostOrPutDTO, Session>();
            CreateMap<Session, SignInDTO>()
                .ForMember(dest => dest.Classes, opts =>
                   opts.MapFrom(src => src.SessionClasses.Select(x => x.Class)));
            CreateMap<SignInDTO, Session>()
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