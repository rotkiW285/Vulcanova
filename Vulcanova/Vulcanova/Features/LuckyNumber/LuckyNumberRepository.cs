using System;
using System.Threading.Tasks;
using LiteDB.Async;
using Vulcanova.Features.Auth;

namespace Vulcanova.Features.LuckyNumber;

public class LuckyNumberRepository : ILuckyNumberRepository, IHasAccountRemovalCleanup
{
    private readonly LiteDatabaseAsync _db;

    public LuckyNumberRepository(LiteDatabaseAsync db)
    {
        _db = db;
    }

    public async Task<LuckyNumber> FindForAccountAndConstituentAsync(int accountId, int constituentId,
        DateTime date)
    {
        return await _db.GetCollection<LuckyNumber>().FindOneAsync(l =>
            l.ConstituentId == constituentId 
            && l.AccountId == accountId 
            && date.Date == l.Date.Date);
    }

    public async Task AddAsync(LuckyNumber luckyNumber)
    {
        await _db.GetCollection<LuckyNumber>().InsertAsync(luckyNumber);
    }

    async Task IHasAccountRemovalCleanup.DoPostRemovalCleanUpAsync(int accountId)
    {
        await _db.GetCollection<LuckyNumber>().DeleteManyAsync(l => l.AccountId == accountId);
    }
}