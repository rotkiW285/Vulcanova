using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB.Async;
using Vulcanova.Uonet.Api.MessageBox;

namespace Vulcanova.Features.Messages;

public class MessagesRepository : IMessagesRepository
{
    private readonly LiteDatabaseAsync _db;

    public MessagesRepository(LiteDatabaseAsync db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Message>> GetMessagesByBoxAsync(Guid messageBoxId, MessageBoxFolder folder)
    {
        return (await _db.GetCollection<Message>()
            .FindAsync(m => m.MessageBoxId == messageBoxId && m.Folder == folder))
            .OrderByDescending(x => x.DateSent);
    }

    public async Task UpsertMessagesForBoxAsync(Guid messageBoxId, IEnumerable<Message> messages)
    {
        await _db.GetCollection<Message>()
            .UpsertAsync(messages);
    }

    public async Task<Message> GetMessageAsync(Guid messageBoxId, Guid messageId)
    {
        return await _db.GetCollection<Message>()
            .FindOneAsync(m => m.MessageBoxId == messageBoxId && m.Id == messageId);
    }

    public async Task UpdateMessageAsync(Message message)
    {
        await _db.GetCollection<Message>().UpdateAsync(message);
    }

    public async Task DeleteMessagesInBoxAsync(Guid messageBoxId)
    {
        await _db.GetCollection<Message>().DeleteManyAsync(m => m.MessageBoxId == messageBoxId);
    }
}