using AutoMapper;
using Vulcanova.Core.Data;
using Vulcanova.Core.Mapping;
using Vulcanova.Uonet.Api.Lessons;

namespace Vulcanova.Features.Attendance;

public class AttendanceMapperProfile : Profile
{
    public AttendanceMapperProfile()
    {
        CreateMap<LessonPayload, Lesson>()
            .ForMember(e => e.Id, cfg => cfg.MapFrom(src => new AccountEntityId { VulcanId = src.Id }))
            .ForMember(dest => dest.TeacherName, cfg => cfg.MapFrom(src => src.TeacherPrimary.DisplayName))
            .ForMember(dest => dest.Date, cfg => cfg.MapFrom(src => src.Day))
            .ForMember(dest => dest.No, cfg => cfg.MapFrom(src => src.TimeSlot.Position))
            .ForMember(dest => dest.Start,
                cfg => cfg.ConvertUsing(TimeZoneAwareTimeConverter.Instance, src => src.TimeSlot.Start))
            .ForMember(dest => dest.End,
                cfg => cfg.ConvertUsing(TimeZoneAwareTimeConverter.Instance, src => src.TimeSlot.End));

        CreateMap<Vulcanova.Uonet.Api.Lessons.PresenceType, PresenceType>();
    }
}