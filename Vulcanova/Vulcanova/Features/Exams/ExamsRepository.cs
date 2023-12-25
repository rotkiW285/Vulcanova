using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB.Async;
using Vulcanova.Features.Auth;

namespace Vulcanova.Features.Exams;

public class ExamsRepository : IExamsRepository, IHasAccountRemovalCleanup
{
    private readonly LiteDatabaseAsync _db;

    public ExamsRepository(LiteDatabaseAsync db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Exam>> GetExamsForPupilAsync(int accountId, DateTime from, DateTime to)
    {
        return await _db.GetCollection<Exam>()
            .FindAsync(e => e.Id.AccountId == accountId
                            && e.Deadline >= from
                            && e.Deadline <= to);
    }

    public async Task UpdateExamsForPupilAsync(int accountId, IEnumerable<Exam> entries)
    {
        await _db.GetCollection<Exam>().DeleteManyAsync(e => e.Id.AccountId == accountId);

        await _db.GetCollection<Exam>().UpsertAsync(entries);
    }

    async Task IHasAccountRemovalCleanup.DoPostRemovalCleanUpAsync(int accountId)
    {
        await _db.GetCollection<Exam>().DeleteManyAsync(e => e.Id.AccountId == accountId);
    }
}