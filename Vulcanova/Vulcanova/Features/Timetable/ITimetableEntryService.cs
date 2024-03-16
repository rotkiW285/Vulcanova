using System;
using System.Collections.Generic;

namespace Vulcanova.Features.Timetable;

public interface ITimetableEntryService
{
    IObservable<IEnumerable<TimetableEntry>> GetEntries(int accountId, DateTime from, DateTime to,
        bool forceSync = false);
}