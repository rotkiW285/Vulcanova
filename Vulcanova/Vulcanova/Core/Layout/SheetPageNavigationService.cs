using System;
using System.Threading.Tasks;
using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
using Prism.Navigation;
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
            _popper.PopSheet();
        }

        return base.DoPop(navigation, useModalNavigation, animated);
    }

    #region GoBackInternal hack
    // Most of the code below is copy pasted from the original PageNavigationService
    // since I was unable to call these methods due to their access protection level
    // and I didn't want to bother myself with too much reflection.
    protected override async Task<INavigationResult> GoBackInternal(INavigationParameters parameters, bool? useModalNavigation, bool animated)
    {
        var result = new NavigationResult();
        
        Page page;
        try
        {
            NavigationSource = PageNavigationSource.NavigationService;

            page = GetCurrentPage();

            if (IsRoot(_applicationProvider.MainPage, page))
                throw new NavigationException(NavigationException.CannotPopApplicationMainPage, page);

            var segmentParameters = UriParsingHelper.GetSegmentParameters(null, parameters);
            segmentParameters.Add("__NavigationMode", "Back");

            var canNavigate = await PageUtilities.CanNavigateAsync(page, segmentParameters);
            if (!canNavigate)
            {
                result.Exception = new NavigationException(NavigationException.IConfirmNavigationReturnedFalse, page);
                return result;
            }

            var useModalForDoPop = UseModalGoBack(page, useModalNavigation);

            var previousPage =
                PageUtilities.GetOnNavigatedToTarget(page, _applicationProvider.MainPage, useModalForDoPop);

            if (previousPage.Navigation.ModalStack.Count == 1 && useModalForDoPop && _popper.DisplayedSheet != null)
            {
                previousPage = _popper.DisplayedSheet;
            }
            
            var poppedPage = await DoPop(page.Navigation, useModalForDoPop, animated);
            
            if (poppedPage != null)
            {
                PageUtilities.OnNavigatedFrom(page, segmentParameters);
                PageUtilities.OnNavigatedTo(previousPage, segmentParameters);
                PageUtilities.DestroyPage(poppedPage);

                result.Success = true;
                return result;
            }
        }
        catch (Exception ex)
        {
            result.Exception = ex;
            return result;
        }
        finally
        {
            NavigationSource = PageNavigationSource.Device;
        }

        result.Exception = GetGoBackException(page, _applicationProvider.MainPage);
        return result;
    }
    
    internal bool UseModalGoBack(Page currentPage, bool? useModalNavigationDefault)
    {
        if (useModalNavigationDefault.HasValue)
            return useModalNavigationDefault.Value;
        else if (currentPage is NavigationPage navPage)
            return GoBackModal(navPage);
        else if (HasNavigationPageParent(currentPage, out var navParent))
            return GoBackModal(navParent);
        else
            return true;
    }

    private bool GoBackModal(NavigationPage navPage)
    {
        if (navPage.CurrentPage != navPage.RootPage)
            return false;
        else if (navPage.CurrentPage == navPage.RootPage && navPage.Parent is Application && _applicationProvider.MainPage != navPage)
            return true;
        else if (navPage.Parent is TabbedPage tabbed && tabbed != _applicationProvider.MainPage)
            return true;
        else if (navPage.Parent is CarouselPage carousel && carousel != _applicationProvider.MainPage)
            return true;

        return false;
    }
    
    static bool HasNavigationPageParent(Page page, out NavigationPage navigationPage)
    {
        if (page?.Parent != null)
        {
            if (page.Parent is NavigationPage navParent)
            {
                navigationPage = navParent;
                return true;
            }
            else if ((page.Parent is TabbedPage || page.Parent is CarouselPage) && page.Parent?.Parent is NavigationPage navigationParent)
            {
                navigationPage = navigationParent;
                return true;
            }
        }

        navigationPage = null;
        return false;
    }
    
    private static Exception GetGoBackException(Page currentPage, Page mainPage)
    {
        if (IsMainPage(currentPage, mainPage))
        {
            return new NavigationException(NavigationException.CannotPopApplicationMainPage, currentPage);
        }
        else if ((currentPage is NavigationPage navPage && IsOnNavigationPageRoot(navPage)) ||
                 (currentPage.Parent is NavigationPage navParent && IsOnNavigationPageRoot(navParent)))
        {
            return new NavigationException(NavigationException.CannotGoBackFromRoot, currentPage);
        }

        return new NavigationException(NavigationException.UnknownException, currentPage);
    }
    
    private static bool IsOnNavigationPageRoot(NavigationPage navigationPage) =>
        navigationPage.CurrentPage == navigationPage.RootPage;

    private static bool IsMainPage(Page currentPage, Page mainPage)
    {
        if (currentPage == mainPage)
        {
            return true;
        }
        else if (mainPage is FlyoutPage flyout && flyout.Detail == currentPage)
        {
            return true;
        }
        else if (currentPage.Parent is TabbedPage tabbed && mainPage == tabbed)
        {
            return true;
        }
        else if (currentPage.Parent is CarouselPage carousel && mainPage == carousel)
        {
            return true;
        }
        else if (currentPage.Parent is NavigationPage navPage && navPage.CurrentPage == navPage.RootPage)
        {
            return IsMainPage(navPage, mainPage);
        }

        return false;
    }
    #endregion
}