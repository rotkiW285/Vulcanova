using System;
using System.Collections.Generic;

namespace Vulcanova.Features.Timetable;

public interface ITimetableProvider
{
    IObservable<IReadOnlyDictionary<DateTime, IReadOnlyCollection<TimetableListEntry>>> GetEntries(
        int accountId, DateTime monthAndYear, bool forceSync = false);
}