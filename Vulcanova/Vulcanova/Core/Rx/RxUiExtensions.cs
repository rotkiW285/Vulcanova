using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using Xamarin.Forms;

namespace Vulcanova.Core.Rx;

public static class RxUiExtensions
{
    public static void SubscribeAndIgnoreErrors<T>(this IObservable<T> observable)
        => observable.Subscribe(_ => { }, _ => { });

    /// <summary>
    /// Utility for implementing force refresh by pull-to-refresh with RefreshView
    /// </summary>
    /// <param name="view">The containing view</param>
    /// <param name="refreshView">The RefreshView to apply bindings to</param>
    /// <param name="commandSelector">Force refresh command property selector</param>
    /// <param name="dontSetIsRefreshingToTrue">
    /// This workarounds the XF issue where the Header of a CollectionView
    /// surrounded with a RefreshView would end up cropped when setting <c>IsRefreshing</c>
    /// to <c>true</c>
    /// </param>
    /// <typeparam name="TView">The containing view type</typeparam>
    /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequences.</returns>
    public static IDisposable BindForceRefresh<TView>(this TView view,
        RefreshView refreshView,
        Expression<Func<TView, ICommand>> commandSelector,
        bool dontSetIsRefreshingToTrue = false)
        where TView : class, IViewFor
    {
        var disposable = new CompositeDisposable();

        refreshView.Events().Refreshing
            .Select(_ => true) // forceRefresh = true
            .InvokeCommand(view, commandSelector)
            .DisposeWith(disposable);

        view.WhenAnyValue(commandSelector)
            .WhereNotNull()
            .Subscribe(cmd => ((IReactiveCommand)cmd).IsExecuting
                .Subscribe(value =>
                {
                    if (!dontSetIsRefreshingToTrue || !value)
                    {
                        refreshView.IsRefreshing = value;
                    }
                })
                .DisposeWith(disposable))
            .DisposeWith(disposable);

        return disposable;
    }

    /// <summary>
    /// Utility for working around the https://github.com/xamarin/Xamarin.Forms/issues/13323 Xamarin.Forms issue
    /// </summary>
    /// <param name="view">The containing view type.</param>
    /// <param name="viewModel">The view model.</param>
    /// <param name="vmProperty">The property that is bound on the view model.</param>
    /// <param name="viewProperty">The view property that is to be bound.</param>
    /// <param name="delay">The delay to wait before applying the binding.</param>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TVMProp">The type of view model property.</typeparam>
    /// <typeparam name="TVProp">The type of the property bound on the view.</typeparam>
    /// <returns></returns>
    public static IDisposable OneWayBind<TViewModel, TView, TVMProp, TVProp>(this TView view,
        TViewModel viewModel,
        Expression<Func<TViewModel, TVMProp>> vmProperty,
        Expression<Func<TView, TVProp>> viewProperty,
        TimeSpan delay)
        where TView : class, IViewFor
    {
        return view.WhenAnyValue(v => (TViewModel) v.ViewModel)
            .WhereNotNull()
            .Delay(delay)
            .Select(vm => vm.WhenAnyValue(vmProperty))
            .Switch()
            .ObserveOn(RxApp.MainThreadScheduler)
            .BindTo(view, viewProperty);
    }

    /// <summary>
    /// Utility for working around the https://github.com/xamarin/Xamarin.Forms/issues/13323 Xamarin.Forms issue
    /// </summary>
    /// <param name="view">The containing view type.</param>
    /// <param name="viewModel">The view model.</param>
    /// <param name="vmProperty">The property that is bound on the view model.</param>
    /// <param name="viewProperty">The view property that is to be bound.</param>
    /// <param name="selector">The value selector.</param>
    /// <param name="delay">The delay to wait before applying the binding.</param>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TVMProp">The type of view model property.</typeparam>
    /// <typeparam name="TVProp">The type of the property bound on the view.</typeparam>
    /// <returns></returns>
    public static IDisposable OneWayBind<TViewModel, TView, TVMProp, TVProp>(this TView view,
        TViewModel viewModel,
        Expression<Func<TViewModel, TVMProp>> vmProperty,
        Expression<Func<TView, TVProp>> viewProperty,
        Func<TVMProp, TVProp> selector,
        TimeSpan delay)
        where TView : class, IViewFor
    {
        return view.WhenAnyValue(v => (TViewModel) v.ViewModel)
            .WhereNotNull()
            .Delay(delay)
            .Select(vm => vm.WhenAnyValue(vmProperty))
            .Switch()
            .Select(selector)
            .ObserveOn(RxApp.MainThreadScheduler)
            .BindTo(view, viewProperty);
    }
}