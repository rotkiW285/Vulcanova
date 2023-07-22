using System.Reactive.Disposables;
using ReactiveUI;
using Vulcanova.Core.Rx;
using Vulcanova.Resources;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Dashboard;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class DashboardView
{
    public DashboardView()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, 
                    vm => vm.DashboardModel.LuckyNumber, 
                    v => v.LuckyNumberLabel.Text,
                    l => l?.Number != 0 ? l?.Number.ToString() : Strings.NoLuckyNumberShort)
                .DisposeWith(disposable);
            
            this.OneWayBind(ViewModel, vm => vm.SelectedDay, v => v.DateAndTime.Text, date => date.ToString("ddd, dd MMMM"))
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, vm => vm.DashboardModel.Timetable, v => v.TimetableCollection.BindingContext)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, vm => vm.DashboardModel.Exams, v => v.ExamsCollection.BindingContext)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, vm => vm.DashboardModel.Homework, v => v.HomeworkCollection.BindingContext)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, vm => vm.DashboardModel.Grades, v => v.GradesCollection.BindingContext)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, vm => vm.AccountViewModel, v => v.TitleView.ViewModel)
                .DisposeWith(disposable);

            this.BindForceRefresh(RefreshView, v => v.ViewModel.RefreshData, true)
                .DisposeWith(disposable);
        });
    }
}