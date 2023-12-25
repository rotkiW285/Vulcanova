using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB.Async;
using Vulcanova.Features.Auth;

namespace Vulcanova.Features.Homework;

public class HomeworkRepository : IHomeworkRepository, IHasAccountRemovalCleanup
{
    private readonly LiteDatabaseAsync _db;
        
    public HomeworkRepository(LiteDatabaseAsync db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Homework>> GetHomeworkForPupilAsync(int accountId, int pupilId)
    {
        return await _db.GetCollection<Homework>()
            .FindAsync(h => h.PupilId == pupilId && h.Id.AccountId == accountId);
    }

    public async Task UpdateHomeworkEntriesAsync(IEnumerable<Homework> entries, int accountId)
    {
        await _db.GetCollection<Homework>()
            .DeleteManyAsync(h => h.Id.AccountId == accountId);
            
        await _db.GetCollection<Homework>().UpsertAsync(entries);
    }

    async Task IHasAccountRemovalCleanup.DoPostRemovalCleanUpAsync(int accountId)
    {
        await _db.GetCollection<Homework>().DeleteManyAsync(h => h.Id.AccountId == accountId);
    }
}