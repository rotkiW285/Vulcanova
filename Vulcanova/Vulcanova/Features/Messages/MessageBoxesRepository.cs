using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB.Async;

namespace Vulcanova.Features.Messages;

public class MessageBoxesRepository : IMessageBoxesRepository
{
    private readonly LiteDatabaseAsync _db;

    public MessageBoxesRepository(LiteDatabaseAsync db)
    {
        _db = db;
    }

    public async Task<IEnumerable<MessageBox>> GetMessageBoxesForAccountAsync(int accountId)
    {
        return await _db.GetCollection<MessageBox>().FindAsync(b => b.AccountId == accountId);
    }

    public async Task<MessageBox> GetSelectedForAccountAsync(int accountId)
    {
        return await _db.GetCollection<MessageBox>()
            .FindOneAsync(b => b.AccountId == accountId && b.IsSelected);
    }

    public async Task UpdateMessageBoxesForAccountAsync(int accountId, IEnumerable<MessageBox> boxes)
    {
        await _db.GetCollection<MessageBox>().DeleteManyAsync(e => e.AccountId == accountId);

        await _db.GetCollection<MessageBox>().UpsertAsync(boxes);
    }

    public async Task UpdateMessageBoxAsync(MessageBox box)
    {
        await _db.GetCollection<MessageBox>().UpdateAsync(box);
    }
}