using System;
using System.Collections.Generic;
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
        return await _db.GetCollection<Message>()
            .FindAsync(m => m.MessageBoxId == messageBoxId && m.Folder == folder);
    }

    public async Task UpsertMessagesForBoxAsync(Guid messageBoxId, IEnumerable<Message> messages)
    {
        await _db.GetCollection<Message>()
            .UpsertAsync(messages);
    }
}