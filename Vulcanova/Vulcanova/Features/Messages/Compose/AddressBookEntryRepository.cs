using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB.Async;

namespace Vulcanova.Features.Messages.Compose;

public class AddressBookEntryRepository : IAddressBookEntryRepository
{
    private readonly LiteDatabaseAsync _db;

    public AddressBookEntryRepository(LiteDatabaseAsync db)
    {
        _db = db;
    }

    public async Task<IEnumerable<AddressBookEntry>> GetBookEntriesForBoxAsync(Guid messageBoxId)
    {
        return await _db.GetCollection<AddressBookEntry>()
            .FindAsync(e => e.MessageBoxId == messageBoxId);
    }

    public async Task UpsertEntriesAsync(IEnumerable<AddressBookEntry> entries)
    {
        await _db.GetCollection<AddressBookEntry>().UpsertAsync(entries);
    }
}