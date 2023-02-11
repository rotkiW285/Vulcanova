using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vulcanova.Features.Messages;

public interface IMessageBoxesRepository
{
    Task<IEnumerable<MessageBox>> GetMessageBoxesForAccountAsync(int accountId);

    Task UpdateMessageBoxesForAccountAsync(int accountId, IEnumerable<MessageBox> boxes);

    Task UpdateMessageBoxAsync(MessageBox box);
}