using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Prism.Navigation;
using ReactiveUI;
using Vulcanova.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Messages;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MessageView : IInitializeAsync
{
    private readonly ToolbarItem _replyItem;

    public MessageView()
    {
        InitializeComponent();

        _replyItem = new ToolbarItem
        {
            Text = Strings.MessageReplyButton
        };

        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, vm => vm.Reply, v => v._replyItem.Command)
                .DisposeWith(disposable);

            ViewModel.Reply.CanExecute
                .Subscribe(ConfigureToolbarItems)
                .DisposeWith(disposable);
        });
    }

    // Set ToolbarItems visibility as soon as the page is initialized: the WhenActivated action declared above
    // will be called a little bit later than InitializeAsync and we want to avoid having the toolbar item appear
    // with a delay. The command is still bound in WhenActivated.
    public Task InitializeAsync(INavigationParameters parameters)
    {
        ConfigureToolbarItems(ViewModel!.CanReply);
        return Task.CompletedTask;
    }

    private void ConfigureToolbarItems(bool canReply)
    {
        if (canReply && !ToolbarItems.Contains(_replyItem))
        {
            ToolbarItems.Add(_replyItem);
        }
        else if (!canReply && ToolbarItems.Contains(_replyItem))
        {
            ToolbarItems.Remove(_replyItem);
        }
    }
}