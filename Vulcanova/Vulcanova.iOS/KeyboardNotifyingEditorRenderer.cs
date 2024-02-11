using Foundation;
using ObjCRuntime;
using Vulcanova.Core.Layout.Controls;
using Vulcanova.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(KeyboardNotifyingEditor), typeof(KeyboardNotifyingEditorRenderer))]
namespace Vulcanova.iOS
{
    public class KeyboardNotifyingEditorRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                NSNotificationCenter.DefaultCenter.AddObserver(this, new Selector("KeyboardWillShow:"),
                    new NSString("UIKeyboardWillShowNotification"), null);
                NSNotificationCenter.DefaultCenter.AddObserver(this, new Selector("KeyboardDidShow:"),
                    new NSString("UIKeyboardDidShowNotification"), null);
            }
        }

        [Export("KeyboardWillShow:")]
        private void KeyboardWillShow(NSNotification note)
        {
            ((KeyboardNotifyingEditor)Element).OnKeyboardWillShow();
        }

        [Export("KeyboardDidShow:")]
        private void KeyboardDidShow(NSNotification note)
        {
            ((KeyboardNotifyingEditor)Element).OnKeyboardDidShow();
        }
    }
}