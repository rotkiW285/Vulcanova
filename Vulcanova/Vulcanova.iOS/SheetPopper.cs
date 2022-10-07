using System;
using System.Collections.Generic;
using UIKit;
using Vulcanova.Core.Layout;
using Vulcanova.Core.Layout.PlatformSpecific.iOS;
using Xamarin.Forms;
using Page = Xamarin.Forms.Page;

namespace Vulcanova.iOS
{
    public class SheetPopper : ISheetPopper
    {
        public Dictionary<Page, Action> Sheets { get; } = new Dictionary<Page, Action>();

        public void PushSheet(Page page)
        {
            Device.InvokeOnMainThreadAsync(() =>
            {
                page.Parent = Xamarin.Forms.Application.Current;
                page.NavigationProxy.Inner = Xamarin.Forms.Application.Current.NavigationProxy;
                
                var cvc = page.CreateViewController();

                var rootController = cvc;

                var closeAction = new Action(() => rootController.DismissViewController(true, null));
            
                if ((bool) page.GetValue(Sheet.HasCloseButtonProperty))
                {
                    rootController = new UINavigationController(cvc);
                
                    cvc.NavigationItem.SetRightBarButtonItem(new UIBarButtonItem(UIBarButtonSystemItem.Close,
                            (sender, args) => closeAction()),
                        true);
                }

                var detents = new List<UISheetPresentationControllerDetent>();

                if ((bool) page.GetValue(Sheet.MediumDetentProperty))
                {
                    detents.Add(UISheetPresentationControllerDetent.CreateMediumDetent());
                }

                if ((bool) page.GetValue(Sheet.LargeDetentProperty))
                {
                    detents.Add(UISheetPresentationControllerDetent.CreateLargeDetent());
                }

                rootController.SheetPresentationController.Detents = detents.ToArray();

                rootController.SheetPresentationController.PrefersGrabberVisible =
                    (bool) page.GetValue(Sheet.PrefersGrabberVisibleProperty);

                rootController.SheetPresentationController.PrefersScrollingExpandsWhenScrolledToEdge =
                    (bool) page.GetValue(Sheet.PrefersScrollingExpandsWhenScrolledToEdgeProperty);

                UIApplication.SharedApplication.KeyWindow?.RootViewController?
                    .PresentViewController(rootController, true, null);
                
                Sheets[page] = closeAction;
            });
        }

        public void PopSheet(Page page)
        {
            Sheets[page]();

            Sheets.Remove(page);
        }
    }
}