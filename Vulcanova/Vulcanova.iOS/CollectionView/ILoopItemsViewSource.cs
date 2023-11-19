namespace Vulcanova.iOS.CollectionView
{
    public interface ILoopItemsViewSource : IItemsViewSource
    {
        bool Loop { get; set; }

        int LoopCount { get; }
    }
}