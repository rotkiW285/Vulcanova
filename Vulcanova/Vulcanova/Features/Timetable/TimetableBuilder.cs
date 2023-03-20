using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Vulcanova.Features.Timetable.Changes;
using Vulcanova.Uonet.Api.Schedule;

namespace Vulcanova.Features.Timetable;

public static class TimetableBuilder
{
    public static IReadOnlyDictionary<DateTime, IReadOnlyCollection<TimetableListEntry>> BuildTimetable(
        ICollection<TimetableEntry> lessons, ICollection<TimetableChangeEntry> changes)
    {
        var timetable = lessons.Where(l => l.Visible)
            .Select(lesson => new TimetableListEntry
            {
                OriginalId = lesson.Id,
                Date = lesson.Date.Date,
                No = lesson.TimeSlot.Position,
                Start = lesson.TimeSlot.Start,
                End = lesson.TimeSlot.End,
                SubjectName = lesson.Subject?.Name,
                RoomName = lesson.RoomName,
                TeacherName = lesson.TeacherName
            })
            .ToList();

        foreach (var change in changes)
        {
            var lessonToUpdate = timetable.SingleOrDefault(l => l.OriginalId == change.TimetableEntryId);

            if (lessonToUpdate != null)
            {
                lessonToUpdate.SubjectName = change.Subject?.Name ?? lessonToUpdate.SubjectName;
                lessonToUpdate.RoomName = change.RoomName ?? lessonToUpdate.RoomName;
                lessonToUpdate.TeacherName = change.TeacherName ?? lessonToUpdate.TeacherName;
                lessonToUpdate.Change = new TimetableListEntry.ChangeDetails
                {
                    ChangeNote = change.Note ?? change.Reason,
                    ChangeType = change.Change.Type,
                };

                if (change.Change.Type is ChangeType.Rescheduled)
                {
                    lessonToUpdate.Change.RescheduleKind = TimetableListEntry.RescheduleKind.Removed;
                }
            }

            
            if (change.Change.Type is ChangeType.Rescheduled
                // for now a hack to not display "ghost" lessons
                && (change.Subject != null || lessonToUpdate != null))
            {
                timetable.Add(new TimetableListEntry
                {
                    No = change.TimeSlot?.Position ?? lessonToUpdate?.No ?? 0,
                    Start = change.TimeSlot?.Start ?? lessonToUpdate?.Start ?? DateTime.MaxValue,
                    End = change.TimeSlot?.End ?? lessonToUpdate?.End ?? DateTime.MaxValue,
                    Date = change.ChangeDate?.Date ?? change.LessonDate.Date,
                    SubjectName = change.Subject?.Name ?? lessonToUpdate?.SubjectName,
                    RoomName = change.RoomName ?? lessonToUpdate?.RoomName,
                    TeacherName = change.TeacherName ?? lessonToUpdate?.TeacherName,
                    Change = new TimetableListEntry.ChangeDetails
                    {
                        ChangeNote = change.Note ?? change.Reason,
                        ChangeType = change.Change.Type,
                        RescheduleKind = TimetableListEntry.RescheduleKind.Added
                    }
                });
            }
        }

        var result = timetable
            .GroupBy(l => l.Date)
            .ToDictionary(l => l.Key,
                l => (IReadOnlyCollection<TimetableListEntry>) l.OrderBy(x => x.No)
                    .ThenByDescending(x => x.Change != null).ToList().AsReadOnly());

        return new ReadOnlyDictionary<DateTime, IReadOnlyCollection<TimetableListEntry>>(result);
    }
}