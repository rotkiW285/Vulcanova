using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using FFImageLoading.Svg.Forms;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Core.Layout.Controls.Calendar;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class Calendar
{
    public static readonly BindableProperty SelectedDateProperty =
        BindableProperty.Create(nameof(SelectedDate), typeof(DateTime), typeof(Calendar), DateTime.Now);

    public DateTime SelectedDate
    {
        get => (DateTime) GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public static readonly BindableProperty SelectedColorProperty =
        BindableProperty.Create(nameof(SelectedColor), typeof(Color), typeof(CalendarDateCell), Color.Red);

    public Color SelectedColor
    {
        get => (Color) GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }
        
    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(CalendarDateCell), Color.Default);

    public Color TextColor
    {
        get => (Color) GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty SelectedTextColorProperty =
        BindableProperty.Create(nameof(SelectedTextColor), typeof(Color), typeof(CalendarDateCell), Color.White);

    public Color SelectedTextColor
    {
        get => (Color) GetValue(SelectedTextColorProperty);
        set => SetValue(SelectedTextColorProperty, value);
    }

    public static readonly BindableProperty SecondaryTextColorProperty =
        BindableProperty.Create(nameof(SecondaryTextColor), typeof(Color), typeof(CalendarDateCell), Color.White);

    public Color SecondaryTextColor
    {
        get => (Color) GetValue(SecondaryTextColorProperty);
        set => SetValue(SecondaryTextColorProperty, value);
    }
        
    public static readonly BindableProperty SelectionModeProperty =
        BindableProperty.Create(nameof(SelectionMode), typeof(CalendarSelectionMode), typeof(CalendarDateCell), CalendarSelectionMode.SingleDay);

    public CalendarSelectionMode SelectionMode
    {
        get => (CalendarSelectionMode) GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    public static readonly BindableProperty HighlightsProperty =
        BindableProperty.Create(nameof(Highlights), typeof(IEnumerable<DateTime>), typeof(Calendar),
            Array.Empty<DateTime>());

    public IEnumerable<DateTime> Highlights
    {
        get => (IEnumerable<DateTime>) GetValue(HighlightsProperty);
        set => SetValue(HighlightsProperty, value);
    }

    public static readonly BindableProperty WeekDisplayProperty =
        BindableProperty.Create(nameof(WeekDisplay), typeof(bool), typeof(Calendar),
            defaultValue: true);

    public bool WeekDisplay
    {
        get => (bool) GetValue(WeekDisplayProperty);
        set => SetValue(WeekDisplayProperty, value);
    }

    public Calendar()
    {
        InitializeComponent();

        this.WhenAnyValue(v => v.WeekDisplay)
            .Select(async value =>
            {
                ToggleModeArrow.Source = SvgImageSource.FromResource(
                    $"Vulcanova.Resources.Icons.chevron-{(value ? "down" : "up")}.svg");

                if (value)
                {
                    var scaleDown = new Animation(d => CalendarGrid.HeightRequest = d,
                        CalendarGrid.Height, CalendarWeekGrid.ExpectedHeight);

                    scaleDown.Commit(this, "ScaleDown", finished: (_, _) =>
                    {
                        CalendarGrid.IsVisible = false;
                        WeekGrid.IsVisible = true;
                        WeekGrid.Opacity = 0;
                        WeekGrid.FadeTo(1);
                    });

                    await CalendarGrid.FadeTo(0);
                }
                else
                {
                    await WeekGrid.FadeTo(0);

                    var scaleUp = new Animation(d => CalendarGrid.HeightRequest = d,
                        WeekGrid.Height, CalendarGrid.ExpectedHeight);

                    WeekGrid.IsVisible = false;
                    CalendarGrid.IsVisible = true;

                    scaleUp.Commit(this, "ScaleUp", finished: (_, _) =>
                    {
                        CalendarGrid.ClearValue(HeightRequestProperty);
                    });

                    await CalendarGrid.FadeTo(1);
                }
            })
            .Subscribe();

    }

    private void ToggleModeArrow_OnTapped(object sender, EventArgs e)
    {
        WeekDisplay = !WeekDisplay;
    }

    private void LeftArrow_OnTapped(object sender, EventArgs e)
    {
        SelectedDate = WeekDisplay ? SelectedDate.AddDays(-7) : SelectedDate.AddMonths(-1);
    }

    private void RightArrow_OnTapped(object sender, EventArgs e)
    {
        SelectedDate = WeekDisplay ? SelectedDate.AddDays(7) : SelectedDate.AddMonths(1);
    }
}

public enum CalendarSelectionMode
{
    SingleDay,
    Week
}