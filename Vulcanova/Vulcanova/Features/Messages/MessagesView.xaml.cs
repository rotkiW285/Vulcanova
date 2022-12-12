using System.Linq;
using System.Reactive.Disposables;
using ReactiveUI;
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
            this.Bind(ViewModel, vm => vm.SelectedFolderIndex, v => v.TabHost.SelectedIndex)
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.Messages, v => v.MessagesList.ItemsSource,
                    msg => msg.OrderByDescending(x => x.DateSent))
                .DisposeWith(disposable);
        });
    }
}