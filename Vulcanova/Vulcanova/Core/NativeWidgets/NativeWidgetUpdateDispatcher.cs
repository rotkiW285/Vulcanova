using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using Vulcanova.Core.Uonet;
using Vulcanova.Features.Attendance.Report;
using Vulcanova.Features.Timetable;
using Vulcanova.Features.Timetable.Changes;

namespace Vulcanova.Core.NativeWidgets;

public sealed class NativeWidgetUpdateDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public NativeWidgetUpdateDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Setup()
    {
        Listen<AttendanceReportUpdatedEvent>();
        Listen<TimetableUpdatedEvent>();
        Listen<TimetableChangesUpdatedEvent>();
    }

    private void Listen<TEvent>() where TEvent : UonetDataUpdatedEvent
    {
        MessageBus.Current.Listen<TEvent>()
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Throttle(TimeSpan.FromSeconds(5))
            .SelectMany(async @event => await ProcessEvent(@event))
            .Subscribe();
    }

    private async Task<Unit> ProcessEvent<T>(T @event) where T : UonetDataUpdatedEvent
    {
        var updater = (IWidgetUpdater<T>)_serviceProvider.GetService(typeof(IWidgetUpdater<T>));

        if (updater is null)
        {
            return default;
        }

        await updater.Handle(@event);

        return default;
    }
}