using System;

namespace Vulcanova.Features.Timetable.NativeWidget;

public sealed class TimetableWidgetUpdateDataModel
{
    public int No { get; set; }
    public string SubjectName { get; set; }
    public string TeacherName { get; set; }
    public string Event { get; set; }
    public DateTime Date { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string RoomName { get; set; }

    public static TimetableWidgetUpdateDataModel FromTimetableListEntry(TimetableListEntry listEntry) =>
        new()
        {
            No = listEntry.No.Value,
            SubjectName = listEntry.SubjectName.Value ?? listEntry.Event.Value,
            TeacherName = listEntry.TeacherName.Value,
            Date = listEntry.Date.Value,
            Start = listEntry.Start.Value,
            End = listEntry.End.Value,
            RoomName = listEntry.RoomName.Value
        };
}