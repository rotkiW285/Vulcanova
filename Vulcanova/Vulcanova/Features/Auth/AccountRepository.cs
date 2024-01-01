using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB.Async;
using Vulcanova.Features.Auth.Accounts;
using Xamarin.Essentials;

namespace Vulcanova.Features.Auth;

public class AccountRepository : IAccountRepository
{
    private readonly LiteDatabaseAsync _db;

    public AccountRepository(LiteDatabaseAsync db)
    {
        _db = db;
    }

    public async Task AddAccountsAsync(IEnumerable<Account> accounts)
    {
        await _db.GetCollection<Account>().InsertBulkAsync(accounts);
    }

    public async Task<Account> GetActiveAccountAsync()
    {
        return await _db.GetCollection<Account>()
            .FindOneAsync(a => a.IsActive).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<Account>> GetAccountsAsync()
    {
        var items = (await _db.GetCollection<Account>().FindAllAsync()).ToArray();
        return Array.AsReadOnly(items);
    }

    public async Task<Account> GetByIdAsync(int id)
    {
        return await _db.GetCollection<Account>()
            .FindByIdAsync(id);
    }

    public async Task UpdateAccountAsync(Account account)
    {
        await _db.GetCollection<Account>().UpdateAsync(account);
    }

    public async Task UpdateAccountsAsync(IEnumerable<Account> accounts)
    {
        await _db.GetCollection<Account>().UpdateAsync(accounts);
    }

    public async Task DeleteByIdAsync(int id)
    {
        await _db.GetCollection<Account>().DeleteAsync(id);
    }

    private static readonly SemaphoreSlim IdGenSemaphore = new (1, 1);

    public async Task<int> GetNextAccountIdAsync()
    {
        await IdGenSemaphore.WaitAsync();

        const string lastAccountIdPreferencesKey = "LastAccountId";

        var lastId = Preferences.Get(lastAccountIdPreferencesKey, -1);
        if (lastId == -1)
        {
            try
            {
                lastId = await _db.GetCollection<Account>().MaxAsync(x => x.Id);
            }
            catch (LiteAsyncException e) when (e.InnerException is InvalidOperationException
                                               {
                                                   Message: "Sequence contains no elements"
                                               })
            {
                lastId = 0;
            }
        }

        var nextId = lastId + 1;
        Preferences.Set(lastAccountIdPreferencesKey, nextId);

        IdGenSemaphore.Release();

        return nextId;
    }
}