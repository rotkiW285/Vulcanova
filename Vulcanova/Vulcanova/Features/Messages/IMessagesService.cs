using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vulcanova.Core.Data;
using Vulcanova.Features.Auth.Accounts;
using Vulcanova.Uonet.Api.MessageBox;

namespace Vulcanova.Features.Messages;

public interface IMessagesService
{
    IObservable<IEnumerable<Message>> GetMessagesByBox(
        Account account, Guid messageBoxId, MessageBoxFolder folder, bool forceSync = false);

    Task MarkMessageAsReadAsync(Guid messageBoxId, AccountEntityId<Guid> messageId);
}