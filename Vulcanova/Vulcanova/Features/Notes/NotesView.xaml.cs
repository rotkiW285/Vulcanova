using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Vulcanova.Core.Rx;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Notes;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class NotesView
{
    public NotesView()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, vm => vm.AccountViewModel, v => v.TitleView.ViewModel);

            this.OneWayBind(ViewModel, vm => vm.CurrentPeriodEntries, v => v.EntriesList.ItemsSource)
                .DisposeWith(disposable);

            this.WhenAnyObservable(v => v.ViewModel.GetNotesEntries.IsExecuting)
                .Select(v => !v)
                .BindTo(NoElementsView, x => x.IsVisible)
                .DisposeWith(disposable);

            this.BindForceRefresh(RefreshView, v => v.ViewModel.GetNotesEntries)
                .DisposeWith(disposable);

            this.Bind(ViewModel, vm => vm.SelectedPeriod, v => v.PeriodPicker.SelectedPeriod)
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.Periods, v => v.PeriodPicker.Periods)
                .DisposeWith(disposable);
        });
    }
}