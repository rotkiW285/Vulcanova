using System.Reactive.Linq;
using System.Reactive.Disposables;
using ReactiveUI;
using Vulcanova.Core.Rx;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Attendance;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AttendanceDetailedView
{
    public AttendanceDetailedView()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.Bind(ViewModel, vm => vm.SelectedDay, v => v.Calendar.SelectedDate)
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.CurrentDayEntries, v => v.EntriesList.ItemsSource)
                .DisposeWith(disposable);

            this.WhenAnyObservable(v => v.ViewModel.GetAttendanceEntries.IsExecuting)
                .Select(v => !v)
                .BindTo(NoElementsView, x => x.IsVisible)
                .DisposeWith(disposable);

            this.BindForceRefresh(RefreshView, v => v.ViewModel.GetAttendanceEntries)
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.EnableJustificationMode, v => v.EnableJustificationButton.Command)
                .DisposeWith(disposable);

            this.WhenAnyObservable(v => v.ViewModel.EnableJustificationMode.CanExecute)
                .BindTo(this, v => v.EnableJustificationButton.IsVisible)
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.JustifyLessonsAttendance, v => v.JustifyButton.Command)
                .DisposeWith(disposable);

            this.WhenAnyObservable(v => v.ViewModel.JustifyLessonsAttendance.CanExecute)
                .BindTo(this, v => v.JustifyButton.IsVisible)
                .DisposeWith(disposable);
        });
    }
}