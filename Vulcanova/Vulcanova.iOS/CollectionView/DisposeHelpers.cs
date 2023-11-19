using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Vulcanova.iOS.CollectionView
{
    public static class DisposeHelpers
    {
        private static readonly BindableProperty RendererProperty = (BindableProperty) typeof(Platform)
            .GetProperty("RendererProperty", BindingFlags.Static)!
            .GetValue(null);

        internal static void DisposeRendererAndChildren(this IVisualElementRenderer rendererToRemove)
        {
            if (rendererToRemove == null)
                return;

            if (rendererToRemove.Element != null && Platform.GetRenderer(rendererToRemove.Element) == rendererToRemove)
                rendererToRemove.Element.ClearValue(RendererProperty);

            if (rendererToRemove.NativeView != null)
            {
                var subviews = rendererToRemove.NativeView.Subviews;
                for (var i = 0; i < subviews.Length; i++)
                {
                    if (subviews[i] is IVisualElementRenderer childRenderer)
                        DisposeRendererAndChildren(childRenderer);
                }

                rendererToRemove.NativeView.RemoveFromSuperview();
            }

            rendererToRemove.Dispose();
        }
    }
}