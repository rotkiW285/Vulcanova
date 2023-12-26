using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Shared;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class PeriodPicker
{
    public static BindableProperty PeriodsProperty = BindableProperty.Create(nameof(Periods),
        typeof(IEnumerable<Period>),
        typeof(PeriodPicker), Array.Empty<Period>());
    
    public IEnumerable<Period> Periods
    {
        get => (IEnumerable<Period>) GetValue(PeriodsProperty);
        set => SetValue(PeriodsProperty, value);
    }

    public static BindableProperty SelectedPeriodProperty = BindableProperty.Create(nameof(SelectedPeriod),
        typeof(Period),
        typeof(PeriodPicker), null);
    
    public Period SelectedPeriod
    {
        get => (Period) GetValue(SelectedPeriodProperty);
        set => SetValue(SelectedPeriodProperty, value);
    }

    public PeriodPicker()
    {
        InitializeComponent();

        var orderedPeriods = this.WhenAnyValue(v => v.Periods)
            .WhereNotNull()
            .Select(x => x.OrderBy(p => p.Start).ToList());

        var selectedPeriodWithPeriodsList = this.WhenAnyValue(v => v.SelectedPeriod)
            .CombineLatest(orderedPeriods)
            .Where(v => v is { Item1: { }, Item2.Count: > 0 })
            .Where(v => v.Second.Contains(v.First))
            .Publish();

        selectedPeriodWithPeriodsList
            .Select(p =>
            {
                var currentPeriod = p.Item1;
                var allPeriods = p.Item2;

                var allPeriodsInYear = allPeriods.Where(x => x.Level == currentPeriod.Level)
                    .ToArray();

                var yearStart = allPeriodsInYear.First().Start.Year;
                var yearEnd = allPeriodsInYear.Last().End.Year;

                return $"{yearStart}/{yearEnd} – {currentPeriod.Number}";
            })
            .BindTo(this, v => v.PeriodNameLabel.Text);
        
        var hasNextPeriodObservable = selectedPeriodWithPeriodsList
            .Select(p =>
            {
                var currentPeriod = p.Item1;
                var allPeriods = p.Item2;
        
                var hasNextPeriod = allPeriods.Last() != currentPeriod;
        
                return hasNextPeriod;
            });
        
        hasNextPeriodObservable.BindTo(this, v => v.NextImg.IsVisible);
        
        var hasPreviousPeriodObservable = selectedPeriodWithPeriodsList
            .Select(p =>
            {
                var currentPeriod = p.Item1;
                var allPeriods = p.Item2;
        
                var hasPreviousPeriod = allPeriods.First() != currentPeriod;
        
                return hasPreviousPeriod;
            });
        
        hasPreviousPeriodObservable.BindTo(this, v => v.PreviousImg.IsVisible);
        
        NextTap.Command = ReactiveCommand.Create((Period p) =>
        {
            SelectedPeriod = p;
        }, hasNextPeriodObservable);

        orderedPeriods
            .CombineLatest(this.WhenAnyValue(v => v.SelectedPeriod).WhereNotNull())
            .Select(v =>
            {
                var currentIndex = v.First.IndexOf(v.Second);
                return v.First.ElementAtOrDefault(++currentIndex);
            })
            .BindTo(this, v => v.NextTap.CommandParameter);
        
        PreviousTap.Command = ReactiveCommand.Create((Period p) =>
        {
            SelectedPeriod = p;
        }, hasPreviousPeriodObservable);

        orderedPeriods
            .CombineLatest(this.WhenAnyValue(v => v.SelectedPeriod).WhereNotNull())
            .Select(v =>
            {
                var currentIndex = v.First.IndexOf(v.Second);
                return v.First.ElementAtOrDefault(--currentIndex);
            })
            .BindTo(this, v => v.PreviousTap.CommandParameter);

        selectedPeriodWithPeriodsList.Connect();
    }
}