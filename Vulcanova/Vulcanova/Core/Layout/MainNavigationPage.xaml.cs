using System.Threading.Tasks;
using Rg.Plugins.Popup.Contracts;
using Vulcanova.Core.Layout.Controls.ErrorPopup;
using Vulcanova.Core.Rx;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Core.Layout
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainNavigationPage
    {
        public MainNavigationPage(IPopupNavigation navigationService)
        {
            InitializeComponent();

            Interactions.Errors.RegisterHandler(async ctx =>
            {
                var popup = new ErrorPopup(ctx.Input, ViewModel.ShowErrorDetails);

                await navigationService.PushAsync(popup);

                await Task.Delay(5000);

                await navigationService.RemovePageAsync(popup);
            });
        }
    }
}