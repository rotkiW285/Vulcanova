using System;
using System.Collections.Generic;
using UIKit;
using Vulcanova.Core.Layout;
using Vulcanova.Core.Layout.PlatformSpecific.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Page = Xamarin.Forms.Page;

namespace Vulcanova.iOS
{
    public class SheetPopper : ISheetPopper
    {
        public event EventHandler<SheetEventArgs> SheetWillDisappear;
        public event EventHandler<SheetEventArgs> SheetDisappeared;

        public Page DisplayedSheet { get; private set; }

        private Action _popSheetAction;

        public void PushSheet(Page page)
        {
            Device.InvokeOnMainThreadAsync(() =>
            {
                var rootNavigationProxy = Xamarin.Forms.Application.Current.NavigationProxy;

                page.NavigationProxy.Inner = rootNavigationProxy;

                var renderer = Platform.GetRenderer(page);
                if (renderer == null)
                {
                    renderer = Platform.CreateRenderer(page);
                    Platform.SetRenderer(page, renderer);
                }

                var cvc = renderer.ViewController;

                var wrapper = new DismissNotifyingUIController(renderer);

                cvc = wrapper;

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

                wrapper.WillDisappear += (s, e) =>
                {
                    if (DisplayedSheet != null)
                    {
                        SheetWillDisappear?.Invoke(s, new SheetEventArgs(DisplayedSheet));
                    }
                };

                wrapper.DidDisappear += (s, e) =>
                {
                    if (DisplayedSheet != null)
                    {
                        SheetDisappeared?.Invoke(s, new SheetEventArgs(DisplayedSheet));
                    }

                    DisplayedSheet = null;

                    (rootNavigationProxy.ModalStack as List<Page>)?.Remove(page);
                };

                UIApplication.SharedApplication.KeyWindow?.RootViewController?
                    .PresentViewController(rootController, true, null);

                // On iOS each UIViewController can present at most one ViewController at a time.
                // That's why, when doing modal navigation, Xamarin presents the modal page from the current topmost page.
                // We need to add the displayed sheet to the ModalStack, so Xamarin knows to present the next modal
                // from the sheet's view controller.
                (rootNavigationProxy.ModalStack as List<Page>)?.Add(page);

                DisplayedSheet = page;
                _popSheetAction = closeAction;
            });
        }

        public void PopSheet()
        {
            if (DisplayedSheet == null)
            {
                throw new NullReferenceException("No sheet is being displayed");
            }

            _popSheetAction();
        }
    }
}