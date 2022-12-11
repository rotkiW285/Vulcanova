using System;
using System.Collections.Generic;
using Vulcanova.Uonet.Api.MessageBox;

namespace Vulcanova.Features.Messages;

public interface IMessagesService
{
    IObservable<IEnumerable<Message>> GetMessagesByBox(
        int accountId, Guid messageBoxId, MessageBoxFolder folder, bool forceSync = false);
}