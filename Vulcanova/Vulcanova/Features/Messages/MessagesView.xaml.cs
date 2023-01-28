using System.Linq;
using System.Reactive.Disposables;
using ReactiveUI;
using Vulcanova.Core.Layout;
using Vulcanova.Core.Rx;
using Vulcanova.Uonet.Api.MessageBox;
using Xamarin.Forms;
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

            this.BindForceRefresh(RefreshView, v => v.ViewModel.LoadMessages)
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.ShowMessageComposer, v => v.NewMessageButton.Command)
                .DisposeWith(disposable);

            // workaround https://github.com/xamarin/Xamarin.Forms/issues/1336 as this breaks pushing modals
            if (Application.Current.MainPage is NavigationPage {CurrentPage: HomeTabbedPage {CurrentPage: null} page})
            {
                page.CurrentPage = this;
            }
        });
    }
}