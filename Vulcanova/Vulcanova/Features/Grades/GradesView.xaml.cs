using System.Reactive.Disposables;
using ReactiveUI;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Grades;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class GradesView
{
    public GradesView()
    {
        InitializeComponent();
            
        this.WhenActivated(disposable =>
        {
            this.Bind(ViewModel, vm => vm.SelectedViewModelIndex, v => v.TabHost.SelectedIndex)
                .DisposeWith(disposable);

            this.Bind(ViewModel, vm => vm.SelectedViewModelIndex, v => v.Switcher.SelectedIndex)
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.GradesSummaryViewModel, v => v.GradesSummaryView.ViewModel)
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.FinalGradesViewModel, v => v.FinalGradesView.ViewModel)
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.AccountViewModel, v => v.TitleView.ViewModel)
                .DisposeWith(disposable);
            
            this.Bind(ViewModel, vm => vm.SelectedPeriod, v => v.PeriodPicker.SelectedPeriod)
                .DisposeWith(disposable);
            
            this.OneWayBind(ViewModel, vm => vm.Periods, v => v.PeriodPicker.Periods)
                .DisposeWith(disposable);
        });
    }
}