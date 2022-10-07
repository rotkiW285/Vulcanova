using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Vulcanova.Core.Layout;
using Vulcanova.Core.Rx;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Attendance;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AttendanceView
{
    public AttendanceView()
    {
        InitializeComponent();
            
        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, vm => vm.AccountViewModel, v => v.TitleView.ViewModel)
                .DisposeWith(disposable);

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
        });
    }
}