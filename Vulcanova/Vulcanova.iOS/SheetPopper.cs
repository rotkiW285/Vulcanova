using System;
using System.Collections.Generic;
using UIKit;
using Vulcanova.Core.Layout;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Vulcanova.iOS
{
    public class SheetPopper : ISheetPopper
    {
        private readonly Dictionary<ContentView, Action> _sheets = new Dictionary<ContentView, Action>();

        public void PushSheet(ContentView content, bool hasCloseButton = true, bool useSafeArea = false)
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

            var closeAction = new Action(() => rootController.DismissViewController(true, null));
            
            if (hasCloseButton)
            {
                rootController = new UINavigationController(cvc);
                
                cvc.NavigationItem.SetRightBarButtonItem(new UIBarButtonItem(UIBarButtonSystemItem.Close,
                        (sender, args) => closeAction()),
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

            _sheets[content] = closeAction;
        }

        public void PopSheet(ContentView content)
        {
            _sheets[content]();

            _sheets.Remove(content);
        }
    }
}