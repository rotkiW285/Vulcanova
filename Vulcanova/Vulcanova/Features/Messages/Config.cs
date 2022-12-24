using Prism.Ioc;

namespace Vulcanova.Features.Messages;

public static class Config
{
    public static void RegisterMessages(this IContainerRegistry container)
    {
        container.RegisterForNavigation<MessagesView>();
        container.RegisterForNavigation<MessageView>();

        container.RegisterScoped<IMessagesService, MessagesService>();
        container.RegisterScoped<IMessagesRepository, MessagesRepository>();
        
        container.RegisterScoped<IMessageBoxesService, MessageBoxesService>();
        container.RegisterScoped<IMessageBoxesRepository, MessageBoxesRepository>();
    }
}