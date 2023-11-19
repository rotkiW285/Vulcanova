using Vulcanova.iOS.CollectionView;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(CollectionView), typeof(CollectionViewRenderer))]
namespace Vulcanova.iOS.CollectionView
{
	public class CollectionViewRenderer : GroupableItemsViewRenderer<GroupableItemsView, GroupableItemsViewController<GroupableItemsView>>
	{
		[Xamarin.Forms.Internals.Preserve(Conditional = true)]
		public CollectionViewRenderer() { }
	}
}