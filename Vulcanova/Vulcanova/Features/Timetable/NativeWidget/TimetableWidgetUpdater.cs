using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Vulcanova.Core.NativeWidgets;
using Vulcanova.Core.Uonet;
using Vulcanova.Features.Timetable.Changes;

namespace Vulcanova.Features.Timetable.NativeWidget;

public sealed class TimetableWidgetUpdater : IWidgetUpdater<TimetableUpdatedEvent>, IWidgetUpdater<TimetableChangesUpdatedEvent>
{
    private readonly ITimetableService _timetableService;
    private readonly ITimetableChangesService _timetableChangesService;
    private readonly INativeWidgetProxy _widgetProxy;

    public TimetableWidgetUpdater(ITimetableChangesService timetableChangesService, ITimetableService timetableService, INativeWidgetProxy widgetProxy)
    {
        _timetableChangesService = timetableChangesService;
        _timetableService = timetableService;
        _widgetProxy = widgetProxy;
    }

    public async Task Handle(TimetableUpdatedEvent message)
    {
        await UpdateWidgetAsync(message.AccountId);
    }

    public async Task Handle(TimetableChangesUpdatedEvent message)
    {
        await UpdateWidgetAsync(message.AccountId);
    }

    private async Task UpdateWidgetAsync(int accountId)
    {
        var now = DateTime.Now;

        var (changes, timetableEntries) = await _timetableChangesService
            .GetChangesEntriesByMonth(accountId, now)
            .CombineLatest(_timetableService.GetPeriodEntriesByMonth(accountId, now))
            .Select(tuple => (tuple.First.ToArray(), tuple.Second.ToArray()));

        var timetable = TimetableBuilder.BuildTimetable(timetableEntries, changes);

        var timetableForNext7Days =
            Enumerable.Range(0, 7).Select(d => now.AddDays(d))
                .Select(d =>
                {
                    var key = d.Date;
                    var lessons = timetable.ContainsKey(key)
                        ? timetable[key].Select(TimetableWidgetUpdateDataModel.FromTimetableListEntry)
                        : Array.Empty<TimetableWidgetUpdateDataModel>();

                    return new KeyValuePair<DateTime, IEnumerable<TimetableWidgetUpdateDataModel>>(key, lessons);
                });

        _widgetProxy.UpdateWidgetState(INativeWidgetProxy.NativeWidget.Timetable, timetableForNext7Days);
    }
}