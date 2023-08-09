using System.Threading.Tasks;
using Vulcanova.Features.Auth;

namespace Vulcanova.Features.Messages;

public sealed class AccountRemovalMessagesCleanUpService : IHasAccountRemovalCleanup
{
    private readonly IMessageBoxesRepository _messageBoxesRepository;
    private readonly IMessagesRepository _messagesRepository;

    public AccountRemovalMessagesCleanUpService(IMessageBoxesRepository messageBoxesRepository, IMessagesRepository messagesRepository)
    {
        _messageBoxesRepository = messageBoxesRepository;
        _messagesRepository = messagesRepository;
    }

    public async Task DoPostRemovalCleanUpAsync(int accountId)
    {
        var messageBoxes = await _messageBoxesRepository.GetMessageBoxesForAccountAsync(accountId);

        foreach (var box in messageBoxes)
        {
            await _messagesRepository.DeleteMessagesInBoxAsync(box.GlobalKey);
        }

        await _messageBoxesRepository.DeleteMessageBoxesForAccountAsync(accountId);
    }
}