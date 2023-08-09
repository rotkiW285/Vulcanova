using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB.Async;
using Vulcanova.Features.Auth;

namespace Vulcanova.Features.Timetable;

public class TimetableRepository : ITimetableRepository, IHasAccountRemovalCleanup
{
    private readonly LiteDatabaseAsync _db;

    public TimetableRepository(LiteDatabaseAsync db)
    {
        _db = db;
    }

    public async Task<IEnumerable<TimetableEntry>> GetEntriesForPupilAsync(int accountId, int pupilId,
        DateTime monthAndYear)
    {
        return await _db.GetCollection<TimetableEntry>()
            .FindAsync(g =>
                g.PupilId == pupilId && g.AccountId == accountId && g.Date.Year == monthAndYear.Year &&
                g.Date.Month == monthAndYear.Month);
    }

    public async Task UpdatePupilEntriesAsync(IEnumerable<TimetableEntry> entries, DateTime monthAndYear)
    {
        await _db.GetCollection<TimetableEntry>()
            .DeleteManyAsync(g => g.Date.Year == monthAndYear.Year &&
                                  g.Date.Month == monthAndYear.Month);

        await _db.GetCollection<TimetableEntry>().UpsertAsync(entries);
    }

    async Task IHasAccountRemovalCleanup.DoPostRemovalCleanUpAsync(int accountId)
    {
        await _db.GetCollection<TimetableEntry>().DeleteManyAsync(n => n.AccountId == accountId);
    }
}