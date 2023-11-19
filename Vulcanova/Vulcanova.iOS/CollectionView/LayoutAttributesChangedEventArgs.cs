using System;
using UIKit;

namespace Vulcanova.iOS.CollectionView
{
	public class LayoutAttributesChangedEventArgs : EventArgs
	{
		public UICollectionViewLayoutAttributes NewAttributes { get; }

		public LayoutAttributesChangedEventArgs(UICollectionViewLayoutAttributes newAttributes) => NewAttributes = newAttributes;
	}
}
