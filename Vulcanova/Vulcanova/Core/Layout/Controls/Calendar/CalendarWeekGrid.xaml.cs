using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Vulcanova.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Core.Layout.Controls.Calendar;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CalendarWeekGrid
{
    private const int HeaderRowHeight = 17;
    private const int DayRowHeight = 42;

    public const double ExpectedHeight = HeaderRowHeight + DayRowHeight;

    public static readonly BindableProperty SelectedDateProperty =
        BindableProperty.Create(nameof(SelectedDate), typeof(DateTime), typeof(Calendar), DateTime.Now,
            propertyChanged: SelectedDateChanged);

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
        BindableProperty.Create(nameof(SelectionMode), typeof(CalendarSelectionMode), typeof(CalendarDateCell),
            CalendarSelectionMode.SingleDay,
            propertyChanged: SelectionModeChanged);

    public CalendarSelectionMode SelectionMode
    {
        get => (CalendarSelectionMode) GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    public static readonly BindableProperty HighlightsProperty =
        BindableProperty.Create(nameof(Highlights), typeof(IEnumerable<DateTime>), typeof(Calendar),
            Array.Empty<DateTime>(), propertyChanged: HighlightsChanged);

    public IEnumerable<DateTime> Highlights
    {
        get => (IEnumerable<DateTime>) GetValue(HighlightsProperty);
        set => SetValue(HighlightsProperty, value);
    }

    private readonly Dictionary<DateTime, CalendarDateCell> _dateCells = new();

    public CalendarWeekGrid()
    {
        InitializeComponent();

        SetupLayout();
    }

    private void SetupLayout()
    {
        SetupHeader();
        CreateCalendarGrid();
    }

    private void SetupHeader()
    {
        CalendarGrid.RowDefinitions.Add(new RowDefinition {Height = HeaderRowHeight});

        // 3.01.2022 is on Monday
        var date = new DateTime(2022, 1, 3);

        for (var day = 0; day < 7; day++)
        {
            var text = date.ToString("ddd", CultureInfo.CurrentCulture).ToUpperInvariant()[..1];
            var label = new Label
            {
                Text = text,
                HorizontalTextAlignment = TextAlignment.Center
            };
            label.FontSize = Device.GetNamedSize(NamedSize.Small, label);

            CalendarGrid.Children.Add(label, day, 0);

            date = date.AddDays(1);
        }
    }

    private void CreateCalendarGrid()
    {
        CalendarGrid.RowDefinitions.Add(new RowDefinition {Height = DayRowHeight});

        var lastMonday = SelectedDate.LastMonday();

        // offset by 7 columns -- header
        for (var i = 6; i < 13; i++)
        {
            var dayOffset = i - 6;

            var date = lastMonday.AddDays(dayOffset);

            var cell = CreateCellForDate(date);

            CalendarGrid.Children.Add(cell, dayOffset, 1);

            _dateCells[date] = cell;
        }
    }

    private CalendarDateCell CreateCellForDate(DateTime date)
    {
        var cell = new CalendarDateCell
        {
            Day = date.Day,
            TapCommand = new Command(() => SelectedDate = date),
            Secondary = date.Month != SelectedDate.Month
        };

        var colorBinding = new Binding
        {
            Source = this,
            Path = nameof(SelectedColor)
        };

        cell.SetBinding(CalendarDateCell.SelectedColorProperty, colorBinding);

        var textColorBinding = new Binding
        {
            Source = this,
            Path = nameof(TextColor)
        };

        cell.SetBinding(CalendarDateCell.TextColorProperty, textColorBinding);

        var selectedTextColorBinding = new Binding
        {
            Source = this,
            Path = nameof(SelectedTextColor)
        };

        cell.SetBinding(CalendarDateCell.SelectedTextColorProperty, selectedTextColorBinding);

        var secondaryColorBinding = new Binding
        {
            Source = this,
            Path = nameof(SecondaryTextColor)
        };

        cell.SetBinding(CalendarDateCell.SecondaryTextColorProperty, secondaryColorBinding);

        return cell;
    }

    private static void SelectedDateChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        var grid = (CalendarWeekGrid) bindable;

        grid.UpdateAllCells();
    }

    private static void SelectionModeChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        var grid = (CalendarWeekGrid) bindable;

        grid.UpdateAllCells();
    }

    private void UpdateAllCells()
    {
        var firstDay = SelectedDate.LastMonday();

        foreach (var (kvp, index) in _dateCells
                     .OrderBy(kvp => kvp.Key)
                     .Select((c, i) => (c, i)))
        {
            UpdateCellForDate(kvp.Value, kvp.Key, firstDay.AddDays(index));
        }
    }

    private void UpdateCellForDate(CalendarDateCell cell, DateTime previousDate, DateTime newDate)
    {
        // When SelectionMode is "week" each date cell in the grid will be selected.
        // That happens because we always display the selection.
        // Considering that we display a single week, the displayed week will always be the selected one.
        cell.Selected = SelectionMode != CalendarSelectionMode.SingleDay
                        || SelectedDate == newDate;

        cell.Day = newDate.Day;
        cell.TapCommand = new Command(() => SelectedDate = newDate);
        cell.Secondary = newDate.Month != SelectedDate.Month;
        cell.IsHighlight = Highlights.Contains(newDate);

        _dateCells.Remove(previousDate);
        _dateCells[newDate] = cell;
    }

    private void SwipeGestureRecognizer_OnSwiped(object sender, SwipedEventArgs e)
    {
        SelectedDate = SelectedDate.AddDays(7 *
                                            e.Direction switch
                                            {
                                                SwipeDirection.Left => 1,
                                                SwipeDirection.Right => -1,
                                                _ => 0
                                            });
    }
    
    private static void HighlightsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var calendar = (CalendarWeekGrid) bindable;
        var newHighlights = (newValue as IEnumerable<DateTime>)?.ToArray();

        foreach (var (date, cell) in calendar._dateCells)
        {
            cell.IsHighlight = newHighlights?.Select(h => h.Date).Contains(date) ?? false;
        }
    }
}