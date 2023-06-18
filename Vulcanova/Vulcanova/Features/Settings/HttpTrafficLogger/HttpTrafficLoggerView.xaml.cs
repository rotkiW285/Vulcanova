using System.Reactive.Disposables;
using ReactiveUI;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Settings.HttpTrafficLogger;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class HttpTrafficLoggerView
{
    public HttpTrafficLoggerView()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, vm => vm.LogEntries, v => v.TrafficView.ItemsSource)
                .DisposeWith(disposable);
        });
    }
}