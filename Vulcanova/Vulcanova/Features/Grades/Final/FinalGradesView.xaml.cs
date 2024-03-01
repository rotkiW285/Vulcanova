using System.Reactive.Disposables;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Vulcanova.Core.Rx;

namespace Vulcanova.Features.Grades.Final;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class FinalGradesView
{
    public FinalGradesView()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            static IEnumerable GradesSelector(IEnumerable<object> grades)
            {
                if (grades == null)
                {
                    return null;
                }

                var enumeratedGrades = grades as object[] ?? grades.ToArray();

                return enumeratedGrades.OfType<FinalGradesEntry>()
                    .All(g => g.PredictedGrade == null && g.FinalGrade == null)
                    ? null
                    : enumeratedGrades;
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                this.OneWayBind(ViewModel, vm => vm.FinalGradeItems, v => v.FinalGradesList.ItemsSource, GradesSelector, TimeSpan.FromMilliseconds(50))
                    .DisposeWith(disposable);
            }
            else
            {
                this.OneWayBind(ViewModel, vm => vm.FinalGradeItems, v => v.FinalGradesList.ItemsSource, GradesSelector)
                    .DisposeWith(disposable);
            }

            this.BindForceRefresh(RefreshView, v => v.ViewModel.GetFinalGrades, true)
                .DisposeWith(disposable);
        });
    }
}