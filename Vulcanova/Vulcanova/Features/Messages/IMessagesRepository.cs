using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vulcanova.Uonet.Api.MessageBox;

namespace Vulcanova.Features.Messages;

public interface IMessagesRepository
{
    Task<IEnumerable<Message>> GetMessagesByBoxAsync(Guid messageBoxId, MessageBoxFolder folder);

    Task UpsertMessagesForBoxAsync(Guid messageBoxId, IEnumerable<Message> messages);

    Task<Message> GetMessageAsync(Guid messageBoxId, Guid messageId);

    Task UpdateMessageAsync(Message message);

    Task DeleteMessagesInBoxAsync(Guid messageBoxId);
}