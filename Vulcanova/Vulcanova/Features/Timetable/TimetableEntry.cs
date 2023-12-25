using System;
using Vulcanova.Core.Data;
using Vulcanova.Features.Shared;

namespace Vulcanova.Features.Timetable;

public class TimetableEntry
{
    public AccountEntityId Id { get; set; }
    public TimetableTimeSlot TimeSlot { get; set; }
    public int PupilId { get; set; }
    public string RoomName { get; set; }
    public string TeacherName { get; set; }
    public DateTime Date { get; set; }
    public Subject Subject { get; set; }
    public string Event { get; set; }
    public bool Visible { get; set; }
    public int PeriodId { get; set; }
}