using System.Reactive.Disposables;
using ReactiveUI;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Settings.HttpTrafficLogger.Details;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class HttpTrafficLoggerEntryDetailsView
{
    public HttpTrafficLoggerEntryDetailsView()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.Bind(ViewModel, vm => vm.SelectedTabIndex, v => v.Switcher.SelectedIndex)
                .DisposeWith(disposable);
            
            this.Bind(ViewModel, vm => vm.SelectedTabIndex, v => v.TabHost.SelectedIndex)
                .DisposeWith(disposable);
        });
    }
}