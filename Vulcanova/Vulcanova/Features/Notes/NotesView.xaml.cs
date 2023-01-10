using System.Linq;
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

            this.BindCommand(ViewModel, vm => vm.PreviousSemester, v => v.PreviousSemesterTap)
                .DisposeWith(disposable);

            this.BindCommand(ViewModel, vm => vm.NextSemester, v => v.NextSemesterTap)
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.PeriodInfo, v => v.PeriodNameLabel.Text,
                    p => p is null
                        ? string.Empty
                        : $"{p.YearStart}/{p.YearEnd} – {p.CurrentPeriod.Number}")
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.CurrentPeriodEntries, v => v.EntriesList.ItemsSource,
                    ex => ex?.GroupBy(x => x.DateModified)
                        .OrderBy(g => g.Key)
                        .Select(g => new NotesGroup(g.Key, g.ToList())))
                .DisposeWith(disposable);

            this.WhenAnyObservable(v => v.ViewModel.GetNotesEntries.IsExecuting)
                .Select(v => !v)
                .BindTo(NoElementsView, x => x.IsVisible)
                .DisposeWith(disposable);

            this.BindForceRefresh(RefreshView, v => v.ViewModel.GetNotesEntries)
                .DisposeWith(disposable);
        });
    }
}