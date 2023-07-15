using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vulcanova.Features.Auth.Accounts;
using Vulcanova.Uonet.Api.MessageBox;

namespace Vulcanova.Features.Messages;

public interface IMessagesService
{
    IObservable<IEnumerable<Message>> GetMessagesByBox(
        Account account, Guid messageBoxId, MessageBoxFolder folder, bool forceSync = false);

    Task MarkMessageAsReadAsync(int accountId, Guid messageBoxId, Guid messageId);
}