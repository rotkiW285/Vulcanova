using System;
using System.Collections.Generic;

namespace Vulcanova.Features.Messages.Compose;

public interface IAddressBookProvider
{
    IObservable<IEnumerable<AddressBookEntry>> GetAddressBookEntriesForBox(int accountId, Guid messageBoxId,
        bool forceSync = false);
}