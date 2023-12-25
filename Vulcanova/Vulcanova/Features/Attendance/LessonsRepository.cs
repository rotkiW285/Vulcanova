using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB.Async;
using Vulcanova.Features.Auth;

namespace Vulcanova.Features.Attendance;

public class LessonsRepository : ILessonsRepository, IHasAccountRemovalCleanup
{
    private readonly LiteDatabaseAsync _db;

    public LessonsRepository(LiteDatabaseAsync db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Lesson>> GetLessonsForAccountAsync(int accountId, DateTime monthAndYear)
    {
        return await _db.GetCollection<Lesson>()
            .FindAsync(g =>
                g.Id.AccountId == accountId && g.Date.Year == monthAndYear.Year && g.Date.Month == monthAndYear.Month);
    }

    public async Task<IEnumerable<Lesson>> GetLessonsBetweenAsync(int accountId, DateTime start, DateTime end)
    {
        return await _db.GetCollection<Lesson>()
            .FindAsync(g => g.Id.AccountId == accountId && g.Date >= start && g.Date <= end);
    }

    public async Task UpsertLessonsForAccountAsync(IEnumerable<Lesson> entries, int accountId, DateTime monthAndYear)
    {
        await _db.GetCollection<Lesson>()
            .DeleteManyAsync(g =>
                g.Date.Year == monthAndYear.Year && g.Date.Month == monthAndYear.Month && g.Id.AccountId == accountId);

        await _db.GetCollection<Lesson>().UpsertAsync(entries);
    }

    async Task IHasAccountRemovalCleanup.DoPostRemovalCleanUpAsync(int accountId)
    {
        await _db.GetCollection<Lesson>().DeleteManyAsync(l => l.Id.AccountId == accountId);
    }
}