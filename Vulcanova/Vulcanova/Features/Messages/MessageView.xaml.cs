using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Messages;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MessageView : IInitialize
{
    public static readonly BindableProperty MessageProperty = BindableProperty.Create(
        nameof(Message),
        typeof(Message),
        typeof(MessageView));

    public Message Message
    {  
        get => (Message) GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public MessageView()
    {
        InitializeComponent();
    }

    public void Initialize(INavigationParameters parameters)
    {
        Message = (Message) parameters[nameof(Message)];
    }
}