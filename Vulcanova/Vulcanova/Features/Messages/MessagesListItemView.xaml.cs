using System;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Messages;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MessagesListItemView
{
    public static readonly BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(Message), typeof(MessagesListItemView),
            propertyChanged: MessageChanged);

    public Message Message
    {
        get => (Message) GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public MessagesListItemView()
    {
        InitializeComponent();

        MessageBus.Current.Listen<MessageReadEvent>()
            .Subscribe(@event =>
            {
                if (@event.MessageBoxId != Message.MessageBoxId || @event.MessageId != Message.Id.VulcanId) return;

                Message.DateRead = @event.DateRead;
                UpdateMessageReadIndicator();
            });
    }

    private static void MessageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (MessagesListItemView) bindable;

        view.ClippyBoi.IsVisible = ((Message) newValue)?.Attachments.Count > 0;
        view.UpdateMessageReadIndicator();
    }

    private void UpdateMessageReadIndicator()
    {
        MessageSubjectLabel.FontAttributes = Message.DateRead != null
            ? FontAttributes.None
            : FontAttributes.Bold;
    }
}