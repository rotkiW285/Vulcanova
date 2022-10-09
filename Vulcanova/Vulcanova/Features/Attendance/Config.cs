using Prism.Ioc;
using Vulcanova.Features.Attendance.Justification;
using Vulcanova.Features.Attendance.LessonDetails;

namespace Vulcanova.Features.Attendance;

public static class Config
{
    public static void RegisterAttendance(this IContainerRegistry container)
    {
        container.RegisterScoped<ILessonsRepository, LessonsRepository>();
        container.RegisterScoped<ILessonsService, LessonsService>();

        container.RegisterForNavigation<LessonDetailsView>();
        
        container.RegisterForNavigation<JustifyAbsenceView>();
    }
}