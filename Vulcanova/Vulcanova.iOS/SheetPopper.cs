using UIKit;
using Vulcanova.Core.Layout;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Vulcanova.iOS
{
    public class SheetPopper : ISheetPopper
    {
        public void PopSheet(ContentView content, bool hasCloseButton = true, bool useSafeArea = false)
        {
            // Maybe let the caller decide about this?
            content.Padding = new Thickness(0, 14, 0, 0);

            var page = new ContentPage
            {
                Content = content,
                Parent = Xamarin.Forms.Application.Current
            };

            if (useSafeArea)
            {
                page.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            }

            var cvc = page.CreateViewController();

            var rootController = cvc;
            
            if (hasCloseButton)
            {
                rootController = new UINavigationController(cvc);
                
                cvc.NavigationItem.SetRightBarButtonItem(new UIBarButtonItem(UIBarButtonSystemItem.Close,
                        (sender, args) => rootController.DismissViewController(true, null)),
                    true);
            }

            rootController.SheetPresentationController.Detents =
                    new[]
                    {
                        UISheetPresentationControllerDetent.CreateMediumDetent(),
                        UISheetPresentationControllerDetent.CreateLargeDetent()
                    };

            rootController.SheetPresentationController.PrefersGrabberVisible = true;
            rootController.SheetPresentationController.PrefersScrollingExpandsWhenScrolledToEdge = true;

            UIApplication.SharedApplication.KeyWindow?.RootViewController?
                .PresentViewController(rootController, true, null);
        }
    }
}