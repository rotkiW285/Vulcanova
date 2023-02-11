using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vulcanova.Features.Messages.Compose;

public interface IAddressBookEntryRepository
{
    Task<IEnumerable<AddressBookEntry>> GetBookEntriesForBoxAsync(Guid messageBoxId);
    Task UpsertEntriesAsync(IEnumerable<AddressBookEntry> entries);
}