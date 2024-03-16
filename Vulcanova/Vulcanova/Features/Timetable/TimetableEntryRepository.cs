using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB.Async;
using Vulcanova.Features.Auth;

namespace Vulcanova.Features.Timetable;

public class TimetableEntryRepository : ITimetableEntryRepository, IHasAccountRemovalCleanup
{
    private readonly LiteDatabaseAsync _db;

    public TimetableEntryRepository(LiteDatabaseAsync db)
    {
        _db = db;
    }

    public async Task<IEnumerable<TimetableEntry>> GetEntriesForPupilAsync(int accountId, int pupilId,
        DateTime from, DateTime to)
    {
        return await _db.GetCollection<TimetableEntry>()
            .FindAsync(g =>
                g.PupilId == pupilId && g.Id.AccountId == accountId && g.Date.Date >= from.Date &&
                g.Date.Date <= to.Date);
    }

    public async Task UpdatePupilEntriesAsync(int accountId, int pupilId, IEnumerable<TimetableEntry> entries, DateTime from,
        DateTime to)
    {
        await _db.GetCollection<TimetableEntry>()
            .DeleteManyAsync(g => g.PupilId == pupilId && g.Id.AccountId == accountId &&
                                  g.Date.Date >= from.Date && g.Date.Date <= to.Date);

        await _db.GetCollection<TimetableEntry>().UpsertAsync(entries);
    }

    async Task IHasAccountRemovalCleanup.DoPostRemovalCleanUpAsync(int accountId)
    {
        await _db.GetCollection<TimetableEntry>().DeleteManyAsync(n => n.Id.AccountId == accountId);
    }
}