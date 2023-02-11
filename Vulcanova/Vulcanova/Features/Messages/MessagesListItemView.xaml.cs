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
    }
    
    
    private static void MessageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (MessagesListItemView) bindable;

        view.ClippyBoi.IsVisible = ((Message) newValue).Attachments.Count > 0;
    }
}