using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB.Async;
using Vulcanova.Features.Auth;

namespace Vulcanova.Features.Timetable.Changes;

public class TimetableChangesRepository : ITimetableChangesRepository, IHasAccountRemovalCleanup
{
    private readonly LiteDatabaseAsync _db;

    public TimetableChangesRepository(LiteDatabaseAsync db)
    {
        _db = db;
    }

    public async Task<IEnumerable<TimetableChangeEntry>> GetEntriesForPupilAsync(int accountId, int pupilId,
        DateTime monthAndYear)
    {
        return await _db.GetCollection<TimetableChangeEntry>()
            .FindAsync(g =>
                g.PupilId == pupilId && g.AccountId == accountId && g.LessonDate.Year == monthAndYear.Year &&
                (g.LessonDate.Month == monthAndYear.Month || (g.ChangeDate != null && g.ChangeDate.Value.Month == monthAndYear.Month)));
    }

    public async Task UpsertEntriesAsync(IEnumerable<TimetableChangeEntry> entries, DateTime monthAndYear)
    {
        await _db.GetCollection<TimetableChangeEntry>()
            .DeleteManyAsync(g => g.LessonDate.Year == monthAndYear.Year &&
                                  g.LessonDate.Month == monthAndYear.Month);

        await _db.GetCollection<TimetableChangeEntry>().UpsertAsync(entries);
    }

    async Task IHasAccountRemovalCleanup.DoPostRemovalCleanUpAsync(int accountId)
    {
        await _db.GetCollection<TimetableChangeEntry>().DeleteManyAsync(n => n.AccountId == accountId);
    }
}