using System.Linq;
using Prism.Navigation;
using Vulcanova.Resources;
using Vulcanova.Uonet.Api.MessageBox;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Messages;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MessageView : IInitialize
{
    public static readonly BindableProperty MessageProperty = BindableProperty.Create(
        nameof(Message),
        typeof(Message),
        typeof(MessageView),
        propertyChanged: MessageChanged);

    public Message Message
    {
        get => (Message) GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public MessageView()
    {
        InitializeComponent();
    }

    private static void MessageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (MessageView) bindable;
        var message = (Message) newValue;

        if (message.Folder == MessageBoxFolder.Received)
        {
            view.ReadByLabel.Text = string.Empty;
            return;
        }

        view.ReadByLabel.Text = string.Format(Strings.MessageReadByLabel,
            message.Receiver.Count(x => x.HasRead == 1),
            message.Receiver.Count
        );
    }

    public void Initialize(INavigationParameters parameters)
    {
        Message = (Message) parameters[nameof(Message)];
    }
}