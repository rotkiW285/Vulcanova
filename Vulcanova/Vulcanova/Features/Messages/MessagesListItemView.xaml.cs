using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Messages;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MessagesListItemView
{
    public static readonly BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(Message), typeof(MessagesListItemView));

    public Message Message
    {
        get => (Message) GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public MessagesListItemView()
    {
        InitializeComponent();
    }
}