using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace Vulcanova.iOS.CollectionView
{
	internal sealed class VerticalCell : WidthConstrainedTemplatedCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.VerticalCell");

		[Export("initWithFrame:")]
		[Xamarin.Forms.Internals.Preserve(Conditional = true)]
		public VerticalCell(CGRect frame) : base(frame)
		{
		}

		public override CGSize Measure()
		{
			var measure = VisualElementRenderer.Element.Measure(ConstrainedDimension,
				double.PositiveInfinity, MeasureFlags.IncludeMargins);

			return new CGSize(ConstrainedDimension, measure.Request.Height);
		}

		protected override bool AttributesConsistentWithConstrainedDimension(UICollectionViewLayoutAttributes attributes)
		{
			return attributes.Frame.Width == ConstrainedDimension;
		}
	}
}