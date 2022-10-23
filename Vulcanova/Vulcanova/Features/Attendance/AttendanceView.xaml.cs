using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Vulcanova.Core.Rx;
using Vulcanova.Resources;
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

            this.OneWayBind(ViewModel, vm => vm.EnableJustificationMode, v => v.EnableJustificationButton.Command)
                .DisposeWith(disposable);

            this.WhenAnyObservable(v => v.ViewModel.EnableJustificationMode.CanExecute)
                .BindTo(this, v => v.EnableJustificationButton.IsVisible)
                .DisposeWith(disposable);

            this.WhenAnyValue(v => v.ViewModel.JustificationMode)
                .Subscribe(v =>
                {
                    if (v)
                    {
                        TitleView.IsVisible = false;

                        if (!ToolbarItems.Any())
                        {
                            ToolbarItems.Add(new ToolbarItem(Strings.CommonCancel, null, () => { })
                            {
                                Command = ViewModel.DisableJustificationMode
                            });
                        }
                    }
                    else
                    {
                        ToolbarItems.Clear();
                        TitleView.IsVisible = true;
                    }
                })
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.JustifyLessonsAttendance, v => v.JustifyButton.Command)
                .DisposeWith(disposable);

            this.WhenAnyObservable(v => v.ViewModel.JustifyLessonsAttendance.CanExecute)
                .BindTo(this, v => v.JustifyButton.IsVisible)
                .DisposeWith(disposable);
        });
    }
}