using System;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Vulcanova.iOS.CollectionView
{
	public abstract class DefaultCell : ItemsViewCell
	{
		public UILabel Label { get; }

		protected NSLayoutConstraint Constraint { get; set; }

		[Export("initWithFrame:")]
		[Xamarin.Forms.Internals.Preserve(Conditional = true)]
		protected DefaultCell(CGRect frame) : base(frame)
		{
			Label = new UILabel(frame)
			{
				TextColor = UIColor.LabelColor,
				Lines = 1,
				Font = UIFont.PreferredBody,
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			ContentView.BackgroundColor = UIColor.Clear;

			InitializeContentConstraints(Label);
		}

		public override void ConstrainTo(nfloat constant)
		{
			Constraint.Constant = constant;
		}
	}
}