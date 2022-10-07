using System.Threading.Tasks;
using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
using Prism.Plugin.Popups;
using Rg.Plugins.Popup.Contracts;
using Xamarin.Forms;

namespace Vulcanova.Core.Layout;

public class SheetPageNavigationService : PopupPageNavigationService
{
    private readonly ISheetPopper _popper;

    public SheetPageNavigationService(IPopupNavigation popupNavigation, IContainerProvider container,
        IApplicationProvider applicationProvider, IPageBehaviorFactory pageBehaviorFactory,
        ISheetPopper popper = null) : base(
        popupNavigation, container, applicationProvider, pageBehaviorFactory)
    {
        _popper = popper;
    }

    protected override async Task DoPush(Page currentPage, Page page, bool? useModalNavigation, bool animated,
        bool insertBeforeLast = false,
        int navigationOffset = 0)
    {
        switch (page)
        {
            case SheetPage when _popper != null:
                _popper.PushSheet(page);
                break;
            default:
                await base.DoPush(currentPage, page, useModalNavigation, animated, insertBeforeLast, navigationOffset);
                break;
        }
    }

    protected override Task<Page> DoPop(INavigation navigation, bool useModalNavigation, bool animated)
    {
        if (_page is SheetPage && _popper != null)
        {
            _popper.PopSheet(_page);
        }

        return base.DoPop(navigation, useModalNavigation, animated);
    }
}