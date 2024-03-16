using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vulcanova.Features.Timetable;

public interface ITimetableEntryRepository
{
    Task<IEnumerable<TimetableEntry>> GetEntriesForPupilAsync(int accountId, int pupilId, DateTime from, DateTime to);

    Task UpdatePupilEntriesAsync(int accountId, int pupilId, IEnumerable<TimetableEntry> entries, DateTime from,
        DateTime to);
}