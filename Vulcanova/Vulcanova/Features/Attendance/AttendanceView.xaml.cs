using System;
using System.Linq;
using System.Reactive.Disposables;
using ReactiveUI;
using Vulcanova.Core.Layout;
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
            this.Bind(ViewModel, vm => vm.SelectedViewModelIndex, v => v.TabHost.SelectedIndex)
                .DisposeWith(disposable);

            this.Bind(ViewModel, vm => vm.SelectedViewModelIndex, v => v.Switcher.SelectedIndex)
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.AttendanceDetailedViewModel, v => v.AttendanceDetailedView.ViewModel)
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.AttendanceReportViewModel, v => v.AttendanceReportView.ViewModel)
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.AccountViewModel, v => v.TitleView.ViewModel)
                .DisposeWith(disposable);

            this.WhenAnyValue(v => v.ViewModel.AttendanceDetailedViewModel.JustificationMode)
                .Subscribe(v =>
                {
                    if (v)
                    {
                        TitleView.IsVisible = false;

                        if (!ToolbarItems.Any())
                        {
                            ToolbarItems.Add(new ToolbarItem(Strings.CommonCancel, null, () => { })
                            {
                                Command = ViewModel.AttendanceDetailedViewModel.DisableJustificationMode
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
            
            // workaround https://github.com/xamarin/Xamarin.Forms/issues/1336 as this breaks pushing modals
            if (Application.Current.MainPage is NavigationPage {CurrentPage: HomeTabbedPage {CurrentPage: null} page})
            {
                page.CurrentPage = this;
            }
        });
    }
}