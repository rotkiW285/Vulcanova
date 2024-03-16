using Prism.Ioc;
using Vulcanova.Features.Timetable.Changes;

namespace Vulcanova.Features.Timetable;

public static class Config
{
    public static void RegisterTimetable(this IContainerRegistry container)
    {
        container.RegisterForNavigation<TimetableView>();
        container.RegisterForNavigation<TimetableEntryDetailsView>();

        container.RegisterScoped<ITimetableEntryRepository, TimetableEntryRepository>();
        container.RegisterScoped<ITimetableEntryService, TimetableEntryService>();

        container.RegisterScoped<ITimetableChangesRepository, TimetableChangesRepository>();
        container.RegisterScoped<ITimetableChangesService, TimetableChangesService>();

        container.RegisterScoped<ITimetableProvider, TimetableProvider>();
    }
}