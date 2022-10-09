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
        public event EventHandler<SheetEventArgs> SheetWillDisappear;
        public event EventHandler<SheetEventArgs> SheetDisappeared;

        public Page DisplayedSheet { get; private set; }

        private Action _popSheetAction;

        public void PushSheet(Page page)
        {
            Device.InvokeOnMainThreadAsync(() =>
            {
                page.Parent = Xamarin.Forms.Application.Current;
                page.NavigationProxy.Inner = Xamarin.Forms.Application.Current.NavigationProxy;
                
                var cvc = page.CreateViewController();

                var wrapper = new DismissNotifyingUIController();
                wrapper.AddChildViewController(cvc);
                cvc.DidMoveToParentViewController(wrapper);
                wrapper.View.AddSubview(cvc.View);

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
                    SheetWillDisappear?.Invoke(s, new SheetEventArgs(DisplayedSheet));
                };

                wrapper.DidDisappear += (s, e) =>
                {
                    SheetDisappeared?.Invoke(s, new SheetEventArgs(DisplayedSheet));

                    DisplayedSheet = null;
                };

                UIApplication.SharedApplication.KeyWindow?.RootViewController?
                    .PresentViewController(rootController, true, null);

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
            DisplayedSheet = null;
        }
    }
}