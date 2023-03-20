using AutoMapper;
using Vulcanova.Core.Mapping;
using Vulcanova.Features.Timetable.Changes;
using Vulcanova.Uonet.Api.Common.Models;
using Vulcanova.Uonet.Api.Schedule;

namespace Vulcanova.Features.Timetable;

public class TimetableMapperProfile : Profile
{
    public TimetableMapperProfile()
    {
        CreateMap<TimeSlot, TimetableTimeSlot>()
            .ForMember(dest => dest.Start,
                cfg => cfg.ConvertUsing(TimeZoneAwareTimeConverter.Instance, src => src.Start))
            .ForMember(dest => dest.End,
                cfg => cfg.ConvertUsing(TimeZoneAwareTimeConverter.Instance, src => src.End));

        CreateMap<ScheduleEntryPayload, TimetableEntry>()
            .ForMember(dest => dest.RoomName, cfg => cfg.MapFrom(src => src.Room.Code))
            .ForMember(dest => dest.TeacherName, cfg => cfg.MapFrom(src => src.TeacherPrimary.DisplayName));

        CreateMap<ScheduleChangeEntryPayload, TimetableChangeEntry>()
            .ForMember(dest => dest.RoomName, cfg => cfg.MapFrom(src => src.Room.Code))
            .ForMember(dest => dest.TeacherName, cfg => cfg.MapFrom(src => src.TeacherPrimary.DisplayName))
            .ForMember(dest => dest.TimetableEntryId, cfg => cfg.MapFrom(src => src.ScheduleId));
    }
}