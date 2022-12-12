using System.Linq;
using System.Reactive.Disposables;
using ReactiveUI;
using Vulcanova.Uonet.Api.MessageBox;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Messages;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MessagesView
{
    public MessagesView()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.Bind(ViewModel, vm => vm.SelectedFolder, v => v.TabHost.SelectedIndex,
                    viewToVmConverter: i => (MessageBoxFolder)(i + 1),
                    vmToViewConverter: folder => (int)(folder - 1))
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.Messages, v => v.MessagesList.ItemsSource,
                    msg => msg.OrderByDescending(x => x.DateSent))
                .DisposeWith(disposable);
        });
    }
}