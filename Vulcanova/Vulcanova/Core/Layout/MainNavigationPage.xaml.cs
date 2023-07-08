using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;
using FFImageLoading.Svg.Forms;
using ReactiveUI;
using Rg.Plugins.Popup.Contracts;
using Vulcanova.Core.Layout.Controls;
using Vulcanova.Core.Rx;
using Vulcanova.Resources;
using Vulcanova.Uonet.Api;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Core.Layout;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MainNavigationPage
{
    private readonly Subject<Exception> _exceptions = new();

    public MainNavigationPage(IPopupNavigation navigationService)
    {
        InitializeComponent();

        Interactions.Errors.RegisterHandler(async ctx =>
        {
            await Task.Yield();
            _exceptions.OnNext(ctx.Input);
        });

        var closes = _exceptions.Throttle(TimeSpan.FromSeconds(1));

        closes
            .Window(() => closes)
            .SelectMany(w => w.ToList())
            .ObserveOn(RxApp.MainThreadScheduler)
            .SelectMany(async exceptions =>
            {
                static bool IsSessionError(Exception e) => e is VulcanException { StatusCode: 108 };

                var sessionError = exceptions.FirstOrDefault(IsSessionError);
                if (sessionError != null)
                {
                    await HandleException(sessionError);
                }

                foreach (var exception in exceptions.Where(e => !IsSessionError(e)))
                {
                    await HandleException(exception);
                }

                return Unit.Default;
            })
            .Subscribe();
        return;

        async Task HandleException(Exception exception)
        {
            var (title, message, icon, action, command, parameter) = exception switch
            {
                VulcanException { StatusCode: 108 } => (
                    Strings.SessionErrorPopupTitle,
                    Strings.SessionErrorAlertMessage,
                    SvgImageSource.FromResource("Vulcanova.Resources.Icons.alert-circle-outline.svg"),
                    Strings.SessionErrorAlertAction,
                    ViewModel.ShowSignInAlert,
                    Unit.Default),

                MaintenanceBreakException _ => (
                    Strings.MaintenanceBreakErrorPopupTitle,
                    Strings.MaintenanceBreakErrorPopupMessage,
                    SvgImageSource.FromResource("Vulcanova.Resources.Icons.alert-circle-outline.svg"),
                    Strings.ShowErrorDetails,
                    ViewModel.ShowErrorDetails,
                    exception),

                _ => (
                    Strings.ErrorAlertTitle,
                    exception.Message,
                    SvgImageSource.FromResource("Vulcanova.Resources.Icons.close-circle.svg"),
                    Strings.ShowErrorDetails,
                    (ICommand)ViewModel.ShowErrorDetails,
                    (object)exception)
            };

            var popup = new SnackPopup(title, message, icon, action, command, parameter);

            await navigationService.PushAsync(popup);

            await Task.Delay(5000);

            await navigationService.RemovePageAsync(popup);
        }
    }
}