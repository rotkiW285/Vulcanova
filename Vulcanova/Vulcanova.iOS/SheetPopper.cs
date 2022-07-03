using UIKit;
using Vulcanova.Core.Layout;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Vulcanova.iOS
{
    public class SheetPopper : ISheetPopper
    {
        public void PopSheet(ContentView content)
        {
            // Maybe let the caller decide about this?
            content.Padding = new Thickness(0, 14, 0, 0);

            var page = new ContentPage
            {
                Content = content,
                Parent = Xamarin.Forms.Application.Current
            };

            var cvc = page.CreateViewController();

            cvc.SheetPresentationController.Detents =
                new[]
                {
                    UISheetPresentationControllerDetent.CreateMediumDetent(),
                    UISheetPresentationControllerDetent.CreateLargeDetent()
                };

            cvc.SheetPresentationController.PrefersGrabberVisible = true;
            cvc.SheetPresentationController.PrefersScrollingExpandsWhenScrolledToEdge = true;

            UIApplication.SharedApplication.KeyWindow?.RootViewController?
                .PresentViewController(cvc, true, null);
        }
    }
}