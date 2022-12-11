using System;
using System.Collections.Generic;

namespace Vulcanova.Features.Messages;

public interface IMessageBoxesService
{
    IObservable<IEnumerable<MessageBox>> GetMessageBoxesByAccountId(int accountId, bool forceSync = false);
}