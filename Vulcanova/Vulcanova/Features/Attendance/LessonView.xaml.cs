using System;
using System.Reactive.Linq;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Attendance;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class LessonView
{
    public static readonly BindableProperty LessonProperty =
        BindableProperty.Create(nameof(Lesson), typeof(Lesson), typeof(LessonView));

    public Lesson Lesson
    {
        get => (Lesson) GetValue(LessonProperty);
        set => SetValue(LessonProperty, value);
    }

    public static readonly BindableProperty JustificationModeProperty =
        BindableProperty.Create(nameof(JustificationMode), typeof(bool), typeof(LessonView));

    public bool JustificationMode
    {
        get => (bool) GetValue(JustificationModeProperty);
        set => SetValue(JustificationModeProperty, value);
    }
    
    public static readonly BindableProperty IsSelectedProperty =
        BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(LessonView));

    public bool IsSelected
    {
        get => (bool) GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public LessonView()
    {
        InitializeComponent();

        const int targetAnimWidth = 40;

        var showCol = new Animation(v => Col1.Width = v, 0, targetAnimWidth, Easing.Linear);
        var hideCol = new Animation(v => Col1.Width = v, targetAnimWidth, 0, Easing.Linear);

        this.WhenAnyValue(v => v.JustificationMode, v => v.Lesson)
            .Where(o => o.Item2 != null)
            .Select(o => o.Item1 && o.Item2.CanBeJustified)
            .Select(o => (bool?)o)
            .StartWith((bool?) null)
            .Buffer(2, 1)
            .Where(x => x.Count > 1)
            .Where(x =>
            {
                var prev = x[0];
                var curr = x[1]!.Value;

                if (prev == null && curr)
                {
                    return true;
                }

                return prev.HasValue && prev != curr;
            })
            .Select(v => v[1]!.Value)
            .Subscribe(v =>
            {
                this.AbortAnimation("GridColWidthAnim");
                (v ? showCol : hideCol).Commit(this, "GridColWidthAnim", length: 150);
            });
    }
}