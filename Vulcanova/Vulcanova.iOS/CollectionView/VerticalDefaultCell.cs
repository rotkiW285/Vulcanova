using CoreGraphics;
using Foundation;
using UIKit;

namespace Vulcanova.iOS.CollectionView
{
	internal sealed class VerticalDefaultCell : DefaultCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.VerticalDefaultCell");

		[Export("initWithFrame:")]
		[Xamarin.Forms.Internals.Preserve(Conditional = true)]
		public VerticalDefaultCell(CGRect frame) : base(frame)
		{
			Constraint = Label.WidthAnchor.ConstraintEqualTo(Frame.Width);
			Constraint.Priority = (float)UILayoutPriority.DefaultHigh;
			Constraint.Active = true;
		}

		public override void ConstrainTo(CGSize constraint)
		{
			Constraint.Constant = constraint.Width;
		}

		public override CGSize Measure()
		{
			return new CGSize(Constraint.Constant, Label.IntrinsicContentSize.Height);
		}
	}
}