using Prism.Ioc;
using Vulcanova.Features.Messages.Compose;

namespace Vulcanova.Features.Messages;

public static class Config
{
    public static void RegisterMessages(this IContainerRegistry container)
    {
        container.RegisterForNavigation<MessagesView>();
        container.RegisterForNavigation<MessageView>();
        
        container.RegisterForNavigation<ComposeMessageView>();

        container.RegisterScoped<IMessagesService, MessagesService>();
        container.RegisterScoped<IMessagesRepository, MessagesRepository>();
        
        container.RegisterScoped<IMessageBoxesService, MessageBoxesService>();
        container.RegisterScoped<IMessageBoxesRepository, MessageBoxesRepository>();

        container.RegisterScoped<IAddressBookEntryRepository, AddressBookEntryRepository>();
        container.RegisterScoped<IAddressBookProvider, AddressBookProvider>();

        container.RegisterScoped<IMessageSender, MessageSender>();
    }
}