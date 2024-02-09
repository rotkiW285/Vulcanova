using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Vulcanova.iOS
{
    public class DismissNotifyingUIController : UIViewController
    {
        public event EventHandler WillDisappear;
        public event EventHandler DidDisappear;

        private readonly IVisualElementRenderer _content;

        public DismissNotifyingUIController(IVisualElementRenderer content)
        {
            _content = content;

            View.AddSubview(content.ViewController.View);
            TransitioningDelegate = content.ViewController.TransitioningDelegate;
            AddChildViewController(content.ViewController);

            content.ViewController.DidMoveToParentViewController(this);
        }

        public override void ViewWillDisappear(bool animated)
        {
            // this should probably be moved to the topmost sheet UIViewController
            if (NavigationController?.IsBeingDismissed == true || IsBeingDismissed)
                WillDisappear?.Invoke(this, EventArgs.Empty);

            base.ViewWillDisappear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            if (NavigationController?.IsBeingDismissed == true || IsBeingDismissed)
                DidDisappear?.Invoke(this, EventArgs.Empty);
            
            base.ViewDidDisappear(animated);
        }
        
        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            _content?.SetElementSize(new Size(View.Bounds.Width, View.Bounds.Height));
        }
    }
}