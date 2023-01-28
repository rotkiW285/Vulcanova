using UIKit;
using Vulcanova.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ResolutionGroupName ("Vulcanova")]
[assembly:ExportEffect (typeof(BorderlessEntryEffect), nameof(BorderlessEntryEffect))]
namespace Vulcanova.iOS
{
    public class BorderlessEntryEffect : PlatformEffect
    {
        private UITextBorderStyle _borderStyle;

        protected override void OnAttached()
        {
            var textField = (UITextField) Control;

            _borderStyle = textField.BorderStyle;
            textField.BorderStyle = UITextBorderStyle.None;
        }

        protected override void OnDetached()
        {
            ((UITextField) Control).BorderStyle = _borderStyle;
        }
    }
}