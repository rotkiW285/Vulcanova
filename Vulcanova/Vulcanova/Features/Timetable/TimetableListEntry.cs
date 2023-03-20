using System;
using Vulcanova.Uonet.Api.Schedule;

namespace Vulcanova.Features.Timetable;

public class TimetableListEntry
{
    public int? OriginalId { get; set; }
    public DateTime Date { get; set; }
    public int No { get; set; }
    public string SubjectName { get; set; }
    public string TeacherName { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string RoomName { get; set; }
    public ChangeDetails Change { get; set; }

    public class ChangeDetails
    {
        public ChangeType ChangeType { get; set; }
        public RescheduleKind? RescheduleKind { get; set; }
        public string ChangeNote { get; set; }
    }

    public enum RescheduleKind
    {
        Removed,
        Added
    }
}