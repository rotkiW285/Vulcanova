using System.Collections.Generic;
using Vulcanova.Features.Exams;
using Vulcanova.Features.Grades;
using Vulcanova.Features.Timetable;

namespace Vulcanova.Features.Dashboard;

public class DashboardModel
{
    public IReadOnlyCollection<TimetableListEntry> Timetable { get; init; }
    public IReadOnlyCollection<Exam> Exams { get; init; }
    public IReadOnlyCollection<Homework.Homework> Homework { get; init; }
    public IReadOnlyCollection<Grade> Grades { get; init; }
    public LuckyNumber.LuckyNumber LuckyNumber { get; init; }
}