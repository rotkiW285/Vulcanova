using Rg.Plugins.Popup.Contracts;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Core.Layout;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MainNavigationPage
{
    public MainNavigationPage(IPopupNavigation navigationService) : base(navigationService)
    {
        InitializeComponent();
    }
}