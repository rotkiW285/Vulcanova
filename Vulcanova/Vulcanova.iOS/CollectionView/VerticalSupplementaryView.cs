using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace Vulcanova.iOS.CollectionView
{
	internal sealed class VerticalSupplementaryView : WidthConstrainedTemplatedCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.VerticalSupplementaryView");

		[Export("initWithFrame:")]
		[Xamarin.Forms.Internals.Preserve(Conditional = true)]
		public VerticalSupplementaryView(CGRect frame) : base(frame)
		{
		}

		public override CGSize Measure()
		{
			if (VisualElementRenderer?.Element == null)
			{
				return CGSize.Empty;
			}

			var measure = VisualElementRenderer.Element.Measure(ConstrainedDimension, 
				double.PositiveInfinity, MeasureFlags.IncludeMargins);

			var height = VisualElementRenderer.Element.Height > 0 
				? VisualElementRenderer.Element.Height : measure.Request.Height;

			return new CGSize(ConstrainedDimension, height);
		}

		protected override bool AttributesConsistentWithConstrainedDimension(UICollectionViewLayoutAttributes attributes)
		{
			return attributes.Frame.Width == ConstrainedDimension;
		}
	}
}