using System;
using UIKit;

namespace Vulcanova.iOS
{
    public class DismissNotifyingUIController : UIViewController
    {
        public event EventHandler WillDisappear;
        public event EventHandler DidDisappear;

        public override void ViewWillDisappear(bool animated)
        {
            WillDisappear?.Invoke(this, EventArgs.Empty);

            base.ViewWillDisappear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            DidDisappear?.Invoke(this, EventArgs.Empty);
            
            base.ViewDidDisappear(animated);
        }
    }
}