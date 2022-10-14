using Android.Content;
using Vulcanova.Android;
using Vulcanova.Core.Layout;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly:ExportRenderer(typeof(SheetPage), typeof(SheetPageRenderer))]
namespace Vulcanova.Android
{
    public class SheetPageRenderer : PageRenderer
    {
        public SheetPageRenderer(Context context) : base(context)
        {
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            Element.Layout(new Rectangle(Context.FromPixels(l), Context.FromPixels(t), Context.FromPixels(r), Context.FromPixels(b)));
            
            base.OnLayout(changed, l, t, r, b);
        }
    }
}