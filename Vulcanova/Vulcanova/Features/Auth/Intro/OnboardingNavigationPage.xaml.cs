using Rg.Plugins.Popup.Contracts;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Auth.Intro;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class OnboardingNavigationPage
{
    public OnboardingNavigationPage(IPopupNavigation navigationService) : base(navigationService)
    {
        InitializeComponent();
    }
}