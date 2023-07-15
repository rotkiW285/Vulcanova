using System;
using System.Collections.Generic;
using Vulcanova.Features.Auth.Accounts;

namespace Vulcanova.Features.Messages;

public interface IMessageBoxesService
{
    IObservable<IEnumerable<MessageBox>> GetMessageBoxesByAccountId(Account account, bool forceSync = false);
}