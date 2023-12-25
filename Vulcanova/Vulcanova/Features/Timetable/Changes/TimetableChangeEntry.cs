using System;
using Vulcanova.Core.Data;
using Vulcanova.Uonet.Api.Schedule;

namespace Vulcanova.Features.Timetable.Changes;

public class TimetableChangeEntry
{
    public AccountEntityId Id { get; set; }
    public int TimetableEntryId { get; set; }
    public int UnitId { get; set; }
    public int PupilId { get; set; }
    public Shared.Subject Subject { get; set; }
    public DateTime LessonDate { get; set; }
    public DateTime? ChangeDate { get; set; }
    public TimetableTimeSlot TimeSlot { get; set; }
    public string Note { get; set; }
    public string Event { get; set; }
    public string Reason { get; set; }
    public string TeacherName { get; set; }
    public string RoomName { get; set; }
    public Change Change { get; set; }
}