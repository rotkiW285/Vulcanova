using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Vulcanova.Extensions;
using Vulcanova.Features.Timetable.Changes;

namespace Vulcanova.Features.Timetable;

public sealed class TimetableProvider : ITimetableProvider
{
    private readonly ITimetableChangesService _timetableChangesService;
    private readonly ITimetableEntryService _timetableEntryService;

    public TimetableProvider(ITimetableChangesService timetableChangesService,
        ITimetableEntryService timetableEntryService)
    {
        _timetableChangesService = timetableChangesService;
        _timetableEntryService = timetableEntryService;
    }

    public IObservable<IReadOnlyDictionary<DateTime, IReadOnlyCollection<TimetableListEntry>>> GetEntries(
        int accountId, DateTime monthAndYear, bool forceSync = false) =>
        _timetableChangesService.GetChangesEntriesByMonth(accountId, monthAndYear, forceSync)
            .Select(changes =>
            {
                var timetableChangeEntries = changes as TimetableChangeEntry[] ?? changes.ToArray();

                var minDateOfTimetableEntriesToLoad = timetableChangeEntries.Min(x => (DateTime?)x.LessonDate);
                var maxDateOfTimetableEntriesToLoad = timetableChangeEntries.Max(x => (DateTime?)x.LessonDate);

                var startOfMonth = monthAndYear.AlignToStartOfMonth();
                var endOfMonth = monthAndYear.AlignToEndOfMonth();

                if (minDateOfTimetableEntriesToLoad is null || minDateOfTimetableEntriesToLoad > startOfMonth)
                {
                    minDateOfTimetableEntriesToLoad = startOfMonth;
                }

                if (maxDateOfTimetableEntriesToLoad is null || maxDateOfTimetableEntriesToLoad < endOfMonth)
                {
                    maxDateOfTimetableEntriesToLoad = endOfMonth;
                }

                return new
                {
                    Changes = timetableChangeEntries,
                    LoadTimetableEntriesFrom = minDateOfTimetableEntriesToLoad.Value,
                    LoadTimetableEntriesTo = maxDateOfTimetableEntriesToLoad.Value
                };
            })
            .Select(changesInfo => _timetableEntryService.GetEntries(accountId,
                    changesInfo.LoadTimetableEntriesFrom,
                    changesInfo.LoadTimetableEntriesTo)
                .Select(entries => new
                {
                    Entries = entries as TimetableEntry[] ?? entries.ToArray(),
                    changesInfo.Changes
                }))
            .Switch()
            .Select(x => TimetableBuilder.BuildTimetable(x.Entries, x.Changes));
}